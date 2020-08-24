using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace college_interview_task_v4
{
    public abstract class HttpRequestHandler<TResponse>
    {

        private static readonly HttpClient _httpClient = new HttpClient();

        protected IHttpResponseParser<TResponse> _parser { get; }

        protected HttpRequestHandler(IHttpResponseParser<TResponse> parser)
        {
            _parser = parser;
        }

        public async Task<TResponse> Handle(IRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request.GetRequestMessage(), HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                return await _parser.ParseAsync(response);
            }
            catch (FormatException exception) 
            {
                throw new FormatException($"Could not parse HTTP response to format: {typeof(TResponse)}");
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}