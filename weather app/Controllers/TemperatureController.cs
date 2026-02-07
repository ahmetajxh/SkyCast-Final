using Microsoft.AspNetCore.Mvc;
using WeatherApp.Utils;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/temperature")]
    public class TemperatureController : ControllerBase
    {
        [HttpGet("format")]
        public IActionResult FormatTemperature([FromQuery] double celsius, [FromQuery] string unit = "C")
        {
            var formatted = TemperatureUtils.FormatTemperature(celsius, unit);
            return Ok(new
            {
                input = celsius,
                unit = unit,
                formatted = formatted
            });
        }

        [HttpGet("convert/to-fahrenheit")]
        public IActionResult ConvertToFahrenheit([FromQuery] double celsius)
        {
            var fahrenheit = TemperatureUtils.CelsiusToFahrenheit(celsius);
            return Ok(new
            {
                celsius = celsius,
                fahrenheit = fahrenheit
            });
        }

        [HttpGet("convert/to-celsius")]
        public IActionResult ConvertToCelsius([FromQuery] double fahrenheit)
        {
            var celsius = TemperatureUtils.FahrenheitToCelsius(fahrenheit);
            return Ok(new
            {
                fahrenheit = fahrenheit,
                celsius = celsius
            });
        }
    }
}
