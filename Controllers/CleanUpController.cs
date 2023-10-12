using FlightPlanner.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanUpController : ControllerBase
    {
        private readonly FlightStorage _storage;
        public CleanUpController()
        {
            _storage = new FlightStorage();
        }

        [Route("clear")]
        [HttpPost]
        public IActionResult Clear()
        {
            _storage.Clear();
            return Ok();
        }
    }
}