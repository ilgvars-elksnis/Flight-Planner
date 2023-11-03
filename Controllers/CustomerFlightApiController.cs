using AutoMapper;
using Flight_Planner.Models;
using FlightPlanner.Core.Interfaces;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class CustomerFlightApiController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IAirportService _airportService;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IStringValidation> _stringValidation;
        private readonly ILogger<CustomerFlightApiController> _logger;
        private readonly IFlightSearchValidator _flightSearchValidator;

        public CustomerFlightApiController(
            ILogger<CustomerFlightApiController> logger,
            IAirportService airportService,
            IFlightService flightService,
            IMapper mapper,
            IEnumerable<IStringValidation> stringValidation,
            IFlightSearchValidator flightSearchValidator)
        {
            _logger = logger;
            _airportService = airportService;
            _flightService = flightService;
            _mapper = mapper;
            _stringValidation = stringValidation;
            _flightSearchValidator = flightSearchValidator;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            var isSearchValid = _stringValidation.Any(v => v.IsValid(search));

            if (!isSearchValid)
                return BadRequest("Search is empty or missing");

            var searchLower = search.Trim().ToLowerInvariant();

            var uniqueAirports = _airportService.SearchAirports(searchLower);

            var hasResults = uniqueAirports.Any();
            var areResultsValid = _stringValidation.Any(v => v.IsValid(hasResults));

            if (!areResultsValid)
                return NotFound("No results found");

            var airportRequests = _mapper.Map<List<AirportRequest>>(uniqueAirports);

            return Ok(airportRequests);
        }

        [Route("flights/search")]
        [HttpPost]

        public IActionResult SearchFlights(SearchFlightsRequest request)
        {
            _logger.LogInformation("SearchFlights endpoint hit with From: {From} and To: {To}", request.From, request.To);

            if (!_flightSearchValidator.IsValid(request))
            {
                _logger.LogWarning("Invalid request: Missing required fields. From: {From} and To: {To}", request.From, request.To);
                return BadRequest(new { Error = "Invalid request. Missing fields." });
            }

            try
            {
                var flights = _flightService.SearchFlights(request);

                if (flights != null && flights.Any())
                {
                    return Ok(new { page = 0, totalItems = flights.Count(), items = flights.ToList() });
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
            var flight = _flightService.GetFullFlightById(id);

            if (flight != null)
            {
                return Ok(_mapper.Map<FlightRequest>(flight));
            }
            else
            {
                return NotFound(new { Error = "Flight not found." });
            }
        }
    }
}
