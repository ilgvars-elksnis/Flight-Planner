using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private readonly FlightStorage _storage;
        private readonly FlightPlannerDbContext _context;
        private static readonly object _locker = new object();
        public AdminAPIController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
           
            var flights = _context.Flights.SingleOrDefault(f => f.ID == id);
            if (flights == null)
            {
                    return NotFound();
            }
            return Ok(flights);
            
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(Flight flight)
        {
            lock (_locker)
            {
                if (flight == null)
                {
                    return BadRequest("Flight is empty");
                }

                if (flight.From == null || string.IsNullOrEmpty(flight.From.Country) || string.IsNullOrEmpty(flight.From.City) || string.IsNullOrEmpty(flight.From.AirportCode))
                {
                    return BadRequest("Missing information 'from'");
                }

                if (flight.To == null || string.IsNullOrEmpty(flight.To.Country) || string.IsNullOrEmpty(flight.To.City) || string.IsNullOrEmpty(flight.To.AirportCode))
                {
                    return BadRequest("Missing information 'to'");
                }

                if (string.IsNullOrEmpty(flight.Carrier))
                    return BadRequest("Missing carrier");

                if (string.IsNullOrEmpty(flight.DepartureTime))
                    return BadRequest("Missing departure time");

                if (string.IsNullOrEmpty(flight.ArrivalTime))
                    return BadRequest("Missing arrival time");

                if (flight.From.AirportCode.Trim().Equals(flight.To.AirportCode.Trim(), StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Deprature and arrival airports must be different");

                DateTime departureTime, arrivalTime;
                if (!DateTime.TryParse(flight.DepartureTime, out departureTime) || !DateTime.TryParse(flight.ArrivalTime, out arrivalTime))
                    return BadRequest("Invalit time format");

                if (arrivalTime <= departureTime)
                    return BadRequest("Arrival must be later than departure");

                var existingFlight = _context.Flights.FirstOrDefault(f =>
                    f.From.Country == flight.From.Country &&
                    f.From.City == flight.From.City &&
                    f.From.AirportCode == flight.From.AirportCode &&
                    f.To.Country == flight.To.Country &&
                    f.To.City == flight.To.City &&
                    f.To.AirportCode == flight.To.AirportCode &&
                    f.DepartureTime == flight.DepartureTime
    );

                if (existingFlight != null)
                {
                    return Conflict("Flight already exists.");
                }

                // _storage.AddFlight(flight);
                _context.Flights.Add(flight);
                _context.SaveChanges();
            }

            return Created("", flight);

        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            //_storage.DeleteFlight(id);

            var flight = _context.Flights.SingleOrDefault(f => f.ID == id);
            if (flight == null)
            {
                return Ok();
            }
            _context.Flights.Remove(flight);
            _context.SaveChanges(); // Save changes to the database
            

            return Ok();
        }
    }
}