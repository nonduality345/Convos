using System.Linq;
using Newtonsoft.Json.Linq;

namespace Convos.Entities
{
    /// <summary>
    /// This class models the data required from the API to create a new Convo
    /// </summary>
    public class ConvoPostRequest
    {
        #region " Properties "
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

        #region " Constructors and Public Methods "
        /// <summary>
        /// This method tries to create a ConvoPostRequest object based on the de-serialized input
        /// </summary>
        /// <param name="json">The de-serialized input (defaults to null if the input json is invalid)</param>
        /// <param name="postRequest">The returned ConvoPostRequest</param>
        /// <returns>A boolean flag indicating whether or not the ConvoPostRequest was successfully created.</returns>
        public static bool TryCreate(JObject json, out ConvoPostRequest postRequest)
        {
            if (json != null)
            {
                postRequest = json.ToObject<ConvoPostRequest>();
                return json["Participant"] != null && json["Subject"] != null && json.Count == 2;
            }
            postRequest = null;
            return false;
        }

        #endregion " Constructors and Public Methods "
    }
}