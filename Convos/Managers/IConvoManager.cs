using System;
using Convos.Entities;
using Convos.Results;

namespace Convos.Managers
{
    /// <summary>
    /// The interface that describes the actions that can be performed on Convos and their messages
    /// </summary>
    public interface IConvoManager
    {
        /// <summary>
        /// This method deletes a Convo. Once a Convo is deleted, all of its messages are no longer accessible by anyone participating
        /// in the Convo.
        /// </summary>
        /// <param name="id">The Id of the Convo to delete.</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryDeleteConvo(long id, ref Result result, long userId);

        /// <summary>
        /// This method deletes a Message. Once a message is deleted, it is no longer accessible by the sender or the receiver of the
        /// Message.
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="id">The Id of the Message to delete</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryDeleteMessage(long convoId, long id, ref Result result, long userId);

        /// <summary>
        /// This method retrieves Convos from the database using paging.
        /// </summary>
        /// <param name="before">The time from which to retrieve results</param>
        /// <param name="count">The number of convos on the page to retrieve</param>
        /// <param name="index">The index of the page of convos. This is zero based.</param>
        /// <param name="maxCreated">The date of the latest created Convo before the "before" time</param>
        /// <param name="response">The returned convos from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="total">The total number of convos</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryGetAllConvos(DateTime before, int count, int index, out DateTime maxCreated, out ConvoResponse[] response, ref Result result, out int total, long userId);

        /// <summary>
        /// This method retrieves Messages from the database
        /// </summary>
        /// <param name="before">The time from which to retrieve results</param>
        /// <param name="convoId">The Id of the Convo from which to get the messages</param>
        /// <param name="count">The number of messages on the page to retrieve</param>
        /// <param name="index">The index of the page of Messages. This is zero based.</param>
        /// <param name="maxCreated">The date of the last Message before the "before" time</param>
        /// <param name="responses">The returned messages from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="total">The total number of messages</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryGetAllMessages(DateTime before, long convoId, int count, int index, out DateTime maxCreated, out MessageResponse[] responses, ref Result result, out int total, long userId);

        /// <summary>
        /// This method retrieves a specific Convo based on its Id
        /// </summary>
        /// <param name="id">The Id of the Convo</param>
        /// <param name="response">The returned Convo from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryGetConvoById(long id, out ConvoResponse response, ref Result result, long userId);

        /// <summary>
        /// This method retrieves a specific Message based on its Id
        /// </summary>
        /// <param name="convoId">The Convo Id that this message belongs to</param>
        /// <param name="id">The Id of the Message</param>
        /// <param name="response">The returned Message from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryGetMessageById(long convoId, long id, out MessageResponse response, ref Result result, long userId);

        /// <summary>
        /// This method inserts a new Convo into the database
        /// </summary>
        /// <param name="id">The returned Id of the newly created Convo</param>
        /// <param name="participant">The Id of the user who is also participating in this Convo</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="subject">The subject of this Convo</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryInsertConvo(out long id, long participant, ref Result result, string subject, long userId);

        /// <summary>
        /// This method inserts a new Message into the database
        /// </summary>
        /// <param name="body">The body of the message</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="id">The returned Id of the newly created Message</param>
        /// <param name="parent">If this Message is a reply to another Message, the id of that Message should be specified as
        /// the parent.</param>
        /// <param name="recipient">The user id of the recipient of this message</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryInsertMessage(string body, long convoId, out long id, long? parent, long recipient, ref Result result, long userId);

        /// <summary>
        /// This method partially updates a Convo for only the allowable fields. The only field that can be updated is the subject.
        /// </summary>
        /// <param name="id">The Id of the Convo to partially update</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="subject">The new value for the subject</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryPatchConvo(long id, ref Result result, string subject, long userId);

        /// <summary>
        /// This method partially updates a Message for only the allowable fields. The only fields that can be updated are the Body and IsRead.
        /// </summary>
        /// <param name="body">The new value for the body (if it is null, it is ignored)</param>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="id">The Id of the message to partially update</param>
        /// <param name="isRead">The new value for the IsRead field (if this is null, it is ignored)</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        bool TryPatchMessage(string body, long convoId, long id, bool? isRead, ref Result result, long userId);
    }
}
