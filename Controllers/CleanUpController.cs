using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanUpController : ControllerBase
    {
        private readonly ICleanUpService _cleanUpService;
        public CleanUpController(ICleanUpService cleanUpService)
        {
            _cleanUpService = cleanUpService;
        }

        [Route("clear")]
        [HttpPost]
        public IActionResult Clear()
        {
            _cleanUpService.CleanDatabase();

            return Ok();
        }
    }
}