using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class ApiBody
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("englishName")]
        public string EnglishName { get; set; } = string.Empty;

        [JsonPropertyName("isPlanet")]
        public bool IsPlanet { get; set; }

        [JsonPropertyName("avgTemp")]
        public double? AverageTemperature { get; set; }

        [JsonPropertyName("moons")]
        public List<ApiMoonReference>? Moons { get; set; }

        [JsonPropertyName("aroundPlanet")]
        public ApiPlanetReference? AroundPlanet { get; set; }

        [JsonPropertyName("mass")]
        public ApiMass? Mass { get; set; }
    }
    public class ApiMoonReference
    {
        [JsonPropertyName("moon")]
        public string MoonId { get; set; } = string.Empty;
    }

    public class ApiPlanetReference
    {
        [JsonPropertyName("planet")]
        public string PlanetId { get; set; } = string.Empty;
    }

    public class ApiMass
    {
        [JsonPropertyName("massValue")]
        public double? MassValue { get; set; }

        [JsonPropertyName("massExponent")]
        public int? MassExponent { get; set; }
    }

    public class ApiBodiesResponse
    {
        [JsonPropertyName("bodies")]
        public List<ApiBody> Bodies { get; set; } = new();
    }
}
