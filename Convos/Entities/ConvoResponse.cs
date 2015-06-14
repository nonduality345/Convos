using System;

namespace Convos.Entities
{
    /// <summary>
    /// A model class that represents a Convo object in the application
    /// </summary>
    public class ConvoResponse
    {
        #region " Properties "
        /// <summary>
        /// The Id of the user who created this Convo
        /// </summary>
        public long Creator
        {
            get; 
            set;
        }

        /// <summary>
        /// The date this Convo was created
        /// </summary>
        public DateTime DateCreated
        {
            get; 
            set;
        }

        /// <summary>
        /// The date at which the last message in the convo was added
        /// </summary>
        public DateTime? DateOfLastMessage
        {
            get; 
            set;
        }

        /// <summary>
        /// The date this Convo was updated
        /// </summary>
        public DateTime DateUpdated
        {
            get;
            set;
        }
        
        /// <summary>
        /// The Id of this Convo
        /// </summary>
        public long Id
        {
            get; 
            set;
        }

        /// <summary>
        /// The number of messages that are in this Convo
        /// </summary>
        public int NumMessages
        {
            get; 
            set;
        }
        /// <summary>
        /// The other user who is participating in this Convo
        /// </summary>
        public long Participant
        {
            get;
            set;
        }

        /// <summary>
        /// The subject of the Convo
        /// </summary>
        public string Subject
        {
            get; 
            set;
        }
        #endregion " Properties "
    }
}
