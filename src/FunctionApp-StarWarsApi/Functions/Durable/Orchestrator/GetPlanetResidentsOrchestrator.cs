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

            var planetName = context.GetInput<string>();

            var result = new PlanetResidents();

            var planetResult = await context.CallActivityAsync<Planet>(
                nameof(SearchPlanetActivity),
                input: planetName);

            if (planetResult != null)
            {
                result.PlanetName = planetResult.Name;

                var tasks = new List<Task<Person>>();
                foreach (var residentUrl in planetResult.ResidentUrls)
                {
                    tasks.Add(context.CallActivityAsync<Person>(nameof(GetCharacterActivity), residentUrl));
                }

                await Task.WhenAll(tasks);

                result.Residents = tasks.Select(task => task.Result).ToList<Person>();
            }

            return result;
        }
    }
}
