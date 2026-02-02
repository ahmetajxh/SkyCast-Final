using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp.Utils
{
    /// <summary>
    /// Weather utility functions - C# implementation
    /// Core business logic for the Weather App
    /// </summary>
    public class ValidationUtils
    {
        /// <summary>
        /// Validate city name input
        /// </summary>
        public static (bool, string?) ValidateCityName(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return (false, "City name is required");

            if (cityName.Length < 2)
                return (false, "City name must be at least 2 characters");

            if (cityName.Length > 100)
                return (false, "City name is too long");

            return (true, null);
        }

        /// <summary>
        /// Validate latitude and longitude coordinates
        /// </summary>
        public static (bool, string?) ValidateCoordinates(string latitude, string longitude)
        {
            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
                return (false, "Latitude and longitude are required");

            try
            {
                if (!double.TryParse(latitude, out double lat) || !double.TryParse(longitude, out double lon))
                    return (false, "Invalid coordinate format. Must be numeric values");

                if (!(lat >= -90 && lat <= 90))
                    return (false, "Latitude must be between -90 and 90");

                if (!(lon >= -180 && lon <= 180))
                    return (false, "Longitude must be between -180 and 180");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Invalid coordinate format: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Temperature formatting utilities
    /// </summary>
    public class TemperatureUtils
    {
        /// <summary>
        /// Format temperature in Celsius or Fahrenheit
        /// </summary>
        public static string FormatTemperature(double celsius, char unit = 'C')
        {
            if (unit == 'F')
            {
                double fahrenheit = celsius * 9.0 / 5.0 + 32;
                return $"{Math.Round(fahrenheit)}°F";
            }
            return $"{Math.Round(celsius)}°C";
        }

        /// <summary>
        /// Convert Celsius to Fahrenheit
        /// </summary>
        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 9.0 / 5.0 + 32;
        }

        /// <summary>
        /// Convert Fahrenheit to Celsius
        /// </summary>
        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32) * 5.0 / 9.0;
        }
    }

    /// <summary>
    /// Air Quality Index utilities
    /// </summary>
    public class AQIUtils
    {
        /// <summary>
        /// Categorize AQI index into descriptive labels
        /// </summary>
        public static string CategorizeAQI(int? aqiIndex)
        {
            if (aqiIndex == null)
                return "Unknown";

            return aqiIndex switch
            {
                <= 20 => "Excellent",
                <= 40 => "Good",
                <= 60 => "Moderate",
                <= 80 => "Poor",
                _ => "Very Poor"
            };
        }

        /// <summary>
        /// Get health recommendation based on AQI
        /// </summary>
        public static string GetAQIRecommendation(int? aqiIndex)
        {
            if (aqiIndex == null)
                return "Unable to determine air quality";

            return aqiIndex switch
            {
                <= 20 => "Good conditions for outdoor activities",
                <= 40 => "Good air quality overall",
                <= 60 => "Sensitive groups may experience effects",
                <= 80 => "Members of general public may feel effects",
                _ => "Everyone may begin to feel effects. Limit outdoor activities"
            };
        }
    }

    /// <summary>
    /// Wind speed utilities
    /// </summary>
    public class WindUtils
    {
        /// <summary>
        /// Get wind direction name from degrees
        /// </summary>
        public static string GetWindDirection(double degrees)
        {
            degrees = degrees % 360;
            
            return degrees switch
            {
                >= 348.75 or < 11.25 => "N",
                >= 11.25 and < 33.75 => "NNE",
                >= 33.75 and < 56.25 => "NE",
                >= 56.25 and < 78.75 => "ENE",
                >= 78.75 and < 101.25 => "E",
                >= 101.25 and < 123.75 => "ESE",
                >= 123.75 and < 146.25 => "SE",
                >= 146.25 and < 168.75 => "SSE",
                >= 168.75 and < 191.25 => "S",
                >= 191.25 and < 213.75 => "SSW",
                >= 213.75 and < 236.25 => "SW",
                >= 236.25 and < 258.75 => "WSW",
                >= 258.75 and < 281.25 => "W",
                >= 281.25 and < 303.75 => "WNW",
                >= 303.75 and < 326.25 => "NW",
                >= 326.25 and < 348.75 => "NNW",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Convert wind speed from m/s to km/h
        /// </summary>
        public static double MetersPerSecondToKmh(double mps)
        {
            return mps * 3.6;
        }

        /// <summary>
        /// Convert wind speed from m/s to mph
        /// </summary>
        public static double MetersPerSecondToMph(double mps)
        {
            return mps * 2.237;
        }
    }

    /// <summary>
    /// JSON response builder for consistent API responses
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static string ToJson(bool success, string? message = null, object? data = null)
        {
            var response = new ApiResponse
            {
                Success = success,
                Message = message,
                Data = data
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };

            return JsonSerializer.Serialize(response, options);
        }
    }

    /// <summary>
    /// Weather Data Models
    /// </summary>
    public class GeocodingResult
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Timezone { get; set; }
        public string? Admin1 { get; set; }
    }

    public class WeatherData
    {
        public string? City { get; set; }
        public double Temperature { get; set; }
        public string? Description { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int Pressure { get; set; }
        public double FeelsLike { get; set; }
    }

    public class AirQualityData
    {
        public int? EuAqi { get; set; }
        public double? Pm25 { get; set; }
        public double? Pm10 { get; set; }
        public double? No2 { get; set; }
    }
}
