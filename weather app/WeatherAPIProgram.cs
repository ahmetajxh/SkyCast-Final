using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherApp.Utils;

namespace WeatherApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // Health check endpoint
            app.MapGet("/api/health", () => Results.Ok(new
            {
                status = "healthy",
                service = "WeatherAPI - ASP.NET Core",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            }));

            // Validation endpoints using C# utilities
            app.MapGet("/api/validate/city", (string name) =>
            {
                var result = ValidationUtils.ValidateCityName(name);
                return Results.Ok(new
                {
                    valid = result.Item1,
                    error = result.Item2,
                    input = name
                });
            });

            app.MapGet("/api/validate/coordinates", (string lat, string lon) =>
            {
                var result = ValidationUtils.ValidateCoordinates(lat, lon);
                return Results.Ok(new
                {
                    valid = result.Item1,
                    error = result.Item2,
                    latitude = lat,
                    longitude = lon
                });
            });

            // Temperature conversion endpoints
            app.MapGet("/api/temperature/format", (double celsius, char unit = 'C') =>
            {
                var formatted = TemperatureUtils.FormatTemperature(celsius, unit);
                return Results.Ok(new
                {
                    input = celsius,
                    unit = unit,
                    formatted = formatted
                });
            });

            app.MapGet("/api/temperature/convert/to-fahrenheit", (double celsius) =>
            {
                var fahrenheit = TemperatureUtils.CelsiusToFahrenheit(celsius);
                return Results.Ok(new
                {
                    celsius = celsius,
                    fahrenheit = fahrenheit
                });
            });

            app.MapGet("/api/temperature/convert/to-celsius", (double fahrenheit) =>
            {
                var celsius = TemperatureUtils.FahrenheitToCelsius(fahrenheit);
                return Results.Ok(new
                {
                    fahrenheit = fahrenheit,
                    celsius = celsius
                });
            });

            // Wind direction endpoint
            app.MapGet("/api/wind/direction", (double degrees) =>
            {
                var direction = WindUtils.GetWindDirection(degrees);
                return Results.Ok(new
                {
                    degrees = degrees,
                    direction = direction
                });
            });

            // AQI endpoints
            app.MapGet("/api/aqi/categorize", (int index) =>
            {
                var category = AQIUtils.CategorizeAQI(index);
                var recommendation = AQIUtils.GetAQIRecommendation(index);
                return Results.Ok(new
                {
                    index = index,
                    category = category,
                    recommendation = recommendation
                });
            });

            Console.WriteLine("\n🚀 ASP.NET Core Weather API Started");
            Console.WriteLine("📍 API: http://localhost:5000");
            Console.WriteLine("📚 Swagger UI: http://localhost:5000/swagger\n");

            app.Run("http://localhost:5000");
        }
    }
}
