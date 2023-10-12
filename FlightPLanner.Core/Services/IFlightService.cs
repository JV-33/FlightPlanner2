using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;

namespace FlightPlanner.Core.Services
{
	public interface IFlightService : IEntityService<Flight>
	{
        Flight? GetFullFlightById(int id);

        bool Exists(Flight flight);
        IEnumerable<Flight> GetAllFlightsWithAirports();

    }
}