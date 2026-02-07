using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseCors("AllowAll");

            // Serve static files (HTML, CSS, JS)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Map controllers
            app.MapControllers();

            Console.WriteLine("\n🚀  Weather App Started!");
            Console.WriteLine("📍 Website: http://localhost:8000");
            Console.WriteLine("🔗 API: http://localhost:8000/api/health\n");

            app.Run("http://localhost:8000");
        }
    }
}

