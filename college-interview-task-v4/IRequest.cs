using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace college_interview_task_v4
{
    public interface IRequest
    {
        HttpRequestMessage GetRequestMessage();
    }
}
