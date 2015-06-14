using Newtonsoft.Json.Linq;

namespace Convos.Entities
{
    /// <summary>
    /// This class models the data required to create a new Message
    /// </summary>
    public class MessagePostRequest
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
        /// The id of the user who will be receiving this message
        /// </summary>
        public long Recipient
        {
            get;
            set;
        }
        #endregion " Properties "

        #region " Constructors and Public Methods "
        /// <summary>
        /// This method tries to create a MessagePostRequest object based on the de-serialized input
        /// </summary>
        /// <param name="json">The de-serialized input (defaults to null if the input json is invalid)</param>
        /// <param name="postRequest">The returned MessagePostRequest</param>
        /// <returns>A boolean flag indicating whether or not the ConvoPostRequest was successfully created.</returns>
        public static bool TryCreate(JObject json, out MessagePostRequest postRequest)
        {
            if (json != null)
            {
                postRequest = json.ToObject<MessagePostRequest>();
                return json["Body"] != null && json["Recipient"] != null && json.Count == 2;
            }
            postRequest = null;
            return false;
        }
        #endregion " Constructors and Public Methods "
    }
}