using Convos.Enums;
using System.Collections.Generic;

namespace Convos.Results
{
    /// <summary>
    /// This is a simple data transfer object that holds information related to the result of an operation
    /// </summary>
    public class Result
    {
        #region " Members "
        /// <summary>
        /// A list of messages that will be communicated to the caller
        /// </summary>
        private List<string> _message;
        #endregion " Members "

        #region " Properties "
        /// <summary>
        /// A ResultCode that indicates the result of the operation
        /// </summary>
        public ResultCode ResultCode
        {
            get; 
            set;
        }
        #endregion " Properties "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Constructor that initializes the _message list
        /// </summary>
        public Result()
        {
            _message = new List<string>();
        }

        /// <summary>
        /// This method adds a message to the list of messages
        /// </summary>
        /// <param name="s">The message to add</param>
        public void AddMessage(string s)
        {
            _message.Add(s);
        }

        /// <summary>
        /// This method writes all of the messages in the _message list out to a semicolon delimited string
        /// </summary>
        /// <returns>A semicolon delimited string of messages</returns>
        public string ToString()
        {
            return string.Join("; ", _message.ToArray());
        }

        #endregion " Constructors and Public Methods "
    }
}
