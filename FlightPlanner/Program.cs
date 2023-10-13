using FlightPlanner.Data;
using FlightPlanner.Handlers;
using FlightPlanner.Services.Exstensions;
using FlightPLanner.Core.Interfaces;
using FlightPlanner.Validations;
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
     
        builder.Services.RegisterServices();

        builder.Services.AddTransient<IValidate, FlightValuesValidator>();
        builder.Services.AddTransient<IValidate, AirportValuesValidator>();
        builder.Services.AddTransient<IValidate, SameAirportValidator>();
        builder.Services.AddTransient<IValidate, FlightDatesValidator>();
        var mapper = AutoMapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

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