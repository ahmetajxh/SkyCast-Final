# config.py - Application Configuration
import os

class Config:
    """Base configuration"""
    SECRET_KEY = os.environ.get('SECRET_KEY') or 'dev-secret-key-change-in-production'
    
    # API URLs
    GEOCODING_API_URL = 'https://geocoding-api.open-meteo.com/v1/search'
    FORECAST_API_URL = 'https://api.open-meteo.com/v1/forecast'
    AIR_QUALITY_API_URL = 'https://air-quality-api.open-meteo.com/v1/air-quality'
    
    # API Settings
    API_TIMEOUT = 10  # seconds
    MAX_FORECAST_DAYS = 7
    
    # CORS Settings
    CORS_ORIGINS = ['*']  # Change to specific origins in production

class DevelopmentConfig(Config):
    """Development configuration"""
    DEBUG = True

class ProductionConfig(Config):
    """Production configuration"""
    DEBUG = False
    # Add production-specific settings here
