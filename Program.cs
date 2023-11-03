using Flight_Planner;
using Flight_Planner.Validations;
using FlightPlanner.Controllers;
using FlightPlanner.Core.Interfaces;
using FlightPlanner.Services.Extensions;
using FlightPlanner.Data;
using FlightPlanner.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using FlightPlanner.Validation;

namespace FlightPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<FlightPlannerDbContext>(options => 
                options
                    .UseSqlServer(
                        builder.Configuration.GetConnectionString("flight-planner")));
            builder.Services.RegisterServices();
            builder.Services.AddTransient<IValidate, AirportValuesValidator>();
            builder.Services.AddTransient<IValidate, FlightDatesValidator>();
            builder.Services.AddTransient<IValidate, FlightValuesValidator>();
            builder.Services.AddTransient<IValidate, SameAirportValidator>();
            builder.Services.AddTransient<IStringValidation, NoResultsValidation>();
            builder.Services.AddTransient<IStringValidation, SearchEmptyValidation>();
            builder.Services.AddTransient<IFlightSearchValidator, FlightSearchValidator>();
            var mapper = AutoMapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            builder.Services.AddControllersWithViews()
            .AddApplicationPart(typeof(CustomerFlightApiController).Assembly);
            builder.Services.AddControllersWithViews().AddApplicationPart(typeof(CustomerFlightApiController).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
}