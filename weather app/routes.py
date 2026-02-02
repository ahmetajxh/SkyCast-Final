# routes.py - API Route Handlers
from flask import Blueprint, jsonify, request
from services import WeatherService
from utils import validate_coordinates, validate_city_name
from datetime import datetime

weather_bp = Blueprint('weather', __name__)
weather_service = WeatherService()

@weather_bp.route('/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    return jsonify({
        "status": "ok",
        "timestamp": datetime.now().isoformat(),
        "service": "Weather API"
    })

@weather_bp.route('/test', methods=['GET'])
def test():
    """Test endpoint - Tests the geocoding service"""
    try:
        from services import WeatherService
        ws = WeatherService()
        # Test multiple cities
        test_cities = ['London', 'Paris', 'Tirana', 'New York']
        results = {}
        for city in test_cities:
            results[city] = ws.geocode_city(city)
        return jsonify({"test": "success", "data": results})
    except Exception as e:
        return jsonify({"test": "failed", "error": str(e)}), 500

@weather_bp.route('/geocode', methods=['GET'])
def geocode_city():
    """
    Search for a city by name and return coordinates
    Query params: name (required)
    Example: /api/geocode?name=London
    """
    city_name = request.args.get('name', '').strip()
    
    # Validate input
    is_valid, error_msg = validate_city_name(city_name)
    if not is_valid:
        return jsonify({"error": error_msg}), 400
    
    # Call service
    result = weather_service.geocode_city(city_name)
    
    if result.get('error'):
        status_code = 404 if 'not found' in result['error'].lower() else 503
        return jsonify(result), status_code
    
    return jsonify(result)

@weather_bp.route('/forecast', methods=['GET'])
def get_forecast():
    """
    Get weather forecast for coordinates
    Query params: lat, lon, timezone (optional)
    Example: /api/forecast?lat=51.5074&lon=-0.1278&timezone=Europe/London
    """
    latitude = request.args.get('lat')
    longitude = request.args.get('lon')
    timezone = request.args.get('timezone', 'auto')
    
    # Validate coordinates
    is_valid, error_msg = validate_coordinates(latitude, longitude)
    if not is_valid:
        return jsonify({"error": error_msg}), 400
    
    # Call service
    result = weather_service.get_forecast(latitude, longitude, timezone)
    
    if result.get('error'):
        return jsonify(result), 503
    
    return jsonify(result)

@weather_bp.route('/air-quality', methods=['GET'])
def get_air_quality():
    """
    Get air quality data for coordinates
    Query params: lat, lon, timezone (optional)
    Example: /api/air-quality?lat=51.5074&lon=-0.1278
    """
    latitude = request.args.get('lat')
    longitude = request.args.get('lon')
    timezone = request.args.get('timezone', 'auto')
    
    # Validate coordinates
    is_valid, error_msg = validate_coordinates(latitude, longitude)
    if not is_valid:
        return jsonify({"error": error_msg}), 400
    
    # Call service
    result = weather_service.get_air_quality(latitude, longitude, timezone)
    
    if result.get('error'):
        status_code = 404 if 'no data' in result['error'].lower() else 503
        return jsonify(result), status_code
    
    return jsonify(result)

@weather_bp.route('/weather', methods=['GET'])
def get_complete_weather():
    """
    Get complete weather data (geocoding + forecast + AQI) in one call
    Query params: city (required)
    Example: /api/weather?city=London
    """
    try:
        city_name = request.args.get('city', '').strip()
        print(f"[DEBUG] Requested city: {city_name}")
        
        # Validate input
        is_valid, error_msg = validate_city_name(city_name)
        if not is_valid:
            print(f"[DEBUG] Validation failed: {error_msg}")
            return jsonify({"error": error_msg}), 400
        
        # Step 1: Geocode the city
        print(f"[DEBUG] Geocoding {city_name}...")
        geo_result = weather_service.geocode_city(city_name)
        print(f"[DEBUG] Geocoding result: {geo_result}")
        
        if geo_result.get('error'):
            print(f"[DEBUG] Geocoding error: {geo_result['error']}")
            return jsonify(geo_result), 404
        
        # Step 2: Get forecast and air quality
        lat = geo_result['latitude']
        lon = geo_result['longitude']
        tz = geo_result.get('timezone', 'auto')
        print(f"[DEBUG] Getting forecast for coordinates: {lat}, {lon}")
        
        forecast = weather_service.get_forecast(lat, lon, tz)
        aqi = weather_service.get_air_quality(lat, lon, tz)
        
        print(f"[DEBUG] Successfully retrieved weather data for {city_name}")
        
        # Combine results
        return jsonify({
            "location": geo_result,
            "forecast": forecast if not forecast.get('error') else None,
            "air_quality": aqi if not aqi.get('error') else None
        })
    except Exception as e:
        print(f"[ERROR] {str(e)}")
        import traceback
        traceback.print_exc()
        return jsonify({"error": f"Server error: {str(e)}"}), 500


