using AutoMapper;
using Flight_Planner.Models;
using FlightPlanner.Core.Interfaces;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IValidate> _validators;
        private static readonly object _locker = new object();
        public AdminAPIController(
            IFlightService flightService, 
            IMapper mapper,
            IEnumerable<IValidate> validators)
        {
            _mapper = mapper;
            _flightService = flightService;
            _validators = validators;
    }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {

            var flights = _flightService.GetById(id);
            
            if (flights == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FlightRequest>(flights));

        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(FlightRequest request)
        {
            lock (_locker)
            {
                var flight = _mapper.Map<Flight>(request);
                if (!_validators.All(v => v.IsValid(flight)))
                {
                    return BadRequest();
                }
                if (_flightService.Exists(flight))
                {
                    return Conflict();
                }

                _flightService.Create(flight);

                request = _mapper.Map<FlightRequest>(flight);
                return Created("", request);
            }

        }


        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _flightService.GetById(id);

            if (flight == null)
            {
                return Ok();
            }

            _flightService.Delete(flight);
            return Ok();
        }
    }
}