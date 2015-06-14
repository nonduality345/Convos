using System;
using System.Net.Http;

namespace Convos.Utilities
{
    /// <summary>
    /// A simple server cache class to illustrate the use of the architecture
    /// </summary>
    public class ServerCache : IServerCache
    {
        public HttpResponseMessage GetResponse(HttpRequestMessage request)
        {
            return null;
        }

        public void PutResponse(HttpRequestMessage request, HttpResponseMessage response, TimeSpan ts)
        {
            
        }

        public void RemoveResponse(HttpRequestMessage request)
        {
            
        }
    }
}