using Microsoft.AspNetCore.Mvc;
using WeatherApp.Utils;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/aqi")]
    public class AqiController : ControllerBase
    {
        [HttpGet("categorize")]
        public IActionResult CategorizeAqi([FromQuery] int index)
        {
            var category = AQIUtils.CategorizeAQI(index);
            var color = AQIUtils.GetAQIColor(index);
            return Ok(new
            {
                index = index,
                category = category,
                color = color
            });
        }
    }
}
