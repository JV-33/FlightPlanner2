using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.DataDB;
using FlightPlanner.Storage;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private readonly ILogger<CustomerAPIController> _logger;
        private readonly FlightPlannerDbContext _context;

        public CustomerAPIController(FlightPlannerDbContext context, ILogger<CustomerAPIController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search term is missing or empty");

            var searchLower = search.Trim().ToLowerInvariant();

            var fromAirports = _context.Flights
                .Include(f => f.From)
                .Where(f => EF.Functions.Like(f.From.AirportCode, $"%{searchLower}%") ||
                            EF.Functions.Like(f.From.City, $"%{searchLower}%") ||
                            EF.Functions.Like(f.From.Country, $"%{searchLower}%"))
                .Select(f => f.From)
                .Distinct();

            var toAirports = _context.Flights
                .Include(f => f.To)
                .Where(f => EF.Functions.Like(f.To.AirportCode, $"%{searchLower}%") ||
                            EF.Functions.Like(f.To.City, $"%{searchLower}%") ||
                            EF.Functions.Like(f.To.Country, $"%{searchLower}%"))
                .Select(f => f.To)
                .Distinct();

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

            try
            {
                var flights = _context.Flights
                                      .Where(f => f.From.AirportCode == request.From && f.To.AirportCode == request.To)
                                      .ToList();

                return Ok(new { page = 0, totalItems = flights?.Count ?? 0, items = flights ?? new List<Flight>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching flights with From: {From} and To: {To}", request.From, request.To);
                throw;
            }
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult FindFlightById(int id)
        {
            var flight = _context.Flights
                                 .Include(f => f.From)
                                 .Include(f => f.To)
                                 .FirstOrDefault(f => f.ID == id);

            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        [HttpGet]
        [Route("flights")]
        public IActionResult GetAllFlights()
        {
            var flights = _context.Flights.ToList();
            Console.WriteLine($"Number of flights: {flights.Count}");
            if (flights == null || !flights.Any())
            {
                return NotFound("No flights found");
            }
            return Ok(flights);
        }
    }
}