using System;

namespace WeatherApp.Utils
{
    public class ValidationUtils
    {
        public static bool IsValidCityName(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return false;

            if (cityName.Length < 2 || cityName.Length > 100)
                return false;

            return true;
        }

        public static bool IsValidCoordinates(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && 
                   longitude >= -180 && longitude <= 180;
        }
    }
}
