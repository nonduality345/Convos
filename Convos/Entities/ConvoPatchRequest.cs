using Newtonsoft.Json.Linq;

namespace Convos.Entities
{
    /// <summary>
    /// This class models the data required for a Patch request
    /// </summary>
    public class ConvoPatchRequest
    {
        #region " Properties "
        /// <summary>
        /// For Convos, currently, only the subject is allowed to be patched
        /// </summary>
        public string Subject
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
        public static bool TryCreate(JObject json, out ConvoPatchRequest postRequest)
        {
            if (json != null)
            {
                postRequest = json.ToObject<ConvoPatchRequest>();
                return json["Subject"] != null && json.Count == 1;
            }
            postRequest = null;
            return false;
        }
        #endregion " Constructors and Public Methods "
    }
}