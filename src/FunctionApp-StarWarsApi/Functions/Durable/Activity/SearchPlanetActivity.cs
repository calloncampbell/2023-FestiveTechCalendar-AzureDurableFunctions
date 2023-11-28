using FunctionApp_StarWarsApi.Abstrations.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace FunctionApp_StarWarsApi.Functions.Durable.Activity
{
    public class SearchPlanetActivity
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public SearchPlanetActivity(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<SearchPlanetActivity>();
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [Function(nameof(SearchPlanetActivity))]
        public async Task<Planet> RunAsync([ActivityTrigger] string name)
        {
            name = name.Trim('"');
            var uri = $"{_configuration["SwapiBaseUrl"]}planets?search={name}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            var client = _httpClientFactory.CreateClient();
            var result = await client.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var planetContent = await result.Content.ReadAsStringAsync();
            var planets = JToken.Parse(planetContent).SelectToken("results").ToObject<Planet[]>();

            return planets.FirstOrDefault();
        }
    }
}
