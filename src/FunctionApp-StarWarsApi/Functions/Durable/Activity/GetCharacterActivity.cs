using FunctionApp_StarWarsApi.Abstrations.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace FunctionApp_StarWarsApi.Functions.Durable.Activity
{
    public class GetCharacterActivity
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetCharacterActivity(
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<GetCharacterActivity>();
            _httpClientFactory = httpClientFactory;
        }

        [Function(nameof(GetCharacterActivity))]
        public async Task<Person> RunAsync([ActivityTrigger] string characterUri)
        {
            characterUri = characterUri.Trim('"');
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, characterUri);

            var client = _httpClientFactory.CreateClient();
            var result = await client.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var characterContent = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Person>(characterContent);
        }
    }
}
