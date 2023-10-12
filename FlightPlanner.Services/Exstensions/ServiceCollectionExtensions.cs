using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using FlightPLanner.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlightPlanner.Services.Exstensions
{
	public static class ServiceCollectionExtensions
	{
		public static void RegisterServices(this IServiceCollection services)
		{
            services.AddTransient<IFlightPlannerDbContext, FlightPlannerDbContext>();
            services.AddTransient<IDbService, DbService>();
            services.AddTransient<IEntityService<Airport>, EntityService<Airport>>();
            services.AddTransient<IEntityService<Flight>, EntityService<Flight>>();
            services.AddTransient<IFlightService, FlightService>();
            services.AddTransient<ICleanupService, CleanupService>();
        }
	}
}

