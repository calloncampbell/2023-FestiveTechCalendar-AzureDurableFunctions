using FunctionApp_HelloDurable.Functions.Durable.Activity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;


namespace FunctionApp_HelloDurable.Functions.Durable.Orchestrator
{
    public class OrchestratorFunction
    {
        private readonly ILogger _logger;

        public OrchestratorFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrchestratorFunction>();
        }

        [Function(nameof(OrchestratorFunction))]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            _logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Dasher"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Dancer"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Prancer"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Vixen"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Comet"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Cupid"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Donner"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Blitzen"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(ActivityFunction), "Rudolph"));

            return outputs;
        }
    }
}
