using FunctionApp_StarWarsApi.Abstrations.Models;
using FunctionApp_StarWarsApi.Functions.Durable.Activity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;


namespace FunctionApp_StarWarsApi.Functions.Durable.Orchestrator
{
    public class GetPlanetResidentsOrchestrator
    {
        private readonly ILogger _logger;

        public GetPlanetResidentsOrchestrator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetPlanetResidentsOrchestrator>();
        }

        [Function(nameof(GetPlanetResidentsOrchestrator))]
        public async Task<PlanetResidents> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            if (!context.IsReplaying)
            {
                _logger.LogInformation($"{nameof(GetPlanetResidentsOrchestrator)} started for InstanceId: {context.InstanceId}");
            }

            var taskOptions = TaskOptions.FromRetryPolicy(
                new RetryPolicy(
                    maxNumberOfAttempts: 3,
                    firstRetryInterval: TimeSpan.FromSeconds(3),
                    backoffCoefficient: 1.5,
                    maxRetryInterval: TimeSpan.FromSeconds(30),
                    retryTimeout: TimeSpan.FromSeconds(180))
                );

            var planetName = context.GetInput<string>();

            var result = new PlanetResidents();

            var planetResult = await context.CallActivityAsync<Planet>(nameof(SearchPlanetActivity), input: planetName, taskOptions);

            if (planetResult != null)
            {
                result.PlanetName = planetResult.Name;

                var tasks = new List<Task<Person>>();
                foreach (var residentUrl in planetResult.ResidentUrls)
                {
                    tasks.Add(context.CallActivityAsync<Person>(nameof(GetCharacterActivity), residentUrl, taskOptions));
                }

                await Task.WhenAll(tasks);

                result.Residents = tasks.Select(task => task.Result).ToList<Person>();
            }

            context.SetCustomStatus("This is a custom status at end of orchestration.");

            return result;
        }
    }
}
