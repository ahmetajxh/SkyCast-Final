# C# Utilities Bridge
# This module provides Python wrappers to call C# utility functions
# The C# library handles validation and formatting with better performance

import subprocess
import json
import os
from pathlib import Path

class CSharpUtilsBridge:
    """
    Bridge to use C# utilities from Python
    Provides optional C#-based validation and formatting
    Falls back to Python implementations if C# is not available
    """
    
    def __init__(self):
        self.csharp_exe = self._find_csharp_exe()
        self.csharp_available = self.csharp_exe is not None
        
    def _find_csharp_exe(self):
        """Find the compiled C# executable"""
        base_dir = Path(__file__).parent
        
        # Check for .NET 10.0 build first (current version)
        exe_path = base_dir / "bin" / "Release" / "net10.0" / "WeatherAppUtils.exe"
        if exe_path.exists():
            return str(exe_path)
        
        # Fallback to .NET 8.0
        exe_path = base_dir / "bin" / "Release" / "net8.0" / "WeatherAppUtils.exe"
        if exe_path.exists():
            return str(exe_path)
            
        return None
    
    def call_csharp_utility(self, command, *args):
        """
        Call the C# utility executable
        
        Args:
            command (str): Command to execute
            *args: Arguments for the command
            
        Returns:
            str or None: Output from the C# utility, or None if not available
        """
        if not self.csharp_available:
            return None
            
        try:
            result = subprocess.run(
                [self.csharp_exe, command] + list(args),
                capture_output=True,
                text=True,
                timeout=5
            )
            
            if result.returncode == 0:
                return result.stdout.strip()
            return None
        except Exception:
            return None
    
    def validate_city_name_csharp(self, city_name):
        """
        Validate city name using C# implementation
        More efficient for bulk operations
        
        Args:
            city_name (str): City name to validate
            
        Returns:
            dict: {"valid": bool, "error": str or None}
        """
        output = self.call_csharp_utility("validate-city", city_name)
        if output:
            try:
                return json.loads(output)
            except json.JSONDecodeError:
                pass
        
        # Fallback to Python validation
        return self._validate_city_python(city_name)
    
    def _validate_city_python(self, city_name):
        """Python fallback for city validation"""
        if not city_name or not city_name.strip():
            return {"valid": False, "error": "City name is required"}
        if len(city_name) < 2:
            return {"valid": False, "error": "City name must be at least 2 characters"}
        if len(city_name) > 100:
            return {"valid": False, "error": "City name is too long"}
        return {"valid": True, "error": None}
    
    @staticmethod
    def get_wind_direction(degrees):
        """
        Get wind direction name from degrees using C# implementation
        
        Args:
            degrees (float): Wind direction in degrees
            
        Returns:
            str: Cardinal direction (N, NE, E, etc.)
        """
        # Direct calculation here since it's simple
        degrees = degrees % 360
        
        directions = [
            "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
            "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"
        ]
        
        index = round((degrees % 360) / 22.5) % 16
        return directions[index]
    
    @staticmethod
    def celsius_to_fahrenheit(celsius):
        """Convert Celsius to Fahrenheit"""
        return celsius * 9.0 / 5.0 + 32
    
    @staticmethod
    def categorize_aqi(aqi_index):
        """
        Categorize AQI index using C# logic
        
        Args:
            aqi_index (int): European AQI index value
            
        Returns:
            str: Category description
        """
        if aqi_index is None:
            return "Unknown"
        
        if aqi_index <= 20:
            return "Excellent"
        elif aqi_index <= 40:
            return "Good"
        elif aqi_index <= 60:
            return "Moderate"
        elif aqi_index <= 80:
            return "Poor"
        else:
            return "Very Poor"
    
    @staticmethod
    def get_aqi_recommendation(aqi_index):
        """
        Get health recommendation based on AQI using C# logic
        
        Args:
            aqi_index (int): European AQI index value
            
        Returns:
            str: Health recommendation
        """
        if aqi_index is None:
            return "Unable to determine air quality"
        
        if aqi_index <= 20:
            return "Good conditions for outdoor activities"
        elif aqi_index <= 40:
            return "Good air quality overall"
        elif aqi_index <= 60:
            return "Sensitive groups may experience effects"
        elif aqi_index <= 80:
            return "Members of general public may feel effects"
        else:
            return "Everyone may begin to feel effects. Limit outdoor activities"


# Create singleton instance
csharp_bridge = CSharpUtilsBridge()
