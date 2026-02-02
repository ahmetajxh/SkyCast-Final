# utils.py - Utility Functions and Validators
# Core validation and formatting functions
# Optimized Python implementations with optional C# fallback via csharp_bridge

from csharp_bridge import csharp_bridge

def validate_city_name(city_name):
    """
    Validate city name input
    
    Args:
        city_name (str): City name to validate
        
    Returns:
        tuple: (is_valid, error_message)
    """
    if not city_name:
        return False, "City name is required"
    
    if len(city_name) < 2:
        return False, "City name must be at least 2 characters"
    
    if len(city_name) > 100:
        return False, "City name is too long"
    
    return True, None

def validate_coordinates(latitude, longitude):
    """
    Validate latitude and longitude coordinates
    
    Args:
        latitude (str/float): Latitude value
        longitude (str/float): Longitude value
        
    Returns:
        tuple: (is_valid, error_message)
    """
    if not latitude or not longitude:
        return False, "Latitude and longitude are required"
    
    try:
        lat = float(latitude)
        lon = float(longitude)
        
        if not (-90 <= lat <= 90):
            return False, "Latitude must be between -90 and 90"
        
        if not (-180 <= lon <= 180):
            return False, "Longitude must be between -180 and 180"
        
        return True, None
        
    except (ValueError, TypeError):
        return False, "Invalid coordinate format. Must be numeric values"

def format_temperature(celsius, unit='C'):
    """
    Format temperature in Celsius or Fahrenheit
    
    Args:
        celsius (float): Temperature in Celsius
        unit (str): 'C' or 'F'
        
    Returns:
        str: Formatted temperature string
    """
    if unit == 'F':
        fahrenheit = celsius * 9/5 + 32
        return f"{round(fahrenheit)}°F"
    return f"{round(celsius)}°C"

def categorize_aqi(aqi_index):
    """
    Categorize AQI index into descriptive labels
    Uses C# implementation if available, falls back to Python
    
    Args:
        aqi_index (int): European AQI index value
        
    Returns:
        str: Category description
    """
    return csharp_bridge.categorize_aqi(aqi_index)

def get_weather_description(wmo_code):
    """
    Convert WMO weather code to description
    
    Args:
        wmo_code (int): WMO weather code
        
    Returns:
        str: Weather description
    """
    wmo_codes = {
        0: 'Clear',
        1: 'Mainly clear',
        2: 'Partly cloudy',
        3: 'Overcast',
        45: 'Fog',
        48: 'Rime fog',
        51: 'Drizzle',
        53: 'Drizzle',
        55: 'Drizzle',
        61: 'Rain',
        63: 'Rain',
        65: 'Rain',
        80: 'Showers',
        81: 'Showers',
        82: 'Showers',
        95: 'Thunderstorm',
        96: 'Thunder & hail',
        99: 'Thunder & hail'
    }
    
    return wmo_codes.get(wmo_code, 'Unknown')
