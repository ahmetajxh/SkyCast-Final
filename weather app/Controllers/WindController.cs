using Microsoft.AspNetCore.Mvc;
using WeatherApp.Utils;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/wind")]
    public class WindController : ControllerBase
    {
        [HttpGet("direction")]
        public IActionResult GetWindDirection([FromQuery] double degrees)
        {
            var direction = WindUtils.GetWindDirection(degrees);
            return Ok(new
            {
                degrees = degrees,
                direction = direction
            });
        }
    }
}
