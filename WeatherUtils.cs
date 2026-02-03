using System;

namespace WeatherApp.Utils
{
    public class TemperatureUtils
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return (celsius * 9 / 5) + 32;
        }

        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32) * 5 / 9;
        }

        public static string FormatTemperature(double temp, string unit = "C")
        {
            return unit.ToUpper() == "F" 
                ? $"{temp:F1}°F" 
                : $"{temp:F1}°C";
        }
    }

    public class WindUtils
    {
        public static string GetWindDirection(double degrees)
        {
            if (degrees >= 337.5 || degrees < 22.5) return "N";
            if (degrees >= 22.5 && degrees < 67.5) return "NE";
            if (degrees >= 67.5 && degrees < 112.5) return "E";
            if (degrees >= 112.5 && degrees < 157.5) return "SE";
            if (degrees >= 157.5 && degrees < 202.5) return "S";
            if (degrees >= 202.5 && degrees < 247.5) return "SW";
            if (degrees >= 247.5 && degrees < 292.5) return "W";
            if (degrees >= 292.5 && degrees < 337.5) return "NW";
            return "N";
        }

        public static string GetWindSpeedCategory(double speedKmh)
        {
            if (speedKmh < 1) return "Calm";
            if (speedKmh < 12) return "Light";
            if (speedKmh < 30) return "Moderate";
            if (speedKmh < 50) return "Strong";
            return "Very Strong";
        }
    }

    public class AQIUtils
    {
        public static string CategorizeAQI(int aqi)
        {
            if (aqi <= 20) return "Good";
            if (aqi <= 40) return "Fair";
            if (aqi <= 60) return "Moderate";
            if (aqi <= 80) return "Poor";
            if (aqi <= 100) return "Very Poor";
            return "Extremely Poor";
        }

        public static string GetAQIColor(int aqi)
        {
            if (aqi <= 20) return "#50f550";
            if (aqi <= 40) return "#50ccaa";
            if (aqi <= 60) return "#f5cf50";
            if (aqi <= 80) return "#ff5050";
            if (aqi <= 100) return "#960032";
            return "#7d2181";
        }
    }
}
