# Flask Environment Configuration
# Copy this file to .env and update with your values

# Flask Environment
FLASK_ENV=development
FLASK_DEBUG=True

# Secret Key (generate a random string for production)
SECRET_KEY=change-this-to-random-secret-key-in-production

# Server Configuration
HOST=0.0.0.0
PORT=5000

# CORS Origins (comma-separated list)
# Use * for development, specific origins for production
CORS_ORIGINS=*

# API Timeout (seconds)
API_TIMEOUT=10