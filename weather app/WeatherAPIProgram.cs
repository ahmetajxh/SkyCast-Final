using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using WeatherApp.Utils;

namespace WeatherApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Enable CORS
            app.UseCors("AllowAll");

            // Serve static files (HTML, CSS, JS)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // API Endpoints

            // Health check endpoint
            app.MapGet("/api/health", () => Results.Ok(new
            {
                status = "ok",
                service = "Weather API",
                timestamp = DateTime.UtcNow
            }));

            // Geocode city by name
            app.MapGet("/api/geocode", async (string name, HttpClient httpClient) =>
            {
                if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                {
                    return Results.BadRequest(new { error = "City name must be at least 2 characters" });
                }

                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(name)}&count=1&language=en&format=json";
                
                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var data = JsonSerializer.Deserialize<JsonElement>(response);
                    
                    if (!data.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                    {
                        return Results.NotFound(new { error = "City not found" });
                    }

                    var city = results[0];
                    return Results.Ok(new
                    {
                        name = city.GetProperty("name").GetString(),
                        country = city.TryGetProperty("country", out var country) ? country.GetString() : null,
                        latitude = city.GetProperty("latitude").GetDouble(),
                        longitude = city.GetProperty("longitude").GetDouble(),
                        timezone = city.TryGetProperty("timezone", out var tz) ? tz.GetString() : null,
                        admin1 = city.TryGetProperty("admin1", out var admin1) ? admin1.GetString() : null,
                        population = city.TryGetProperty("population", out var pop) ? pop.GetInt32() : 0
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Geocoding service error: {ex.Message}");
                }
            });

            // Get weather forecast for coordinates
            app.MapGet("/api/forecast", async (double lat, double lon, string? timezone, HttpClient httpClient) =>
            {
                timezone ??= "auto";
                
                var url = $"https://api.open-meteo.com/v1/forecast?" +
                          $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                          "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m,precipitation,pressure_msl,surface_pressure,visibility,uv_index,is_day" +
                          "&hourly=temperature_2m,weather_code,precipitation_probability,precipitation,visibility,uv_index" +
                          "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max,wind_speed_10m_max,uv_index_max";
                
                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var data = JsonSerializer.Deserialize<JsonElement>(response);
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Forecast service error: {ex.Message}");
                }
            });

            // Get air quality data
            app.MapGet("/api/air-quality", async (double lat, double lon, string? timezone, HttpClient httpClient) =>
            {
                timezone ??= "auto";
                
                var url = $"https://air-quality-api.open-meteo.com/v1/air-quality?" +
                          $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                          "&current=european_aqi,pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone,dust" +
                          "&hourly=pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone";
                
                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var data = JsonSerializer.Deserialize<JsonElement>(response);
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Air quality service error: {ex.Message}");
                }
            });

            // Combined weather endpoint (all-in-one)
            app.MapGet("/api/weather", async (string city, HttpClient httpClient) =>
            {
                if (string.IsNullOrWhiteSpace(city) || city.Length < 2)
                {
                    return Results.BadRequest(new { error = "City name must be at least 2 characters" });
                }

                try
                {
                    // Step 1: Geocode
                    var geocodeUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json";
                    var geocodeResponse = await httpClient.GetStringAsync(geocodeUrl);
                    var geocodeData = JsonSerializer.Deserialize<JsonElement>(geocodeResponse);
                    
                    if (!geocodeData.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                    {
                        return Results.NotFound(new { error = "City not found" });
                    }

                    var cityData = results[0];
                    var lat = cityData.GetProperty("latitude").GetDouble();
                    var lon = cityData.GetProperty("longitude").GetDouble();
                    var timezone = cityData.TryGetProperty("timezone", out var tz) ? tz.GetString() : "auto";

                    // Step 2: Get forecast
                    var forecastUrl = $"https://api.open-meteo.com/v1/forecast?" +
                                     $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                                     "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m,precipitation,pressure_msl,surface_pressure,visibility,uv_index,is_day" +
                                     "&hourly=temperature_2m,weather_code,precipitation_probability,precipitation,visibility,uv_index" +
                                     "&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_sum,precipitation_probability_max,wind_speed_10m_max,uv_index_max";
                    
                    var forecastResponse = await httpClient.GetStringAsync(forecastUrl);
                    var forecastData = JsonSerializer.Deserialize<JsonElement>(forecastResponse);

                    // Step 3: Get air quality
                    var aqiUrl = $"https://air-quality-api.open-meteo.com/v1/air-quality?" +
                                $"latitude={lat}&longitude={lon}&timezone={timezone}" +
                                "&current=european_aqi,pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone,dust" +
                                "&hourly=pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone";
                    
                    var aqiResponse = await httpClient.GetStringAsync(aqiUrl);
                    var aqiData = JsonSerializer.Deserialize<JsonElement>(aqiResponse);

                    // Combine all data
                    return Results.Ok(new
                    {
                        city = new
                        {
                            name = cityData.GetProperty("name").GetString(),
                            country = cityData.TryGetProperty("country", out var country) ? country.GetString() : null,
                            latitude = lat,
                            longitude = lon,
                            timezone = timezone
                        },
                        current = forecastData.GetProperty("current"),
                        hourly = forecastData.GetProperty("hourly"),
                        daily = forecastData.GetProperty("daily"),
                        current_units = forecastData.GetProperty("current_units"),
                        hourly_units = forecastData.GetProperty("hourly_units"),
                        daily_units = forecastData.GetProperty("daily_units"),
                        air_quality = aqiData.GetProperty("current"),
                        air_quality_units = aqiData.GetProperty("current_units")
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Weather service error: {ex.Message}");
                }
            });

            // C# Utility endpoints
            app.MapGet("/api/validate/city", (string name) =>
            {
                var valid = ValidationUtils.IsValidCityName(name);
                return Results.Ok(new
                {
                    valid = valid,
                    error = valid ? null : "Invalid city name",
                    input = name
                });
            });

            app.MapGet("/api/validate/coordinates", (string lat, string lon) =>
            {
                bool valid = false;
                if (double.TryParse(lat, out double latitude) && double.TryParse(lon, out double longitude))
                {
                    valid = ValidationUtils.IsValidCoordinates(latitude, longitude);
                }
                return Results.Ok(new
                {
                    valid = valid,
                    error = valid ? null : "Invalid coordinates",
                    latitude = lat,
                    longitude = lon
                });
            });

            // Temperature conversion endpoints
            app.MapGet("/api/temperature/format", (double celsius, string unit = "C") =>
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
                var color = AQIUtils.GetAQIColor(index);
                return Results.Ok(new
                {
                    index = index,
                    category = category,
                    color = color
                });
            });

            Console.WriteLine("\n🚀  Weather App Started!");
            Console.WriteLine("📍 Website: http://localhost:8000");
            Console.WriteLine("� API: http://localhost:8000/api/health\n");

            app.Run("http://localhost:8000");
        }
    }
}

