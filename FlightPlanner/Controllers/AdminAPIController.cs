﻿using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using FlightPLanner.Core.Interfaces;
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
        private static readonly object _lockObj = new object();

        private readonly IMapper _mapper;
        private readonly IEnumerable<IValidate> _validators;

        public AdminAPIController(IFlightService flightService, IMapper mapper, IEnumerable<IValidate> validators)
        {
            _flightService = flightService;
            _mapper = mapper;
            _validators = validators;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            Flight flight = _flightService.GetFullFlightById (id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FlightRequest>(flight));
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(FlightRequest request)
        {
            lock (_lockObj)
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
            }

            return Created(" ", request);
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _flightService.Get().FirstOrDefault(f => f.ID == id);
            if (flight != null)
            {
                _flightService.Delete(flight);
            }

            return Ok();
        }
    }
}