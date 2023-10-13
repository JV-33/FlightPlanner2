using FlightPlanner.Core.Models;

namespace FlightPLanner.Core.Interfaces
{
	public interface IValidate
	{
		bool IsValid(Flight flight);
	}
}