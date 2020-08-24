using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace college_interview_task_v4
{
    public class HttpResponseToStringParser : IHttpResponseParser<string>
    {
        public Task<string> ParseAsync(HttpResponseMessage response)
        {
            if(response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Content.ReadAsStringAsync();
        }
    }
}
