using System;
using System.Text.Json;
using WeatherApp.Utils;

namespace WeatherApp.CLI
{
    /// <summary>
    /// Command-line interface for C# weather utilities
    /// Can be called from Python via subprocess
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            try
            {
                string command = args[0].ToLower();

                switch (command)
                {
                    case "validate-city":
                        if (args.Length < 2) { Console.WriteLine("Error: City name required"); return; }
                        ValidateCity(args[1]);
                        break;

                    case "validate-coords":
                        if (args.Length < 3) { Console.WriteLine("Error: Latitude and longitude required"); return; }
                        ValidateCoordinates(args[1], args[2]);
                        break;

                    case "format-temp":
                        if (args.Length < 2) { Console.WriteLine("Error: Temperature required"); return; }
                        FormatTemperature(args[1], args.Length > 2 ? args[2][0] : 'C');
                        break;

                    case "wind-direction":
                        if (args.Length < 2) { Console.WriteLine("Error: Degrees required"); return; }
                        GetWindDirection(args[1]);
                        break;

                    case "categorize-aqi":
                        if (args.Length < 2) { Console.WriteLine("Error: AQI index required"); return; }
                        CategorizeAQI(args[1]);
                        break;

                    case "aqi-recommendation":
                        if (args.Length < 2) { Console.WriteLine("Error: AQI index required"); return; }
                        GetAQIRecommendation(args[1]);
                        break;

                    case "mps-to-kmh":
                        if (args.Length < 2) { Console.WriteLine("Error: Speed required"); return; }
                        ConvertMpsToKmh(args[1]);
                        break;

                    case "mps-to-mph":
                        if (args.Length < 2) { Console.WriteLine("Error: Speed required"); return; }
                        ConvertMpsToMph(args[1]);
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        PrintHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void ValidateCity(string cityName)
        {
            var (isValid, error) = ValidationUtils.ValidateCityName(cityName);
            var response = new { valid = isValid, error };
            Console.WriteLine(JsonSerializer.Serialize(response));
        }

        static void ValidateCoordinates(string latitude, string longitude)
        {
            var (isValid, error) = ValidationUtils.ValidateCoordinates(latitude, longitude);
            var response = new { valid = isValid, error };
            Console.WriteLine(JsonSerializer.Serialize(response));
        }

        static void FormatTemperature(string tempStr, char unit)
        {
            if (double.TryParse(tempStr, out double temp))
            {
                string formatted = TemperatureUtils.FormatTemperature(temp, unit);
                Console.WriteLine(formatted);
            }
            else
            {
                Console.WriteLine("Error: Invalid temperature value");
            }
        }

        static void GetWindDirection(string degreesStr)
        {
            if (double.TryParse(degreesStr, out double degrees))
            {
                string direction = WindUtils.GetWindDirection(degrees);
                Console.WriteLine(direction);
            }
            else
            {
                Console.WriteLine("Error: Invalid degrees value");
            }
        }

        static void CategorizeAQI(string aqiStr)
        {
            if (int.TryParse(aqiStr, out int aqi))
            {
                string category = AQIUtils.CategorizeAQI(aqi);
                Console.WriteLine(category);
            }
            else
            {
                Console.WriteLine("Unknown");
            }
        }

        static void GetAQIRecommendation(string aqiStr)
        {
            if (int.TryParse(aqiStr, out int aqi))
            {
                string recommendation = AQIUtils.GetAQIRecommendation(aqi);
                Console.WriteLine(recommendation);
            }
            else
            {
                Console.WriteLine("Unable to determine air quality");
            }
        }

        static void ConvertMpsToKmh(string mpsStr)
        {
            if (double.TryParse(mpsStr, out double mps))
            {
                double kmh = WindUtils.MetersPerSecondToKmh(mps);
                Console.WriteLine(kmh.ToString("F2"));
            }
            else
            {
                Console.WriteLine("Error: Invalid speed value");
            }
        }

        static void ConvertMpsToMph(string mpsStr)
        {
            if (double.TryParse(mpsStr, out double mps))
            {
                double mph = WindUtils.MetersPerSecondToMph(mps);
                Console.WriteLine(mph.ToString("F2"));
            }
            else
            {
                Console.WriteLine("Error: Invalid speed value");
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("Weather App C# Utilities");
            Console.WriteLine("Usage: WeatherAppUtils.CLI <command> [args]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  validate-city <name>              - Validate city name");
            Console.WriteLine("  validate-coords <lat> <lon>       - Validate coordinates");
            Console.WriteLine("  format-temp <temp> [C|F]          - Format temperature");
            Console.WriteLine("  wind-direction <degrees>          - Get wind direction");
            Console.WriteLine("  categorize-aqi <index>            - Categorize AQI");
            Console.WriteLine("  aqi-recommendation <index>        - Get AQI health recommendation");
            Console.WriteLine("  mps-to-kmh <speed>                - Convert m/s to km/h");
            Console.WriteLine("  mps-to-mph <speed>                - Convert m/s to mph");
        }
    }
}
