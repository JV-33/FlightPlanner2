using FlightPlanner.DataDB;
using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Storage
{
    public class FlightStorage
    {
        private static List<Flight> _flightStorage = new List<Flight>();
        private readonly FlightPlannerDbContext _context;

        public FlightStorage(FlightPlannerDbContext context)
        {
            _context = context;
        }

        public bool AddFlight(Flight flight)
        {
            var existingFlight = GetExistingFlight(flight);
            if (existingFlight != null)
            {
                Console.WriteLine($"Flight already exists: {existingFlight.ID}");
                return false;
            }

            _context.Flights.Add(flight);
            _context.SaveChanges();
            Console.WriteLine($"Flight added: {flight.ID}");
            return true;
        }

        public List<Flight> GetCopyOfFlightStorage()
        {
            return _flightStorage.ToList();
        }

        public void Clear()
        {
            _flightStorage.Clear();
        }

        public bool FlightExists(int id)
        {
            return _flightStorage.Any(f => f.ID == id);
        }

        public Flight GetExistingFlight(Flight flight)
        {
            {
                return _flightStorage.FirstOrDefault(f =>
                    f.From.Country == flight.From.Country &&
                    f.From.City == flight.From.City &&
                    f.From.AirportCode == flight.From.AirportCode &&
                    f.To.Country == flight.To.Country &&
                    f.To.City == flight.To.City &&
                    f.To.AirportCode == flight.To.AirportCode &&
                    f.DepartureTime == flight.DepartureTime
                );
            }
        }

        public Flight? GetFlight(int id)
        {
            return _context.Flights.FirstOrDefault(flight => flight.ID == id);
        }

        public bool DeleteFlight(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.ID == id);
            if (flight == null) return false;

            _context.Flights.Remove(flight);
            _context.SaveChanges();
            return true;
        }

        public static List<Airport> GetUniqueAirports(FlightPlannerDbContext context, string search)
        {
            var searchLower = search.Trim().ToLowerInvariant();

            var fromAirports = context.Flights
                .Include(f => f.From)
                .Where(f => EF.Functions.Like(f.From.AirportCode, $"%{searchLower}%") ||
                            EF.Functions.Like(f.From.City, $"%{searchLower}%") ||
                            EF.Functions.Like(f.From.Country, $"%{searchLower}%"))
                .Select(f => f.From)
                .Distinct();

            var toAirports = context.Flights
                .Include(f => f.To)
                .Where(f => EF.Functions.Like(f.To.AirportCode, $"%{searchLower}%") ||
                            EF.Functions.Like(f.To.City, $"%{searchLower}%") ||
                            EF.Functions.Like(f.To.Country, $"%{searchLower}%"))
                .Select(f => f.To)
                .Distinct();

            var uniqueAirports = fromAirports.Union(toAirports).ToList();

            return uniqueAirports;
        }

        public List<Flight> SearchFlights(SearchFlightsRequest request)
        {
            return _context.Flights
                        .Where(f => f.From.AirportCode == request.From &&
                                    f.To.AirportCode == request.To &&
                                    f.DepartureTime.Contains(request.DepartureDate))
                        .ToList();
        }
    }
}