using Newtonsoft.Json.Linq;

namespace Convos.Entities
{
    /// <summary>
    /// This class models the data required for a request to patch a Message
    /// </summary>
    public class MessagePatchRequest
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
        /// A boolean flag indicating whether or not the Message was read
        /// </summary>
        public bool? IsRead
        {
            get; 
            set;
        }
        #endregion " Properties "

        #region " Constructors and Public Methods "
        /// <summary>
        /// This method tries to create a ConvoPostRequest object based on the de-serialized input
        /// </summary>
        /// <param name="json">The de-serialized input (defaults to null if the input json is invalid)</param>
        /// <param name="postRequest">The returned ConvoPostRequest</param>
        /// <returns>A boolean flag indicating whether or not the ConvoPostRequest was successfully created.</returns>
        public static bool TryCreate(JObject json, out MessagePatchRequest postRequest)
        {
            if (json != null)
            {
                postRequest = json.ToObject<MessagePatchRequest>();
                return (json["Body"] != null || json["IsRead"] != null) && json.Count <= 2;
            }
            postRequest = null;
            return false;
        }
        #endregion " Constructors and Public Methods "
    }
}