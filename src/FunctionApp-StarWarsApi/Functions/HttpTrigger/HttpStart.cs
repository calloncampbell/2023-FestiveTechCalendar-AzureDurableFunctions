using System.Net;
using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace FunctionApp_StarWarsApi.Functions.Http
{
    public class HttpStart
    {
        private readonly ILogger _logger;

        public HttpStart(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpStart>();
        }

        [Function(nameof(HttpStart))]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "start/{orchestratorName}/")] HttpRequestData req,
            [DurableClient(TaskHub = "%DurableTaskHubName%")] DurableTaskClient client,
            string orchestratorName)
        {            
            string orchestratorInput = string.Empty;
            var streamReader = new StreamReader(req.Body);
            orchestratorInput = await streamReader.ReadToEndAsync();           
            
            string instanceId = Guid.NewGuid().ToString();
            await client.ScheduleNewOrchestrationInstanceAsync(orchestratorName, input: orchestratorInput, options: new Microsoft.DurableTask.StartOrchestrationOptions(InstanceId: instanceId));

            var taskHubName = Environment.GetEnvironmentVariable("DurableTaskHubName", EnvironmentVariableTarget.Process);

            _logger.LogInformation("Created new orchestration with instance ID = {instanceId} on task hub {taskHubName}.", instanceId, taskHubName);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
