
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using FlightPlanner.Services;
using Microsoft.AspNetCore.Mvc;


namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController : ControllerBase
    {
        private readonly IDbService _dbService;

        public CleanupApiController(IDbService dbService)
        {
            _dbService  =  dbService;
        }

        [Route("clear")]
        [HttpPost]
        public IActionResult ClearFlights()
        {
            _dbService.DeleteRange<Flight>();
            _dbService.DeleteRange<Airport>();
            return Ok();
        }
    }
}