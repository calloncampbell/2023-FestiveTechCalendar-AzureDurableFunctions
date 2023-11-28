using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp_StarWarsApi.Functions.HttpTrigger
{
    public class Welcome
    {
        private readonly ILogger _logger;

        public Welcome(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Welcome>();
        }

        [Function(nameof(Welcome))]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Festive Tech Calendar 2023!");

            return response;
        }
    }
}
