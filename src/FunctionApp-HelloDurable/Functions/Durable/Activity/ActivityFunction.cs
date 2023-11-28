using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp_HelloDurable.Functions.Durable.Activity
{
    public class ActivityFunction
    {
        private readonly ILogger _logger;

        public ActivityFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ActivityFunction>();
        }

        [Function(nameof(ActivityFunction))]
        public string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            _logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }
    }
}
