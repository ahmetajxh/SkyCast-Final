# services.py - Business Logic Layer
import requests
from config import Config

class WeatherService:
    """Service class for weather-related operations"""
    
    def __init__(self):
        self.geocoding_url = Config.GEOCODING_API_URL
        self.forecast_url = Config.FORECAST_API_URL
        self.aqi_url = Config.AIR_QUALITY_API_URL
        self.timeout = Config.API_TIMEOUT
    
    def geocode_city(self, city_name):
        """
        Search for a city by name and return its coordinates
        
        Args:
            city_name (str): Name of the city to search
            
        Returns:
            dict: City information or error
        """
        try:
            params = {
                'name': city_name,
                'count': 1,
                'language': 'en',
                'format': 'json'
            }
            
            response = requests.get(
                self.geocoding_url, 
                params=params, 
                timeout=self.timeout
            )
            response.raise_for_status()
            data = response.json()
            
            if not data.get('results'):
                return {"error": "City not found"}
            
            city = data['results'][0]
            return {
                "name": city.get('name'),
                "country": city.get('country'),
                "latitude": city.get('latitude'),
                "longitude": city.get('longitude'),
                "timezone": city.get('timezone'),
                "admin1": city.get('admin1'),
                "population": city.get('population')
            }
            
        except requests.exceptions.Timeout:
            return {"error": "Request timed out. Please try again."}
        except requests.exceptions.RequestException as e:
            return {"error": f"Geocoding service unavailable: {str(e)}"}
    
    def get_forecast(self, latitude, longitude, timezone='auto'):
        """
        Get weather forecast for given coordinates
        
        Args:
            latitude (str/float): Latitude coordinate
            longitude (str/float): Longitude coordinate
            timezone (str): Timezone (default: 'auto')
            
        Returns:
            dict: Weather forecast data or error
        """
        try:
            params = {
                'latitude': latitude,
                'longitude': longitude,
                'timezone': timezone,
                'current': [
                    'temperature_2m',
                    'relative_humidity_2m',
                    'apparent_temperature',
                    'weather_code',
                    'wind_speed_10m',
                    'wind_direction_10m',
                    'precipitation',
                    'pressure_msl',
                    'surface_pressure',
                    'visibility',
                    'uv_index',
                    'is_day'
                ],
                'hourly': [
                    'temperature_2m',
                    'weather_code',
                    'precipitation_probability',
                    'precipitation',
                    'visibility',
                    'uv_index'
                ],
                'daily': [
                    'weather_code',
                    'temperature_2m_max',
                    'temperature_2m_min',
                    'precipitation_sum',
                    'precipitation_probability_max',
                    'precipitation_hours',
                    'sunrise',
                    'sunset',
                    'uv_index_max',
                    'wind_speed_10m_max',
                    'wind_direction_10m_dominant'
                ],
                'forecast_days': '10',
                'past_hours': '1'
            }
            
            # Convert list parameters to comma-separated strings
            for key in ['current', 'hourly', 'daily']:
                if isinstance(params[key], list):
                    params[key] = ','.join(params[key])
            
            response = requests.get(
                self.forecast_url, 
                params=params, 
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json()
            
        except requests.exceptions.Timeout:
            return {"error": "Request timed out. Please try again."}
        except requests.exceptions.RequestException as e:
            return {"error": f"Forecast service unavailable: {str(e)}"}
    
    def get_air_quality(self, latitude, longitude, timezone='auto'):
        """
        Get air quality data for given coordinates
        
        Args:
            latitude (str/float): Latitude coordinate
            longitude (str/float): Longitude coordinate
            timezone (str): Timezone (default: 'auto')
            
        Returns:
            dict: Air quality metrics or error
        """
        try:
            params = {
                'latitude': latitude,
                'longitude': longitude,
                'timezone': timezone,
                'hourly': 'european_aqi,pm10,pm2_5,nitrogen_dioxide'
            }
            
            response = requests.get(
                self.aqi_url, 
                params=params, 
                timeout=self.timeout
            )
            response.raise_for_status()
            data = response.json()
            
            # Extract latest values
            hourly = data.get('hourly', {})
            times = hourly.get('time', [])
            
            if not times:
                return {"error": "No air quality data available for this location"}
            
            latest_index = len(times) - 1
            
            # Safely extract values with proper checks
            aqi_values = hourly.get('european_aqi', [])
            pm25_values = hourly.get('pm2_5', [])
            pm10_values = hourly.get('pm10', [])
            no2_values = hourly.get('nitrogen_dioxide', [])
            
            return {
                "index": round(aqi_values[latest_index]) if aqi_values and len(aqi_values) > latest_index and aqi_values[latest_index] is not None else None,
                "pm25": round(pm25_values[latest_index]) if pm25_values and len(pm25_values) > latest_index and pm25_values[latest_index] is not None else None,
                "pm10": round(pm10_values[latest_index]) if pm10_values and len(pm10_values) > latest_index and pm10_values[latest_index] is not None else None,
                "no2": round(no2_values[latest_index]) if no2_values and len(no2_values) > latest_index and no2_values[latest_index] is not None else None,
                "timestamp": times[latest_index]
            }
            
        except requests.exceptions.Timeout:
            return {"error": "Request timed out. Please try again."}
        except requests.exceptions.RequestException as e:
            return {"error": f"Air quality service unavailable: {str(e)}"}
    
    def get_complete_weather(self, city_name):
        """
        Get complete weather data (geocoding + forecast + AQI) for a city
        
        Args:
            city_name (str): Name of the city
            
        Returns:
            dict: Complete weather information or error
        """
        # Step 1: Geocode the city
        city_data = self.geocode_city(city_name)
        if city_data.get('error'):
            return city_data
        
        lat = city_data['latitude']
        lon = city_data['longitude']
        tz = city_data.get('timezone', 'auto')
        
        # Step 2: Get forecast
        forecast_data = self.get_forecast(lat, lon, tz)
        if forecast_data.get('error'):
            return forecast_data
        
        # Step 3: Get air quality
        aqi_data = self.get_air_quality(lat, lon, tz)
        # Don't fail if AQI is unavailable, just set it to None
        if aqi_data.get('error'):
            aqi_data = None
        
        return {
            "city": {
                "name": city_data.get('name'),
                "country": city_data.get('country'),
                "latitude": lat,
                "longitude": lon,
                "timezone": tz
            },
            "forecast": forecast_data,
            "air_quality": aqi_data
        }
