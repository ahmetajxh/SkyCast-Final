using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeocodingController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GeocodingController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GeocodeCity([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                return BadRequest(new { error = "City name must be at least 2 characters" });
            }

            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(name)}&count=1&language=en&format=json";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<JsonElement>(response);

                if (!data.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                {
                    return NotFound(new { error = "City not found" });
                }

                var city = results[0];
                return Ok(new
                {
                    name = city.GetProperty("name").GetString(),
                    country = city.TryGetProperty("country", out var country) ? country.GetString() : null,
                    latitude = city.GetProperty("latitude").GetDouble(),
                    longitude = city.GetProperty("longitude").GetDouble(),
                    timezone = city.TryGetProperty("timezone", out var tz) ? tz.GetString() : null,
                    admin1 = city.TryGetProperty("admin1", out var admin1) ? admin1.GetString() : null,
                    population = city.TryGetProperty("population", out var pop) ? pop.GetInt32() : 0
                });
            }
            catch (Exception ex)
            {
                return Problem($"Geocoding service error: {ex.Message}");
            }
        }
    }
}
