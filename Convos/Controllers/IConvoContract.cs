using System.Net.Http;

namespace Convos.Controllers
{
    public interface IConvoContract
    {
        /// <summary>
        /// This function handles a web api request to delete a Convo
        /// </summary>
        /// <param name="convoId">The ID of the Convo</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage DeleteConvo(long convoId, HttpRequestMessage request);

        /// <summary>
        /// This function handles a web api request to delete a Message
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="messageId">The Id of the Message to delete</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage DeleteMessage(long convoId, long messageId, HttpRequestMessage request);

        /// <summary>
        /// This function gets a list of Convos
        /// </summary>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage GetConvo(HttpRequestMessage request);

        /// <summary>
        /// This function gets a specific Convo by Id
        /// </summary>
        /// <param name="convoId">The Id of the convo to get</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage GetConvo(long convoId, HttpRequestMessage request);

        /// <summary>
        /// This function gets all messages and their replies for a specific Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo from which to get the Messages</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage GetMessage(long convoId, HttpRequestMessage request);

        /// <summary>
        /// This function gets a specific Message within a Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo from which to get the Message</param>
        /// <param name="messageId">The Id of the Message to get</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage GetMessage(long convoId, long messageId, HttpRequestMessage request);

        /// <summary>
        /// This function partially updates a Convo
        /// </summary>
        /// <param name="convoId">The Id of the Convo to update</param>
        /// <param name="request">The http request message that was made</param>
        /// <param name="subject">The new subject of the Convo (if this value is null, it is ignored)</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage PatchConvo(long convoId, HttpRequestMessage request, string subject);

        /// <summary>
        /// This function partially updates a Message
        /// </summary>
        /// <param name="body">The new body of the message (if this value is null, it is ignored)</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="isRead">A new boolean value to indicate whether or not this message was read(if this value is null, it is ignored)</param>
        /// <param name="messageId">The Id of the Message to partially updated</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage PatchMessage(string body, long convoId, bool? isRead, long messageId, HttpRequestMessage request);

        /// <summary>
        /// This function creates a new Convo
        /// </summary>
        /// <param name="participant">The other user who is participating in this Convo</param>
        /// <param name="request">The http request message that was made</param>
        /// <param name="subject">The subject of this Convo</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage PostConvo(long participant, HttpRequestMessage request, string subject);

        /// <summary>
        /// This function creates a new Message within a Convo
        /// </summary>
        /// <param name="body">The body of the message</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="parent">The parent Message Id if this message is in reply to another message</param>
        /// <param name="recipient">The user who is the recipient of this Message</param>
        /// <param name="request">The http request message that was made</param>
        /// <returns>An HttpResponseMessage object to return to the client</returns>
        HttpResponseMessage PostMessage(string body, long convoId, long? parent, long recipient, HttpRequestMessage request);
    }
}