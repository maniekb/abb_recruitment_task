using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace college_interview_task_v4
{
    public class DefaultHttpRequestHandler<TResponse> : HttpRequestHandler<TResponse>
    {
        public DefaultHttpRequestHandler(IHttpResponseParser<TResponse> parser) : base(parser)
        {

        }

        public async Task<TResponse> ProcessRequest(string url, HttpMethod methodName, object payload = null,
            IDictionary<string, string> additionalHeaders = null, int allowedDelay = Timeout.Infinite)
        {
            var request = new HttpRequest(url, methodName, payload, additionalHeaders);

            var cts = new CancellationTokenSource(allowedDelay);

            try
            {
                return await Handle(request, cts.Token);
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException($"Response from {request.GetRequestMessage().RequestUri} exceded timeout of {allowedDelay} miliseconds.");
            }
        }
    }
}
