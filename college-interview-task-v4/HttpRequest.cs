using System;
using System.Collections.Generic;
using System.Net.Http;

namespace college_interview_task_v4
{
    public class HttpRequest : IRequest
    {
        private HttpRequestMessage HttpRequestMessage { get; set; }
        public HttpRequest(string uri, HttpMethod methodName, object payload = null,
            IDictionary<string, string> additionalHeaders = null)
        {
            HttpRequestMessage = new HttpRequestMessage();

            HttpRequestMessage.RequestUri = HttpRequestMessage.RequestUri = new Uri(uri);
            HttpRequestMessage.Method = methodName;

            if (payload != null)
            {
                HttpRequestMessage.Content = payload as HttpContent;
            }

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    HttpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }
        }

        public HttpRequestMessage GetRequestMessage()
            => HttpRequestMessage;
    }
}
