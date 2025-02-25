using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        // Controller de Health Check
        [HttpGet("")]
        
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
