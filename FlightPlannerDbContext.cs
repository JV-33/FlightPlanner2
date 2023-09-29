using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.DataDB
{
	public class FlightPlannerDbContext : DbContext
    {
		public FlightPlannerDbContext(DbContextOptions<FlightPlannerDbContext> options) : base(options)
		{
		}

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airport { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airport>().HasKey(a => a.ID);
        }
    }
}