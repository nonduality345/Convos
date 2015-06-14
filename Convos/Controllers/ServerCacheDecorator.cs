using Convos.Utilities;
using System;
using System.Net.Http;

namespace Convos.Controllers
{
    /// <summary>
    /// A crude and simple class to illustrate how to extend the architecture
    /// </summary>
    public class ServerCacheDecorator : IConvoContract
    {
        #region " Members "
        private readonly IConvoContract _convoContract;
        private readonly IServerCache _serverCache;
        #endregion " Members "

        #region " Constructors and Public Methods "
        public ServerCacheDecorator(IConvoContract convoContract, IServerCache serverCache)
        {
            _convoContract = convoContract;
            _serverCache = serverCache;
        }

        public HttpResponseMessage DeleteConvo(long convoId, HttpRequestMessage request)
        {
            var response = _convoContract.DeleteConvo(convoId, request);
            if (response.IsSuccessStatusCode)
            {
                _serverCache.RemoveResponse(request);
            }
            return response;
        }

        public HttpResponseMessage DeleteMessage(long convoId, long messageId, HttpRequestMessage request)
        {
            var response = _convoContract.DeleteMessage(convoId, messageId, request);
            if (response.IsSuccessStatusCode)
            {
                _serverCache.RemoveResponse(request);
            }
            return response;
        }

        public HttpResponseMessage GetConvo(HttpRequestMessage request)
        {
            var response = _serverCache.GetResponse(request);
            if (response == null)
            {
                response = _convoContract.GetConvo(request);
                _serverCache.PutResponse(request, response, TimeSpan.FromHours(1));
            }
            return response;
        }

        public HttpResponseMessage GetConvo(long convoId, HttpRequestMessage request)
        {
            var response = _serverCache.GetResponse(request);
            if (response == null)
            {
                response = _convoContract.GetConvo(convoId, request);
                _serverCache.PutResponse(request, response, TimeSpan.FromHours(1));
            }
            return response;
        }

        public HttpResponseMessage GetMessage(long convoId, HttpRequestMessage request)
        {
            var response = _serverCache.GetResponse(request);
            if (response == null)
            {
                response = _convoContract.GetMessage(convoId, request);
                _serverCache.PutResponse(request, response, TimeSpan.FromHours(1));
            }
            return response;
        }

        public HttpResponseMessage GetMessage(long convoId, long messageId, HttpRequestMessage request)
        {
            var response = _serverCache.GetResponse(request);
            if (response == null)
            {
                response = _convoContract.GetMessage(convoId, messageId, request);
                _serverCache.PutResponse(request, response, TimeSpan.FromHours(1));
            }
            return response;
        }

        public HttpResponseMessage PatchConvo(long convoId, HttpRequestMessage request, string subject)
        {
            var response = _convoContract.PatchConvo(convoId, request, subject);
            if (response.IsSuccessStatusCode)
            {
                _serverCache.RemoveResponse(request);
            }
            return response;
        }

        public HttpResponseMessage PatchMessage(string body, long convoId, bool? isRead, long messageId, HttpRequestMessage request)
        {
            var response = _convoContract.PatchMessage(body, convoId, isRead, messageId, request);
            if (response.IsSuccessStatusCode)
            {
                _serverCache.RemoveResponse(request);
            }
            return response;
        }

        public HttpResponseMessage PostConvo(long participant, HttpRequestMessage request, string subject)
        {
            return _convoContract.PostConvo(participant, request, subject);
        }

        public HttpResponseMessage PostMessage(string body, long convoId, long? parent, long recipient, HttpRequestMessage request)
        {
            return _convoContract.PostMessage(body, convoId, parent, recipient, request);
        }
        #endregion " Constructors and Public Methods "
    }
}