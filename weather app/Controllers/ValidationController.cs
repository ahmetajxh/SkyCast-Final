using Microsoft.AspNetCore.Mvc;
using WeatherApp.Utils;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/validate")]
    public class ValidationController : ControllerBase
    {
        [HttpGet("city")]
        public IActionResult ValidateCity([FromQuery] string name)
        {
            var valid = ValidationUtils.IsValidCityName(name);
            return Ok(new
            {
                valid = valid,
                error = valid ? null : "Invalid city name",
                input = name
            });
        }

        [HttpGet("coordinates")]
        public IActionResult ValidateCoordinates([FromQuery] string lat, [FromQuery] string lon)
        {
            bool valid = false;
            if (double.TryParse(lat, out double latitude) && double.TryParse(lon, out double longitude))
            {
                valid = ValidationUtils.IsValidCoordinates(latitude, longitude);
            }
            return Ok(new
            {
                valid = valid,
                error = valid ? null : "Invalid coordinates",
                latitude = lat,
                longitude = lon
            });
        }
    }
}
