using FlightPlanner.Core.Models;
using FlightPLanner.Core.Interfaces;

namespace FlightPlanner.Validations
{
	public class FlightValuesValidator : IValidate
	{

        public bool IsValid(Flight flight)
        {
            return !string.IsNullOrEmpty(flight?.ArrivalTime) &&
                   !string.IsNullOrEmpty(flight?.DepartureTime) &&
                   !string.IsNullOrEmpty(flight?.Carrier) &&
                   flight?.To != null &&
                   flight?.From != null;
        }
    }
}