using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/air-quality")]
    public class AirQualityController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AirQualityController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetAirQuality([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string? timezone)
        {
            timezone ??= "auto";

            var url = $"https://air-quality-api.open-meteo.com/v1/air-quality?" +
                      $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                      "&current=european_aqi,pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone,dust" +
                      "&hourly=pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<JsonElement>(response);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Problem($"Air quality service error: {ex.Message}");
            }
        }
    }
}
