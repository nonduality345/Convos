using System;
using System.Linq;
using System.Net.Http;
using System.Configuration;
using Convos.Enums;
using Convos.Results;

namespace Convos.Utilities
{
    /// <summary>
    /// A helper class that adds additional functions to the HttpRequestMessage object
    /// </summary>
    public static class RequestExtensions
    {
        #region " Members "
        /// <summary>
        /// The name of the authorization header that is expected in the request
        /// </summary>
        private const string AUTHORIZATION_HEADER = "X-Authorization";

        // Default values used in the methods below
        private static readonly int _defaultPageSize = 10;
        private static readonly int _defaultIndex;
        private const int _defaultUserId = 0;
        private static readonly int _defaultMaxPageCount = 50;
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Static constructor that initializes values from the configuration file
        /// </summary>
        static RequestExtensions()
        {
            int defaultPageSize;
            if (int.TryParse(ConfigurationManager.AppSettings["DefaultPageSize"], out defaultPageSize))
            {
                _defaultPageSize = defaultPageSize;
            }
            int defaultPageIndex;
            if (int.TryParse(ConfigurationManager.AppSettings["DefaultPageIndex"], out defaultPageIndex))
            {
                _defaultIndex = defaultPageIndex;
            }
            int defaultMaxPageSize;
            if (int.TryParse(ConfigurationManager.AppSettings["DefaultMaxPageCount"], out defaultMaxPageSize))
            {
                _defaultMaxPageCount = defaultMaxPageSize;
            }
        }

        /// <summary>
        /// This method retrieves the page index from the query string of the request
        /// </summary>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>An integer representing the page index</returns>
        public static int GetPageIndex(this HttpRequestMessage request, Result result)
        {
            var queryParams = request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            int index = _defaultIndex;
            if (queryParams.ContainsKey("index"))
            {
                int.TryParse(queryParams["index"], out index); 
            }
            return index;
        }

        /// <summary>
        /// This function retrieves the page size from the request. If the page size is not found, the default page size is used.
        /// If the page size is found, but it is beyond the maximum allowed, then the function returns false and the Result object
        /// is set accordingly.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="count">The returned value for the page size</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public static bool TryGetPageCount(this HttpRequestMessage request, out int count, Result result)
        {
            var queryParams = request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            count = _defaultPageSize;
            if (queryParams.ContainsKey("count") &&
                int.TryParse(queryParams["count"], out count) &&
                count > _defaultMaxPageCount)
            {
                result.AddMessage(string.Format("Maximum page size is {0}", _defaultMaxPageCount));
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function retrieves the page size from the request. If the page size is not found, the default page size is used.
        /// If the page size is found, but it is beyond the maximum allowed, then the function returns false and the Result object
        /// is set accordingly.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="count">The returned value for the page size</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public static bool TryGetPagingTimestamp(this HttpRequestMessage request, out DateTime before, Result result)
        {
            var queryParams = request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            before = DateTime.MaxValue;
            if (queryParams.ContainsKey("before") &&
                !DateTime.TryParse(queryParams["before"], out before))
            {
                result.AddMessage("Could not read a valid date for the 'before' querystring parameter");
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function tries to get the user's id from the request headers. If the function fails to get a user's id, the
        /// Result object is set appropriately and the function returns false.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="userId">The returned user id</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public static bool TryGetUserId(this HttpRequestMessage request, out long userId, Result result)
        {
            userId = _defaultUserId;
            if (!request.Headers.Contains(AUTHORIZATION_HEADER) ||
                !long.TryParse(request.Headers.GetValues(AUTHORIZATION_HEADER).FirstOrDefault(), out userId))
            {
                result.AddMessage("Could not parse a valid user id");
                result.ResultCode = ResultCode.UNAUTHORIZED;
                return false;
            }
            return true;
        }
        #endregion " Constructors and Public Methods "
    }
}