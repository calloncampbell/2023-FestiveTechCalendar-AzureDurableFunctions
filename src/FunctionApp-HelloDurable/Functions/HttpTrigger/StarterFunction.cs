using System.Net;
using FunctionApp_HelloDurable.Functions.Durable.Orchestrator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FunctionApp_HelloDurable.Functions.HttpTrigger
{
    public class StarterFunction
    {
        private readonly ILogger _logger;

        public StarterFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<StarterFunction>();
        }

        [Function(nameof(StarterFunction))]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient(TaskHub = "%DurableTaskHubName%")] DurableTaskClient client,
            FunctionContext executionContext)
        {
            var taskHubName = Environment.GetEnvironmentVariable("DurableTaskHubName", EnvironmentVariableTarget.Process);
            _logger.LogInformation("StarterFunction triggered on DurableTaskHub: {taskHubName}", taskHubName);

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(OrchestratorFunction));

            _logger.LogInformation("Started orchestration with ID = '{instanceId}' on task hub '{taskHubName}'.", instanceId, taskHubName);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
