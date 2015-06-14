using System;
using System.Net.Http;

namespace Convos.Utilities
{
    /// <summary>
    /// A simple server side cache interface to illustrate the use of the architecture
    /// </summary>
    public interface IServerCache
    {
        HttpResponseMessage GetResponse(HttpRequestMessage request);
        void PutResponse(HttpRequestMessage request, HttpResponseMessage response, TimeSpan ts);
        void RemoveResponse(HttpRequestMessage request);
    }
}
