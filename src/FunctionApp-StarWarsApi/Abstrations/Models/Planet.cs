using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace FunctionApp_StarWarsApi.Abstrations.Models
{
    public sealed class Planet
    {
        public string Name { get; set; }

        [JsonProperty("residents")]
        public string[] ResidentUrls { get; set; }
    }
}
