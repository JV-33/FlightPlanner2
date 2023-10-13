using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPLanner.Core.Interfaces;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private readonly ILogger<CustomerAPIController> _logger;
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IValidate> _validators;

        public CustomerAPIController(IFlightService flightService, IMapper mapper, IEnumerable<IValidate> validators, ILogger<CustomerAPIController> logger)
        {
            _flightService = flightService;
            _mapper = mapper;
            _validators = validators;
            _logger = logger;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search term is missing or empty");

            var searchLower = search.Trim().ToLowerInvariant();

            var fromAirports = _flightService.GetAllFlightsWithAirports()
                .Where(f => f.From != null && (f.From.AirportCode.ToLowerInvariant().Contains(searchLower) ||
                                               f.From.City.ToLowerInvariant().Contains(searchLower) ||
                                               f.From.Country.ToLowerInvariant().Contains(searchLower)))
                .Select(f => f.From)
                .Distinct()
                .Select(a => new
                {
                    airport = a.AirportCode,
                    a.City,
                    a.Country
                });

            _logger.LogInformation($"Found {fromAirports.Count()} from airports.");

            var toAirports = _flightService.GetAllFlightsWithAirports()
                .Where(f => f.To != null && (f.To.AirportCode.ToLowerInvariant().Contains(searchLower) ||
                                             f.To.City.ToLowerInvariant().Contains(searchLower) ||
                                             f.To.Country.ToLowerInvariant().Contains(searchLower)))
                .Select(f => f.To)
                .Distinct()
                .Select(a => new
                {
                    airport = a.AirportCode,
                    a.City,
                    a.Country
                });

            _logger.LogInformation($"Found {toAirports.Count()} to airports.");

            var uniqueAirports = fromAirports.Union(toAirports).ToList();

            if (!uniqueAirports.Any())
                return NotFound("No airports found");

            return Ok(uniqueAirports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightsRequest request)
        {
            _logger.LogInformation("SearchFlights endpoint hit with From: {From} and To: {To}", request.From, request.To);

            if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To) || request.From == request.To)
            {
                _logger.LogWarning("Invalid Request: Missing required fields. From: {From}, To: {To}", request.From, request.To);
                return BadRequest(new { Error = "Invalid Request: Missing required fields." });
            }

            var flights = _flightService.GetAllFlightsWithAirports()
                                      .Where(f => f.From != null &&
                                                  f.From.AirportCode == request.From &&
                                                  f.To != null &&
                                                  f.To.AirportCode == request.To &&
                                                  f.DepartureTime.Contains(request.DepartureDate))
                                      .ToList();

            return Ok(new { page = 0, totalItems = flights?.Count ?? 0, items = flights ?? new List<Flight>() });
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult FindFlightById(int id)
        {
            var flight = _flightService.GetFullFlightById(id);

            if (flight == null)
                return NotFound();

            var result = _mapper.Map<FlightRequest>(flight);

            return Ok(result);
        }

        [HttpGet]
        [Route("flights")]
        public IActionResult GetAllFlights()
        {
            var flights = _flightService.Get().ToList();
            if (flights == null || !flights.Any())
            {
                return NotFound("No flights found");
            }
            return Ok(flights);
        }
    }
}