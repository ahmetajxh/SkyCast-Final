# SkyCast Weather App 🌤️

A modern weather application built with **ASP.NET Core 10.0**. Features real-time weather data, forecasts, air quality monitoring, and an interactive chatbot - all powered by C#.

---

## ✨ Features

- 🔍 City search with real-time geocoding
- 🌡️ Current weather conditions (temperature, humidity, wind, pressure)
- 📅 7-day weather forecast
- ⏰ 24-hour hourly forecast
- 🌫️ Air quality monitoring (EU AQI, PM2.5, PM10, NO₂)
- 💬 Interactive weather chatbot
- 🎨 Light/Dark theme toggle
- 🌡️ Temperature unit switching (°C/°F)
- 📱 Responsive, modern design
- ⚡ High-performance C# backend

---

## 🛠️ Tech Stack

**Backend:**
- **ASP.NET Core 10.0 (C#)** - MVC Controllers-based Web API
- Open-Meteo API integration (weather, geocoding, air quality)
- Built-in C# utilities for validation and formatting
- 9 separate controller classes for organized API endpoints

**Frontend:**
- HTML5, CSS3 (with gradients & animations)
- Vanilla JavaScript (ES6+)
- Served directly by ASP.NET Core

---

## 📁 Project Structure

```
weather app/
├── WeatherAPIProgram.cs      # Main ASP.NET Core application
├── WeatherAPI.csproj          # ASP.NET Web project file
├── Controllers/               # MVC Controller classes
│   ├── HealthController.cs   # Health check endpoint
│   ├── WeatherController.cs  # Combined weather data
│   ├── GeocodingController.cs# City geocoding
│   ├── ForecastController.cs # Weather forecast
│   ├── AirQualityController.cs# Air quality data
│   ├── ValidationController.cs# Input validation
│   ├── TemperatureController.cs# Temperature utilities
│   ├── WindController.cs     # Wind utilities
│   └── AqiController.cs      # AQI categorization
├── WeatherUtils.cs            # C# utility classes
├── Program.cs                 # CLI utility tool
├── WeatherAppUtils.csproj     # CLI utility project
├── wwwroot/                   # Static web files
│   ├── index.html            # Main webpage
│   ├── script.js             # Frontend JavaScript
│   ├── style.css             # Styles
│   └── images/               # Weather icons
├── bin/                       # Compiled binaries
└── obj/                       # Build artifacts
```

---

## 🚀 Quick Start

### Prerequisites
- **.NET SDK 10.0** or later ([Download](https://dotnet.microsoft.com/download))
- Windows, macOS, or Linux

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ahmetajxh/SkyCast-Final.git
   cd "SkyCast-Final/weather app"
   ```

2. **Build the project:**
   ```bash
   dotnet build WeatherAPI.csproj -c Release
   ```

3. **Run the application:**
   ```bash
   dotnet run --project WeatherAPI.csproj --configuration Release --no-build
   ```

4. **Access the app:**
   - **Website:** http://localhost:8000
   - **API Health:** http://localhost:8000/api/health

---

## 🌐 API Endpoints

### Weather Endpoints
```
GET /api/health                              - Health check
GET /api/weather?city={city}                 - Complete weather data for a city
GET /api/geocode?name={city}                 - Search for city coordinates
GET /api/forecast?lat={lat}&lon={lon}        - Weather forecast by coordinates
GET /api/air-quality?lat={lat}&lon={lon}     - Air quality data
```

### Utility Endpoints
```
GET /api/validate/city?name={name}           - Validate city name
GET /api/wind/direction?degrees={deg}        - Get wind direction (N, NE, E, etc.)
GET /api/aqi/categorize?index={aqi}          - Categorize air quality index
```

### Example Usage
```bash
# Get weather for a city
curl "http://localhost:8000/api/weather?city=London"

# Get wind direction
curl "http://localhost:8000/api/wind/direction?degrees=270"

# Validate city name
curl "http://localhost:8000/api/validate/city?name=London"

# Categorize AQI
curl "http://localhost:8000/api/aqi/categorize?index=65"
```

---

## ⚡ C# Utilities

The project includes high-performance C# utility classes:

### ValidationUtils
- City name validation (length, character checks)
- Coordinate validation (latitude/longitude bounds)

### TemperatureUtils  
- Celsius ↔ Fahrenheit conversion
- Temperature formatting with units

### WindUtils
- Degrees to cardinal direction conversion (N, NE, E, SE, S, SW, W, NW)

### AQIUtils
- Air quality index categorization (Excellent, Good, Moderate, Poor, Very Poor)
- Health recommendations based on AQI levels

---

## 🔧 CLI Utility Tool

A command-line interface is also available for standalone utility operations:

```bash
# Validate city name
./bin/Release/net10.0/WeatherAppUtils.exe validate-city "London"

# Get wind direction
./bin/Release/net10.0/WeatherAppUtils.exe wind-direction 45

# Categorize AQI
./bin/Release/net10.0/WeatherAppUtils.exe categorize-aqi 65

# Format temperature
./bin/Release/net10.0/WeatherAppUtils.exe format-temp 25 F
```

---

## 🎨 Frontend Features

- **Dynamic Weather Display** - Real-time data updates
- **7-Day Forecast Cards** - Visual forecast presentation
- **Hourly Temperature Chart** - 24-hour trends
- **Air Quality Widget** - PM2.5, PM10, NO₂ levels
- **Theme Toggle** - Light/Dark mode with smooth transitions
- **Unit Conversion** - Seamless °C/°F switching
- **Weather Chatbot** - Natural language weather queries
- **Responsive Design** - Works on desktop and mobile

---

## 💻 Development

### Build from Source
```bash
# Clean previous builds
dotnet clean WeatherAPI.csproj

# Build in release mode
dotnet build WeatherAPI.csproj -c Release

# Run without rebuilding
dotnet run --project WeatherAPI.csproj --configuration Release --no-build
```

### Build CLI Utilities
```bash
dotnet build WeatherAppUtils.csproj -c Release
```

### Testing Endpoints
```bash
# Test health endpoint
curl http://localhost:8000/api/health

# Test weather endpoint
curl "http://localhost:8000/api/weather?city=London"

# Test geocoding
curl "http://localhost:8000/api/geocode?name=Paris"

# Test air quality
curl "http://localhost:8000/api/air-quality?lat=51.5074&lon=-0.1278"
```

---

## 🐛 Troubleshooting

### .NET Not Found

**Error:** `dotnet is not recognized`

**Solution:**
```bash
# Add to PATH (PowerShell)
$env:Path += ";C:\Program Files\dotnet"

# Or install .NET SDK from:
# https://dotnet.microsoft.com/download
```

### Port Already in Use

**Error:** `Address already in use`

**Solution:**
```bash
# Find process using port 8000
netstat -ano | findstr :8000

# Kill the process (replace PID with actual process ID)
taskkill /PID <process_id> /F
```

### Build Errors

If you encounter build errors, clean and rebuild:
```bash
dotnet clean WeatherAPI.csproj
dotnet build WeatherAPI.csproj -c Release
```

### wwwroot Not Found

If static files aren't loading, ensure the wwwroot folder exists:
```bash
# The wwwroot folder should contain:
# - index.html
# - script.js
# - style.css
# - images/ folder
```

---

## 📊 Architecture

The application uses ASP.NET Core MVC Controllers architecture:

1. **MVC Controllers** - 9 separate controller classes handle API endpoints
2. **ASP.NET Core Web API** - Handles HTTP requests and routing
3. **Static File Middleware** - Serves HTML/CSS/JS from wwwroot
4. **HttpClient Factory** - Calls external Open-Meteo APIs
5. **C# Utilities** - Validation and formatting logic
6. **JSON Serialization** - Data transformation

### Controller Organization
- **HealthController** - System health monitoring
- **WeatherController** - Combined weather endpoint
- **GeocodingController** - City search and coordinates
- **ForecastController** - Weather forecast data
- **AirQualityController** - Air pollution metrics
- **ValidationController** - Input validation
- **TemperatureController** - Temperature conversions
- **WindController** - Wind direction utilities
- **AqiController** - Air quality categorization

All components run in a single process on port 8000.

---

## 🌍 Data Sources

- **Open-Meteo Weather API** - Weather forecasts
- **Open-Meteo Geocoding API** - City search
- **Open-Meteo Air Quality API** - Air pollution data

All APIs are free and require no API keys.
