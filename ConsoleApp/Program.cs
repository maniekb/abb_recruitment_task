using college_interview_task_v4;
using System;
using System.Net.Http;

namespace ConsoleApp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {


            HttpResponseToStringParser parser = new HttpResponseToStringParser();
            DefaultHttpRequestHandler<string> handler = new DefaultHttpRequestHandler<string>(parser);


            // GET method example
            Console.WriteLine("GET method example:");
            var url1 = "https://jsonplaceholder.typicode.com/posts/1";

            Console.WriteLine($"Sending GET request to {url1}.");
            var result1 = await handler.ProcessRequest(url1, HttpMethod.Get);
            Console.WriteLine($"Response: {result1}");


            // POST  method example
            Console.WriteLine("\n\nPOST  method example:");
            var url2 = "https://reqres.in/api/users";
            var payload = "{\"name\": \"morpheus\",\"job\": \"leader\"}";

            Console.WriteLine($"Sending POST request to {url2} with body: {payload}.");
            var result2 = await handler.ProcessRequest(url2, HttpMethod.Post, payload);
            Console.WriteLine($"Response: {result2}");

            // GET method example with cancellation token
            Console.WriteLine("\n\nGET method example with cancellation token:");
            var url3 = "https://reqres.in/api/users/2";
            var delay = 10;

            Console.WriteLine($"Sending GET request to {url3} with allowed delay of {delay} miliseconds.");
            try
            {
                var result3 = await handler.ProcessRequest(url3, HttpMethod.Get, allowedDelay: delay);
                Console.WriteLine($"Response: {result3}");
            }
            catch(TimeoutException exception)
            {
                Console.WriteLine($"{url3} did not response in {delay} miliseconds. Request cancelled.");
            }


            // GET method example with cancellation token
            Console.WriteLine("\n\nGET method example to not existing resource - exception with 404 status code is returned:");
            var url4 = "https://jsonplaceholder.typicode.com/thisresourcedoesnotexist";

            Console.WriteLine($"Sending GET request to: {url4}");
            try
            {
                await handler.ProcessRequest(url4, HttpMethod.Get);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine($"{exception.Message}");
            }
        }
    }
}
