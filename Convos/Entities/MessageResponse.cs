using System;
using System.Collections.Generic;

namespace Convos.Entities
{
    /// <summary>
    /// A model class that represents a Message object in the application
    /// </summary>
    public class MessageResponse
    {
        #region " Properties "
        /// <summary>
        /// The body of the message
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// The Convo to which this message belongs
        /// </summary>
        public long ConvoId
        {
            get;
            set;
        }

        /// <summary>
        /// The date this message was created
        /// </summary>
        public DateTime DateCreated
        {
            get;
            set;
        }

        /// <summary>
        /// The date this message was updated
        /// </summary>
        public DateTime DateUpdated
        {
            get;
            set;
        }

        /// <summary>
        /// The Id of the message 
        /// </summary>
        public long Id
        {
            get; 
            set;
        }

        /// <summary>
        /// A boolean flag indicating whether or not this message was read
        /// </summary>
        public bool IsRead
        {
            get;
            set;
        }

        /// <summary>
        /// The Id of the message that this message is in reply to. May be null if this is the first message of
        /// the Convo.
        /// </summary>
        public long? Parent
        {
            get;
            set;
        }

        /// <summary>
        /// The id of the recipient
        /// </summary>
        public long Recipient
        {
            get;
            set;
        }

        /// <summary>
        /// A list of messages that represents replies to this message
        /// </summary>
        public List<MessageResponse> Thread
        {
            get; 
            set;
        }

        /// <summary>
        /// The id of the sender
        /// </summary>
        public long Sender
        {
            get;
            set;
        }
        #endregion " Properties "
    }
}