using System;
using Convos.Enums;
using System.Net.Http;

namespace Convos.Utilities
{
    /// <summary>
    /// System.Net.Http HttpResponseMessage extension methods
    /// </summary>
    public static class ResponseExtentions
    {
        #region " Members "
        // Instead of being constants, these could alternatively be configurable if you needed that flexibility
        private const string LAST_MODIFIED_HEADER = "X-Last-Modified";
        private const string LOCATION_HEADER = "X-URI-Reference";
        private const string MESSAGE_HEADER = "X-Message";
        private const string NEXT_PAGE_HEADER = "X-URI-Next-Page";
        private const string TOTAL_COUNT_HEADER = "X-Total-Count";
        private const string RELATIVE_COUNT_HEADER = "X-Response-Count";
        private const string RESULT_CODE_HEADER = "X-Result-Code";
        #endregion " Members "

        #region " Constructors and Public Methods "

        /// <summary>
        /// Inserts custom HTTP response last modified header
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="lastModified">The date the resource was last modified</param>
        public static void AddLastModifiedHeader(this HttpResponseMessage response, DateTime lastModified)
        {
            response.Headers.Add(LAST_MODIFIED_HEADER, lastModified.ToString("s"));
        }

        /// <summary>
        /// Inserts custom HTTP response location header
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="location">The value of the resource's location</param>
        public static void AddLocationHeader(this HttpResponseMessage response, string location)
        {
            response.Headers.Add(LOCATION_HEADER, location);
        }

        /// <summary>
        /// Inserts custom HTTP response message header into HttpResponseMessage class
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="message">message to be inserted into the header</param>
        public static void AddMessageHeader(this HttpResponseMessage response, string message)
        {
            response.Headers.Add(MESSAGE_HEADER, message);
        }

        /// <summary>
        /// Inserts custom HTTP response next page header into HttpResponseMessage class
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="nextPageUri">The URI to the next page of results</param>
        public static void AddNextPageHeader(this HttpResponseMessage response, string nextPageUri)
        {
            response.Headers.Add(NEXT_PAGE_HEADER, nextPageUri);
        }

        /// <summary>
        /// Inserts custom HTTP collection relative count header into HttpResponseMessage class
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="relativeCount">number of elements in the response</param>
        public static void AddRelativeCountHeader(this HttpResponseMessage response, int relativeCount)
        {
            response.Headers.Add(RELATIVE_COUNT_HEADER, relativeCount.ToString());
        }

        /// <summary>
        /// Inserts custom HTTP response result code header into HttpResponseMessage class
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="resultCode">result code to be inserted into the header</param>
        public static void AddResultCodeHeader(this HttpResponseMessage response, ResultCode resultCode)
        {
            response.Headers.Add(RESULT_CODE_HEADER, ((int)resultCode).ToString());
        }

        /// <summary>
        /// Inserts custom HTTP collection total count header into HttpResponseMessage class
        /// </summary>
        /// <param name="response">System.Net.Http HttpResponseMessage object</param>
        /// <param name="total">total number of elements in the collection</param>
        public static void AddTotalCountHeader(this HttpResponseMessage response, int total)
        {
            response.Headers.Add(TOTAL_COUNT_HEADER, total.ToString());
        }

        #endregion " Constructors and Public Methods "
    }
}