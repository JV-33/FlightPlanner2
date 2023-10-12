using FlightPlanner.Data;
using FlightPLanner.Core.Services;

namespace FlightPlanner.Services
{
	public class CleanupService : DbService, ICleanupService
	{
		public CleanupService(IFlightPlannerDbContext context) : base(context)
		{
		}

        public void CleanupDatabase()
        {
            _context.Airport.RemoveRange(_context.Airport);
            _context.Flights.RemoveRange(_context.Flights);
            _context.SaveChanges();
        }
    }
}

