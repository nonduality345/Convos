using Convos.Entities;
using Convos.Utilities;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Convos.Controllers
{
    /// <summary>
    /// This class is the controller class that handles all incoming requests for Convos through the API
    /// </summary>
    public class ConvoController : ApiController
    {
        #region " Members "
        /// <summary>
        /// A concrete instance of IConvoContract that interfaces with the manager to perform the desired action.
        /// </summary>
        private readonly IConvoContract _convoContract;
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Constructor in which a concrete instance of IConvoContract is injected
        /// </summary>
        /// <param name="convoContract">A concrete IConvoContract instance</param>
        public ConvoController(IConvoContract convoContract)
        {
            _convoContract = convoContract;
        }

        /// <summary>
        /// This method deletes the specified Convo
        /// </summary>
        /// <param name="convoId">The specified convo to delete</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("convo")]
        public virtual HttpResponseMessage DeleteConvo(long convoId)
        {
            return _convoContract.DeleteConvo(convoId, Request);
        }

        /// <summary>
        /// This method deletes the specified message
        /// </summary>
        /// <param name="convoId">The convo to which the message belongs</param>
        /// <param name="messageId">The id of the of message to delete</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("message")]
        public virtual HttpResponseMessage DeleteMessage(long convoId, int messageId)
        {
            return _convoContract.DeleteMessage(convoId, messageId, Request);
        }

        /// <summary>
        /// This method returns Convos to the client
        /// </summary>
        /// <param name="convoId">The specific convo to retrieve. If not specified, all convos are returned to the client</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("convo")]
        [Route("api/Convo")]
        public virtual HttpResponseMessage GetConvo()
        {
            return _convoContract.GetConvo(Request);
        }

        /// <summary>
        /// This method returns Convos to the client
        /// </summary>
        /// <param name="convoId">The specific convo to retrieve. If not specified, all convos are returned to the client</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("convo")]
        public virtual HttpResponseMessage GetConvo(long convoId)
        {
            return _convoContract.GetConvo(convoId, Request);
        }

        /// <summary>
        /// This method returns Messages to the client
        /// </summary>
        /// <param name="convoId">The Convo to which the message belongs</param>
        /// <param name="messageId">The Id of the message to retrieve. If not specified all messages for the specific convo are
        /// returned.</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("message")]
        public virtual HttpResponseMessage GetMessage(long convoId, long? messageId = null)
        {
            if (messageId == null)
            {
                return _convoContract.GetMessage(convoId, Request);
            }
            return _convoContract.GetMessage(convoId, (long)messageId, Request);
        }

        /// <summary>
        /// This method partially updates a Convo
        /// </summary>
        /// <param name="convoId">The specific Convo to partially update</param>
        /// <param name="json">The payload supplied by the client</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("convo")]
        public virtual HttpResponseMessage PatchConvo(long convoId, JObject json)
        {
            ConvoPatchRequest patchRequest;

            if (!ConvoPatchRequest.TryCreate(json, out patchRequest))
            {
                var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.AddMessageHeader("Invalid Convo Patch request");
                return response;
            }
            return _convoContract.PatchConvo(convoId, Request, patchRequest.Subject);
        }

        /// <summary>
        /// This method partially updates a Message
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which the message belongs</param>
        /// <param name="messageId">The Id of the Message to partially update</param>
        /// <param name="json">The payload supplied by the client</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("message")]
        public virtual HttpResponseMessage PatchMessage(long convoId, long messageId, JObject json)
        {
            MessagePatchRequest patchRequest;

            if (!MessagePatchRequest.TryCreate(json, out patchRequest))
            {
                var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.AddMessageHeader("Invalid Message Patch request");
                return response;
            }
            return _convoContract.PatchMessage(patchRequest.Body, convoId, patchRequest.IsRead, messageId, Request);
        }

        /// <summary>
        /// This method creates a new Convo
        /// </summary>
        /// <param name="json">The payload supplied by the client</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("convo")]
        [Route("api/Convo")]
        public virtual HttpResponseMessage PostConvo(JObject json)
        {
            ConvoPostRequest postRequest;
            
            if (!ConvoPostRequest.TryCreate(json, out postRequest))
            {
                var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.AddMessageHeader("Invalid Convo Post request");
                return response;
            }
            return _convoContract.PostConvo(postRequest.Participant, Request, postRequest.Subject);
        }

        /// <summary>
        /// This method creates a new Message
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="json">The payload supplied by the client</param>
        /// <param name="messageId">The message that this new message is in reply to. If this is not supplied, the message is posted just
        /// to the Convo</param>
        /// <returns>An HttpResponseMessage to send back to the client</returns>
        [ActionName("message")]
        public virtual HttpResponseMessage PostMessage([FromUri]long convoId, JObject json, [FromUri]long? messageId = null)
        {
            MessagePostRequest postRequest;

            if (!MessagePostRequest.TryCreate(json, out postRequest))
            {
                var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.AddMessageHeader("Invalid Message Post request");
                return response;
            }
            return _convoContract.PostMessage(postRequest.Body, convoId, messageId, postRequest.Recipient, Request);
        }

        #endregion " Constructors and Public Methods "
    }
}
