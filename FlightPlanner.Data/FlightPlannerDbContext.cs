
using FlightPlanner.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlightPlanner.Data;

	public class FlightPlannerDbContext : DbContext, IFlightPlannerDbContext
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