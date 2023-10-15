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
        private readonly FlightPlannerDbContext _context;
        private readonly ILogger<CustomerFlightApiController> _logger;

        public CustomerFlightApiController( ILogger<CustomerFlightApiController> logger, FlightPlannerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search is empty or missing");

            var searchLower = search.Trim().ToLowerInvariant();

            var allAirports = _context.Flights
                .AsEnumerable() // Bring data into memory for client-side evaluation
                .SelectMany(f => new[] { f.From, f.To })
                .Where(a => a != null && (a.AirportCode.Trim().ToLowerInvariant().Contains(searchLower) ||
                            a.City.Trim().ToLowerInvariant().Contains(searchLower) ||
                            a.Country.Trim().ToLowerInvariant().Contains(searchLower)))
                .ToList();

            var uniqueAirports = allAirports
                .GroupBy(a => a.AirportCode)
                .Select(g => g.First())
                .ToList();

            if (!uniqueAirports.Any())
                return NotFound("No results found");

            return Ok(uniqueAirports);
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
                var flights = _context.Flights
                    .Where(f => f.From.AirportCode == request.From &&
                                f.To.AirportCode == request.To &&
                                f.DepartureTime.Contains(request.DepartureDate))
                    .ToList();

                if (flights != null && flights.Any())
                {
                    return Ok(new { page = 0, totalItems = flights.Count, items = flights });
                }
                else
                {
                    return Ok(new { page = 0, totalItems = 0, items = new List<Flight>() });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request: {Message}", ex.Message);
                return BadRequest(new { Error = ex.Message });
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
            var flight = _context.Flights.FirstOrDefault(f => f.ID == id);

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