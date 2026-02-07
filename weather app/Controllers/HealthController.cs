using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            return Ok(new
            {
                status = "ok",
                service = "Weather API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
