using Convos.Entities;
using Convos.Managers;
using Convos.Results;
using Convos.Utilities;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Convos.Controllers
{
    /// <summary>
    /// An instance of IConvoContract 
    /// </summary>
    public class ConvoContract : IConvoContract
    {
        #region " Members "
        /// <summary>
        /// This constant defines the resource path for Convos
        /// </summary>
        private const string CONVO_RESOURCE_PATH = "{0}/api/Convo/{1}";

        /// <summary>
        /// This constant defines the resource path for Messages
        /// </summary>
        private const string MESSAGE_RESOURCE_PATH = "{0}/api/Convo/{1}/Message/{2}";

        /// <summary>
        /// Query string for paging variables
        /// </summary>
        private const string PAGING_QUERYSTRING = "?index={0}&count={1}&before={2}";

        /// <summary>
        /// The instance of IConvoManager that is going to interface with the database
        /// </summary>
        private readonly IConvoManager _convoManager;

        /// <summary>
        /// The base uri for creating uri's
        /// </summary>
        private static readonly string _baseUri;

        /// <summary>
        /// The maximum amount of time a list of Convos should be cached in seconds
        /// </summary>
        private static readonly int _convoListMaxAge = 15;

        /// <summary>
        /// The maximum amount of time a specific convo should be cached in seconds
        /// </summary>
        private static readonly int _convoMaxAge = 3600;

        /// <summary>
        /// The maximum amount of time a list of Messages should be cached in seconds
        /// </summary>
        private static readonly int _messageListMaxAge = 15;

        /// <summary>
        /// The maximum amount of time a specific convo should be cached in seconds
        /// </summary>
        private static readonly int _messageMaxAge = 3600;
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Constructor that gets a concrete instance of IConvoManager injected
        /// </summary>
        /// <param name="manager">The concrete instance of IConvoManager</param>
        public ConvoContract(IConvoManager manager)
        {
            _convoManager = manager;
        }

        /// <summary>
        /// Static constructor that initializes values from the configuration file
        /// </summary>
        static ConvoContract()
        {
            _baseUri = ConfigurationManager.AppSettings["BaseUri"];
            int.TryParse(ConfigurationManager.AppSettings["ConvoListMaxAge"], out _convoListMaxAge);
            int.TryParse(ConfigurationManager.AppSettings["ConvoMaxAge"], out _convoMaxAge);
            int.TryParse(ConfigurationManager.AppSettings["MessageListMaxAge"], out _messageListMaxAge);
            int.TryParse(ConfigurationManager.AppSettings["MessageMaxAge"], out _messageMaxAge);
        }

        /// <summary>
        /// This function handles a web api request to delete a Convo
        /// </summary>
        /// <param name="convoId">The ID of the Convo</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage DeleteConvo(long convoId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            if (request.TryGetUserId(out userId, result))
            {
                _convoManager.TryDeleteConvo(convoId, ref result, userId);
            }
            return GetResponse(request, result);
        }

        /// <summary>
        /// This function handles a web api request to delete a Message
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="messageId">The Id of the Message to delete</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage DeleteMessage(long convoId, long messageId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            if (request.TryGetUserId(out userId, result))
            {
                _convoManager.TryDeleteMessage(convoId, messageId, ref result, userId);
            }
            return GetResponse(request, result);
        }

        /// <summary>
        /// This function gets a list of Convos
        /// </summary>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage GetConvo(HttpRequestMessage request)
        {
            var result = new Result();
            int count;
            int index = request.GetPageIndex(result);
            DateTime before;
            long userId;
            int total;
            DateTime maxCreated;
            ConvoResponse[] convoResponses;

            if (!request.TryGetPageCount(out count, result) ||
                !request.TryGetPagingTimestamp(out before, result) ||
                !request.TryGetUserId(out userId, result) ||
                !_convoManager.TryGetAllConvos(before, count, index, out maxCreated, out convoResponses, ref result, out total, userId))
            {
                return GetResponse(request, result);
            }
            var response = GetResponse(request, result, convoResponses);
            // Add the total and relative counts to the header
            response.AddTotalCountHeader(total);
            response.AddRelativeCountHeader(convoResponses.Count());

            var minDate = convoResponses.Min(c => c.DateOfLastMessage);
            // Add locations for the next set of results
            if ((index*count) + count < total)
            {
                var pagingQueryString = string.Format(PAGING_QUERYSTRING, 
                    index + 1, 
                    count,
                    // If the current set of Convos has all null values for the DateOfLastMessage, just carry over the "before" date from the current request
                    HttpUtility.UrlEncode(minDate == null ? before.ToString("s") : ((DateTime)minDate).ToString("s"))); 
                response.AddNextPageHeader(string.Format(CONVO_RESOURCE_PATH, _baseUri, pagingQueryString));
            }

            // Add the date of the last created convo before the "before" time in the "Last Modified" header since the convo list is sorted by
            // the date they are created. A newer Convo inserted would change the convos seen per page, and therefore gateway and proxy
            // caches should know to invalidate.
            response.AddLastModifiedHeader(maxCreated);

            // Add cache directives to control caching
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(_convoListMaxAge),
                MustRevalidate = true,
                Private = true
            };
            return response;
        }

        /// <summary>
        /// This function gets a specific Convo by Id
        /// </summary>
        /// <param name="convoId">The Id of the convo to get</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage GetConvo(long convoId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            ConvoResponse convoResponse;
            if (!request.TryGetUserId(out userId, result) ||
                !_convoManager.TryGetConvoById(convoId, out convoResponse, ref result, userId))
            {
                return GetResponse(request, result);
            }

            var response = GetResponse(request, result, convoResponse);

            // Add the last modified header as the last time the convo was modified in the db
            response.AddLastModifiedHeader(convoResponse.DateUpdated);

            // Add cache directives to control caching
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(_convoMaxAge),
                MustRevalidate = true,
                Private = true
            };
            return response;
        }

        /// <summary>
        /// This function gets all messages and their replies for a specific Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo from which to get the Messages</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage GetMessage(long convoId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            int count;
            int index = request.GetPageIndex(result);
            DateTime before;
            DateTime maxCreated;
            MessageResponse[] messageResponses;
            int total;
            if (!request.TryGetPageCount(out count, result) ||
                !request.TryGetPagingTimestamp(out before, result) ||
                !request.TryGetUserId(out userId, result) ||
                !_convoManager.TryGetAllMessages(before, convoId, count, index, out maxCreated, out messageResponses, ref result, out total, userId))
            {
                return GetResponse(request, result);
            }
            var response = GetResponse(request, result, messageResponses);

            // Add the total and relative counts to the header
            response.AddTotalCountHeader(total);
            response.AddRelativeCountHeader(messageResponses.Count());

            var minDate = messageResponses.Min(m => m.DateCreated).ToString("s");
            // Add locations for the next set of results
            if ((index * count) + count < total)
            {
                var pagingQueryString = string.Format(PAGING_QUERYSTRING, 
                    index + 1, 
                    count,
                    HttpUtility.UrlEncode(minDate));
                response.AddNextPageHeader(string.Format(MESSAGE_RESOURCE_PATH, _baseUri, convoId, pagingQueryString));
            }

            // Add the date of the last created Message before the "before" time in the "Last Modified" header since the Message list is sorted by
            // the date they are created. A newer Message inserted would change the Messages seen per page, and therefore gateway and proxy
            // caches should know to invalidate.
            response.AddLastModifiedHeader(maxCreated);

            // Add cache directives to control caching
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(_messageListMaxAge),
                MustRevalidate = true,
                Private = true
            };
            return response;
        }

        /// <summary>
        /// This function gets a specific Message within a Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo from which to get the Message</param>
        /// <param name="messageId">The Id of the Message to get</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage GetMessage(long convoId, long messageId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            MessageResponse messageResponse;
            if (!request.TryGetUserId(out userId, result) ||
                !_convoManager.TryGetMessageById(convoId, messageId, out messageResponse, ref result, userId))
            {
                return GetResponse(request, result);
            }
            var response = GetResponse(request, result, messageResponse);

            // Add the last modified header as the last time the convo was modified in the db
            response.AddLastModifiedHeader(messageResponse.DateUpdated);

            // Add cache directives to control caching
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(_messageMaxAge),
                MustRevalidate = true,
                Private = true
            };
            return response;
        }

        /// <summary>
        /// This function partially updates a Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo to update</param>
        /// <param name="request">The http request message that was made</param>
        /// <param name="subject">The new subject of the Convo (if this value is null, it is ignored)</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage PatchConvo(long convoId, HttpRequestMessage request, string subject)
        {
            var result = new Result();
            long userId;
            if (request.TryGetUserId(out userId, result))
            {
                _convoManager.TryPatchConvo(convoId, ref result, subject, userId);
            }
            return GetResponse(request, result);
        }

        /// <summary>
        /// This function partially updates a Message
        /// </summary>
        /// <param name="body">The new body of the message (if this value is null, it is ignored)</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="isRead">A new boolean value to indicate whether or not this message was read(if this value is null, it is ignored)</param>
        /// <param name="messageId">The Id of the Message to partially updated</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage PatchMessage(string body, long convoId, bool? isRead, long messageId, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            if (request.TryGetUserId(out userId, result))
            {
                _convoManager.TryPatchMessage(body, convoId, messageId, isRead, ref result, userId);
            }
            return GetResponse(request, result);
        }

        /// <summary>
        /// This function creates a new Convo
        /// </summary>
        /// <param name="participant">The other user who is participating in this Convo</param>
        /// <param name="request">The http request message that was made</param>
        /// <param name="subject">The subject of this Convo</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage PostConvo(long participant, HttpRequestMessage request, string subject)
        {
            var result = new Result();
            long userId;
            long id = 0;

            var success = (request.TryGetUserId(out userId, result) &&
                           _convoManager.TryInsertConvo(out id, participant, ref result, subject, userId));
            var response = GetResponse(request, result);
            if (success)
            {
                response.AddLocationHeader(string.Format(CONVO_RESOURCE_PATH, _baseUri, id));
            }
            return response;
        }

        /// <summary>
        /// This function creates a new Message within a Convo
        /// </summary>
        /// <param name="body">The body of the message</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="parent">The parent Message Id if this message is in reply to another message</param>
        /// <param name="recipient">The user who is the recipient of this Message</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        public HttpResponseMessage PostMessage(string body, long convoId, long? parent, long recipient, HttpRequestMessage request)
        {
            var result = new Result();
            long userId;
            long id = 0;
            var success = (request.TryGetUserId(out userId, result) &&
                           _convoManager.TryInsertMessage(body, convoId, out id, parent, recipient, ref result, userId));
            var response =  GetResponse(request, result);
            if (success)
            {
                response.AddLocationHeader(string.Format(MESSAGE_RESOURCE_PATH, _baseUri, convoId, id));
            }
            return response;
        }
        #endregion " Constructors and Public Methods "

        #region " Private Methods "
        private static HttpResponseMessage GetResponse(HttpRequestMessage request, Result result, object payLoad = null)
        {
            HttpResponseMessage response;
            if (payLoad != null)
            {
                response = request.CreateResponse(HttpStatusCodeMap.GetHttpStatusCode(result.ResultCode), payLoad);
            }
            else
            {
                response = request.CreateResponse();
                response.StatusCode = HttpStatusCodeMap.GetHttpStatusCode(result.ResultCode);
            }
            response.AddResultCodeHeader(result.ResultCode);
            response.AddMessageHeader(result.ToString());
            return response;
        }
        #endregion " Private Methods "
    }
}