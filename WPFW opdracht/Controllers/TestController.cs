using Microsoft.AspNetCore.Mvc;

namespace WPFW_opdracht.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello, World!");
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id < 3)
            {
                return Ok($"dit moet goed gaan (ID: {id})");
            }
            else
            {

                return NotFound($"dit moet fout gaan (ID: {id})");
            }
        }
    }
}
