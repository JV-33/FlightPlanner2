using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.DataDB;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;

        public CleanupApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("clear")]
        [HttpPost]
        public IActionResult ClearFlights()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airport.RemoveRange(_context.Airport);
            _context.SaveChanges();
            return Ok();
        }
    }
}