using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Storage;
using FlightPlanner.DataDB;

namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _lockObj = new object();

        public AdminAPIController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(Flight flight)
        {
            lock (_lockObj)
            {
                if (flight == null)
                    return BadRequest("Missing flight information");

                if (flight.From == null || string.IsNullOrEmpty(flight.From.Country) || string.IsNullOrEmpty(flight.From.City) || string.IsNullOrEmpty(flight.From.AirportCode))
                    return BadRequest("Incomplete 'from' airport information");

                if (flight.To == null || string.IsNullOrEmpty(flight.To.Country) || string.IsNullOrEmpty(flight.To.City) || string.IsNullOrEmpty(flight.To.AirportCode))
                    return BadRequest("Incomplete 'to' airport information");

                if (string.IsNullOrEmpty(flight.Carrier))
                    return BadRequest("Missing carrier information");

                if (string.IsNullOrEmpty(flight.DepartureTime))
                    return BadRequest("Missing departure time");

                if (string.IsNullOrEmpty(flight.ArrivalTime))
                    return BadRequest("Missing arrival time");
                if (flight.From.AirportCode.Trim().Equals(flight.To.AirportCode.Trim(), StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Departure and arrival airports must be different");

                DateTime departureTime, arrivalTime;
                if (!DateTime.TryParse(flight.DepartureTime, out departureTime) || !DateTime.TryParse(flight.ArrivalTime, out arrivalTime))
                    return BadRequest("Invalid time format");

                if (arrivalTime <= departureTime)
                    return BadRequest("Arrival time must be later than departure time");

                var existingFlight = _context.Flights
                             .FirstOrDefault(f => f.From.AirportCode == flight.From.AirportCode &&
                                                  f.To.AirportCode == flight.To.AirportCode &&
                                                  f.DepartureTime == flight.DepartureTime &&
                                                  f.ArrivalTime == flight.ArrivalTime &&
                                                  f.Carrier == flight.Carrier);

                if (existingFlight != null)
                    return Conflict("A flight with the same data already exists");

                _context.Flights.Add(flight);
                _context.SaveChanges();

            }
            return Created(" ", flight);
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.ID == id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}