using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ForecastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetForecast([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string? timezone)
        {
            timezone ??= "auto";

            var url = $"https://api.open-meteo.com/v1/forecast?" +
                      $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                      "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m,precipitation,pressure_msl,surface_pressure,visibility,uv_index,is_day" +
                      "&hourly=temperature_2m,weather_code,precipitation_probability,precipitation,visibility,uv_index" +
                      "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max,wind_speed_10m_max,uv_index_max";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<JsonElement>(response);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Problem($"Forecast service error: {ex.Message}");
            }
        }
    }
}
