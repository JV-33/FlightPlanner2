using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private readonly IEntityService<Flight> _flightService;
        private static readonly object _lockObj = new object();

        public AdminAPIController(IEntityService<Flight> flightService)
        {
            _flightService = flightService;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            Flight flight = _flightService.GetById(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(FlightRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is null");
            }

            var flight = MapToFlight(request);

            if (flight == null)
                return BadRequest("Missing flight information");

            if (flight.From == null)
                return BadRequest("Missing 'from' airport information");

            if (string.IsNullOrEmpty(flight.From.Country) || string.IsNullOrEmpty(flight.From.City) || string.IsNullOrEmpty(flight.From.AirportCode))
                return BadRequest("Incomplete 'from' airport information");

            if (flight.To == null)
                return BadRequest("Missing 'to' airport information");

            if (string.IsNullOrEmpty(flight.To.Country) || string.IsNullOrEmpty(flight.To.City) || string.IsNullOrEmpty(flight.To.AirportCode))
                return BadRequest("Incomplete 'to' airport information");

            if (string.IsNullOrEmpty(flight.Carrier))
                return BadRequest("Missing carrier information");

            if (string.IsNullOrEmpty(flight.DepartureTime))
                return BadRequest("Missing departure time");

            if (string.IsNullOrEmpty(flight.ArrivalTime))
                return BadRequest("Missing arrival time");

            if (flight.From.AirportCode.Trim().Equals(flight.To.AirportCode.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest("Departure and arrival airports must be different");

            if (!DateTime.TryParse(flight.DepartureTime, out DateTime departureTime) || !DateTime.TryParse(flight.ArrivalTime, out DateTime arrivalTime))
                return BadRequest("Invalid time format");

            if (arrivalTime <= departureTime)
                return BadRequest("Arrival time must be later than departure time");

            lock (_lockObj)
            {

                var existingFlight = _flightService.Get()
                                     .FirstOrDefault(f => f.From != null && f.To != null &&
                                                          f.From.AirportCode == flight.From.AirportCode &&
                                                          f.To.AirportCode == flight.To.AirportCode &&
                                                          f.DepartureTime == flight.DepartureTime &&
                                                          f.ArrivalTime == flight.ArrivalTime &&
                                                          f.Carrier == flight.Carrier);

                if (existingFlight != null)
                    return Conflict("A flight with the same data already exists");

                _flightService.Create(flight);
            }

            request = MapToFlightRequest(flight);
            return Created(" ", request);
        }


        private Flight MapToFlight(FlightRequest request)
        {
            return new Flight
            {
                ID = request.ID,
                ArrivalTime = request.ArrivalTime,
                Carrier = request.Carrier,
                DepartureTime = request.DepartureTime,
                From = new Airport
                {
                    AirportCode = request.From.Airport,
                    City = request.From.City,
                    Country = request.From.Country
                },
                To = new Airport
                {
                    AirportCode = request.To.Airport,
                    City = request.To.City,
                    Country = request.To.Country
                }
            };
        }

        private FlightRequest MapToFlightRequest(Flight flight)
        {
            return new FlightRequest
            {
                ArrivalTime = flight.ArrivalTime,
                Carrier = flight.Carrier,
                DepartureTime = flight.DepartureTime,
                From = new AirportRequest
                {
                    Airport = flight.From.AirportCode,
                    City = flight.From.City,
                    Country = flight.From.Country
                },
                To = new AirportRequest
                {
                    Airport = flight.To.AirportCode,
                    City = flight.To.City,
                    Country = flight.To.Country
                },
                ID = flight.ID,
            };
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