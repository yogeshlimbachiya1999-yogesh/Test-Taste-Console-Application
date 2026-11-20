using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            var response = _httpClientService.Client
                .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                .Result;

            //If the status code isn't 200-299, then the function returns an empty collection.
            if (!response.IsSuccessStatusCode)
            {
                Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                return allPlanetsWithTheirMoons;
            }

            var content = response.Content.ReadAsStringAsync().Result;

            //The JSON converter uses DTO's, that can be found in the DataTransferObjects folder, to deserialize the response content.
            var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

            //The JSON converter can return a null object. 
            if (results == null) return allPlanetsWithTheirMoons;

            //If the planet doesn't have any moons, then it isn't added to the collection.
            foreach (var planet in results.Bodies)
            {
                if(planet.Moons != null)
                {
                    var newMoonsCollection = new Collection<MoonDto>();
                    foreach (var moon in planet.Moons)
                    {
                        var moonResponse = _httpClientService.Client
                            .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId)
                            .Result;
                        var moonContent = moonResponse.Content.ReadAsStringAsync().Result;
                        newMoonsCollection.Add(JsonConvert.DeserializeObject<MoonDto>(moonContent));
                    }
                    planet.Moons = newMoonsCollection;

                }
                allPlanetsWithTheirMoons.Add(new Planet(planet));
            }

            return allPlanetsWithTheirMoons;
        }
        //Get Counts of Moon and Temprature
        public async Task<List<ApiBody>> GetAllBodiesAsync(
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Loading Solar System data...");

            var response = _httpClientService.Client
               .GetAsync(UriPath.GetAllPlanetsWithMoonsCounts)
               .Result;

            string json = await response.Content.ReadAsStringAsync(
                cancellationToken);

            var result = System.Text.Json.JsonSerializer.Deserialize<ApiBodiesResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result?.Bodies == null)
            {
                throw new InvalidOperationException(
                    "Unable to read Solar System API response.");
            }
            return result.Bodies;
        }
        public List<PlanateCountDto> GetPlanetsWithMoonTemperatures(
            List<ApiBody> bodies)
        {
            Console.WriteLine(
                "Processing planets and their moons...");

            if (bodies == null || bodies.Count == 0)
            {
                return new List<PlanateCountDto>();
            }

            var bodyLookup = bodies.ToDictionary(
                x => x.Id,
                StringComparer.OrdinalIgnoreCase);

            var planets = bodies
                .Where(x =>
                    x.IsPlanet &&
                    x.Moons != null &&
                    x.Moons.Count > 0)
                .Select(body =>
                {
                    var planet = new PlanateCountDto
                    {
                        Id = body.Id,
                        Name = body.EnglishName,
                        AverageTemprature= Convert.ToString(body.AverageTemperature)
                    };

                    foreach (var moonReference in body.Moons!)
                    {
                        if (!bodyLookup.TryGetValue(
                            moonReference.MoonId,
                            out var moonBody))
                        {
                            continue;
                        }

                        if (!string.Equals(
                            moonBody.AroundPlanet?.PlanetId,
                            body.Id,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        planet.Moons.Add(new MoonCountDto
                        {
                            Id = moonBody.Id,
                            Name = moonBody.EnglishName,

                            AverageTemperature =
                                moonBody.AverageTemperature,

                            MassValue =
                                moonBody.Mass?.MassValue,

                            MassExponent =
                                moonBody.Mass?.MassExponent
                        });
                    }

                    return planet;
                })
                .Where(p => p.Moons.Count > 0)
                .ToList();

            Console.WriteLine(
                $"Found {planets.Count} planets with moons.");

            return planets;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
