using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompleteWeather([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city) || city.Length < 2)
            {
                return BadRequest(new { error = "City name must be at least 2 characters" });
            }

            try
            {
                // Step 1: Geocode
                var geocodeUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json";
                var geocodeResponse = await _httpClient.GetStringAsync(geocodeUrl);
                var geocodeData = JsonSerializer.Deserialize<JsonElement>(geocodeResponse);

                if (!geocodeData.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                {
                    return NotFound(new { error = "City not found" });
                }

                var cityData = results[0];
                var lat = cityData.GetProperty("latitude").GetDouble();
                var lon = cityData.GetProperty("longitude").GetDouble();
                var timezone = cityData.TryGetProperty("timezone", out var tz) ? tz.GetString() : "auto";

                // Step 2: Get forecast
                var forecastUrl = $"https://api.open-meteo.com/v1/forecast?" +
                                 $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                                 "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m,precipitation,pressure_msl,surface_pressure,visibility,uv_index,is_day" +
                                 "&hourly=temperature_2m,weather_code,precipitation_probability,precipitation,visibility,uv_index" +
                                 "&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_sum,precipitation_probability_max,wind_speed_10m_max,uv_index_max";

                var forecastResponse = await _httpClient.GetStringAsync(forecastUrl);
                var forecastData = JsonSerializer.Deserialize<JsonElement>(forecastResponse);

                // Step 3: Get air quality
                var aqiUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?" +
                            $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                            "&current=european_aqi,pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone,dust" +
                            "&hourly=pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone";

                var aqiResponse = await _httpClient.GetStringAsync(aqiUrl);
                var aqiData = JsonSerializer.Deserialize<JsonElement>(aqiResponse);

                // Combine all data
                return Ok(new
                {
                    city = new
                    {
                        name = cityData.GetProperty("name").GetString(),
                        country = cityData.TryGetProperty("country", out var country) ? country.GetString() : null,
                        latitude = lat,
                        longitude = lon,
                        timezone = timezone
                    },
                    current = forecastData.GetProperty("current"),
                    hourly = forecastData.GetProperty("hourly"),
                    daily = forecastData.GetProperty("daily"),
                    current_units = forecastData.GetProperty("current_units"),
                    hourly_units = forecastData.GetProperty("hourly_units"),
                    daily_units = forecastData.GetProperty("daily_units"),
                    air_quality = aqiData.GetProperty("current"),
                    air_quality_units = aqiData.GetProperty("current_units")
                });
            }
            catch (Exception ex)
            {
                return Problem($"Weather service error: {ex.Message}");
            }
        }
    }
}
