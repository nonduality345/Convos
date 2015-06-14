using System.Collections.Generic;
using System.Net;
using Convos.Enums;

namespace Convos.Utilities
{
    /// <summary>
    /// A helper class that maps custom ResultCodes to Http Status Codes
    /// </summary>
    public class HttpStatusCodeMap
    {
        #region " Members "
        private static readonly Dictionary<ResultCode, HttpStatusCode> _httpStatusCodeMap;
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Static constructor that initializes the dictionary that holds the mapping of result codes to http status codes
        /// </summary>
        static HttpStatusCodeMap()
        {
            _httpStatusCodeMap = new Dictionary<ResultCode, HttpStatusCode>();
            _httpStatusCodeMap.Add(ResultCode.OK, HttpStatusCode.OK);
            _httpStatusCodeMap.Add(ResultCode.INVALID_INPUT_DATA, HttpStatusCode.BadRequest);
            _httpStatusCodeMap.Add(ResultCode.ENTITY_NOT_FOUND, HttpStatusCode.NotFound);
            _httpStatusCodeMap.Add(ResultCode.NO_RESULTS, HttpStatusCode.NotFound);
            _httpStatusCodeMap.Add(ResultCode.UNAUTHORIZED, HttpStatusCode.Unauthorized);
            _httpStatusCodeMap.Add(ResultCode.CREATED, HttpStatusCode.Created);
            _httpStatusCodeMap.Add(ResultCode.DELETED, HttpStatusCode.NoContent);
            _httpStatusCodeMap.Add(ResultCode.UNKNOWN, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// This function returns an Http Status code that corresponds to the supplied ResultCode
        /// </summary>
        /// <param name="resultCode">The result code for which we are trying to get an Http Status Code</param>
        /// <returns>An HttpStatusCode that corresponds to the ResultCode</returns>
        public static HttpStatusCode GetHttpStatusCode(ResultCode resultCode)
        {
            HttpStatusCode statusCode;
            if (!_httpStatusCodeMap.TryGetValue(resultCode, out statusCode))
            {
                statusCode = HttpStatusCode.InternalServerError;
            }
            return statusCode;
        }
        #endregion " Constructors and Public Methods "
    }
}