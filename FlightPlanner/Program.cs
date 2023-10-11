
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using FlightPlanner.Handlers;
using FlightPlanner.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions,
                BasicAuthenticationHandler>("BasicAuthentication", null);
        builder.Services.AddDbContext<FlightPlannerDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("flight-planner")));
        builder.Services.AddTransient<IDbService, DbService>();
        builder.Services.AddTransient<IEntityService<Airport>, EntityService<Airport>>();
        builder.Services.AddTransient<IEntityService<Flight>, EntityService<Flight>>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}