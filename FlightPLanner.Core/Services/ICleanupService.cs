using FlightPlanner.Core.Services;

namespace FlightPLanner.Core.Services
{
	public interface ICleanupService : IDbService
    {
        void CleanupDatabase();
    }
}