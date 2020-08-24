using college_interview_task_v4;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class HttpRequestHandlerTests
    {
        public static HttpResponseToStringParser parser = new HttpResponseToStringParser();
        public static DefaultHttpRequestHandler<string> handler = new DefaultHttpRequestHandler<string>(parser);

        [Fact]
        public async Task request_to_not_existing_resource_should_throw_httprequestexception()
        {
            var url = "https://jsonplaceholder.typicode.com/thisresourcedoesnotexist";
            var method = HttpMethod.Get;

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await handler.ProcessRequest(url, method));
        }

        [Fact]
        public async Task request_with_1_milisecond_delay_allowed_should_throw_timeoutexception()
        {
            var url = "https://jsonplaceholder.typicode.com/posts/1";
            var method = HttpMethod.Get;

            await Assert.ThrowsAsync<TimeoutException>(async () => await handler.ProcessRequest(url, method, allowedDelay: 1));
        }

        [Fact]
        public async Task get_request_for_valid_url_should_return_resource_with_proper_id_as_string()
        {
            var url = "https://jsonplaceholder.typicode.com/posts/1";
            var method = HttpMethod.Get;

            var result = await handler.ProcessRequest(url, method);

            Assert.IsType<string>(result);
            Assert.Contains("\"id\": 1", result);
        }

        [Fact]
        public async Task post_request_for_valid_url_should_return_created_resource_as_string()
        {
            var url = "https://reqres.in/api/users";
            var method = HttpMethod.Post;
            var payload = "{\"name\": \"morpheus\",\"job\": \"leader\"}";

            var result = await handler.ProcessRequest(url, method, payload);

            // server should return id of newly created resource
            Assert.Contains("id", result);
        }
    }
}
