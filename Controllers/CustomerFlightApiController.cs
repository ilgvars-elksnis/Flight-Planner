using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class CustomerFlightApiController : ControllerBase
    {
        private readonly FlightStorage _storage;
        private readonly ILogger<CustomerFlightApiController> _logger;

        public CustomerFlightApiController(FlightStorage storage, ILogger<CustomerFlightApiController> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search is empty or  missing");

            var airports = _storage.GetAirports(search);
            if (airports == null || !airports.Any())
                return NotFound("No results found");

            return Ok(airports);
        }

        [Route("flights/search")]
        [HttpPost]

        public IActionResult SearchFlights(SearchFlightsRequest request)
        {
            _logger.LogInformation("SearchFlights endpoint hit with From: {From} and To: {To}", request.From, request.To);

            if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To) || request.From == request.To)
            {
                _logger.LogWarning("Invalid request: Missing required fields. From: {From} and To: {To}", request.From, request.To);
                return BadRequest(new { Error = "Invalid request. Missing fields." });
            }

            try
            {
                var flights = _storage.SearchFlights(request);

                if (flights != null && flights.Any())
                {
                    return Ok(new { page = 0, totalItems = flights.Count, items = flights });
                }
                else
                {
                    return Ok(new { page = 0, totalItems = 0, items = new List<Flight>() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for flights.");
                return StatusCode(500, new { Error = "An internal server error occurred." });
            }
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlightById(int id)
        {
            var flight = _storage.GetFlight(id);
            if (flight != null)
            {
                return Ok(flight);
            }
            else
            {
                return NotFound(new { Error = "Flight not found." });
            }
        }
    }
}