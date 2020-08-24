using System.Net.Http;
using System.Threading.Tasks;

namespace college_interview_task_v4
{
    public interface IHttpResponseParser<TResult>
    {
        Task<TResult> ParseAsync(HttpResponseMessage response);
    }
}
