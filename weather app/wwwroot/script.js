// script.js - Weather App Frontend Logic
// Change this to 'http://localhost:5000/api' if using Live Server, or 'window.location.origin + '/api'' for Flask server
const API_BASE = window.location.origin.includes('5500') ? 'http://localhost:5000/api' : window.location.origin + '/api';

// State
let currentCity = null;
let currentUnit = 'C';
let currentTheme = 'light';

// Elements
const themeBtn = document.getElementById('themeBtn');
const unitBtn = document.getElementById('unitBtn');
const searchForm = document.getElementById('searchForm');
const cityInput = document.getElementById('cityInput');
const msgDiv = document.getElementById('msg');
const chatForm = document.getElementById('chatForm');
const chatInput = document.getElementById('chatInput');
const chatBox = document.getElementById('chatBox');

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    // Theme toggle
    themeBtn.addEventListener('click', toggleTheme);
    
    // Unit toggle
    unitBtn.addEventListener('click', toggleUnit);
    
    // Search form
    searchForm.addEventListener('submit', handleSearch);
    
    // Chat form
    chatForm.addEventListener('submit', handleChat);
    
    // Load default city and remove offline message
    msgDiv.style.display = 'none';
    searchCity('London');
});

function toggleTheme() {
    currentTheme = currentTheme === 'light' ? 'dark' : 'light';
    document.body.className = currentTheme;
    themeBtn.textContent = currentTheme === 'light' ? 'Light' : 'Dark';
}

function toggleUnit() {
    currentUnit = currentUnit === 'C' ? 'F' : 'C';
    unitBtn.textContent = '°' + currentUnit;
    
    // Refresh display with new unit
    if (currentCity) {
        updateDisplay(currentCity);
    }
}

async function handleSearch(e) {
    e.preventDefault();
    const city = cityInput.value.trim();
    
    if (city.length < 2) {
        showMessage('City name too short', 'error');
        return;
    }
    
    await searchCity(city);
}

async function searchCity(cityName) {
    showMessage('Loading...', 'info');
    
    try {
        const url = `${API_BASE}/weather?city=${encodeURIComponent(cityName)}`;
        console.log('API_BASE:', API_BASE);
        console.log('Fetching:', url);
        const response = await fetch(url);
        
        console.log('Response status:', response.status);
        console.log('Response ok:', response.ok);
        
        if (!response.ok) {
            const errorData = await response.json();
            console.error('API Error:', errorData);
            showMessage(errorData.error || 'Failed to load weather data', 'error');
            return;
        }
        
        const data = await response.json();
        console.log('Raw received data:', JSON.stringify(data, null, 2));
        console.log('data.city exists:', !!data.city);
        console.log('data.error exists:', !!data.error);
        
        if (data.error) {
            console.error('Data has error:', data.error);
            showMessage(data.error || 'City not found', 'error');
            return;
        }
        
        if (!data.city) {
            console.error('No city in data');
            showMessage('City not found', 'error');
            return;
        }
        
        // Transform C# API response to match expected format
        const transformedData = {
            location: data.city,
            forecast: {
                current: data.current,
                hourly: data.hourly,
                daily: data.daily,
                current_units: data.current_units,
                hourly_units: data.hourly_units,
                daily_units: data.daily_units
            },
            air_quality: data.air_quality ? {
                index: data.air_quality.european_aqi,
                pm25: data.air_quality.pm2_5,
                pm10: data.air_quality.pm10,
                no2: data.air_quality.nitrogen_dioxide
            } : null
        };
        
        currentCity = transformedData;
        console.log('Updating display with:', transformedData);
        updateDisplay(transformedData);
        showMessage('', '');
        
    } catch (error) {
        console.error('Fetch error:', error);
        showMessage('Failed to fetch weather data. Please check your connection.', 'error');
    }
}

function updateDisplay(data) {
    console.log('updateDisplay called with:', data);
    const { location, forecast, air_quality } = data;
    
    // Update city info
    if (location) {
        console.log('Updating city to:', location.name);
        document.querySelector('.title').textContent = `${location.name}, ${location.country}`;
    }
    
    // The API returns forecast data directly, not nested under forecast.current
    if (forecast && forecast.current) {
        console.log('Updating current weather with:', forecast.current);
        updateCurrentWeather(forecast.current, forecast.current_units);
        updateDetailedConditions(forecast);
    } else {
        console.error('No forecast.current data found!', forecast);
    }
    
    if (forecast && forecast.hourly) {
        console.log('Updating hourly forecast');
        updateHourlyForecast(forecast.hourly, forecast.hourly_units);
    }
    
    if (forecast && forecast.daily) {
        console.log('Updating daily forecast');
        updateForecast(forecast.daily, forecast.daily_units);
    }
    
    if (air_quality) {
        console.log('Updating air quality');
        updateAirQuality(air_quality);
    }
}

function updateCurrentWeather(current, units) {
    // Temperature
    const temp = convertTemp(current.temperature_2m);
    const feels = convertTemp(current.apparent_temperature);
    
    document.getElementById('currentTemp').textContent = `${Math.round(temp)}°${currentUnit}`;
    document.getElementById('feels').textContent = `Feels like ${Math.round(feels)}°${currentUnit}`;
    
    // Weather condition
    const weatherDesc = getWeatherDescription(current.weather_code);
    document.getElementById('nowText').textContent = weatherDesc;
    
    // Weather icons
    const iconName = getWeatherIcon(current.weather_code);
    document.getElementById('iconLeft').src = `images/${iconName}`;
    document.getElementById('iconRight').src = `images/${iconName}`;
    
    // Humidity and wind
    document.getElementById('humidity').textContent = `${current.relative_humidity_2m}%`;
    document.getElementById('wind').textContent = `${Math.round(current.wind_speed_10m)} km/h`;
}

function updateForecast(daily, units) {
    const forecastDiv = document.getElementById('forecast');
    forecastDiv.innerHTML = '';
    
    const times = daily.time;
    const maxTemps = daily.temperature_2m_max;
    const minTemps = daily.temperature_2m_min;
    const codes = daily.weather_code;
    const precipProb = daily.precipitation_probability_max || [];
    
    for (let i = 0; i < Math.min(10, times.length); i++) {
        const date = new Date(times[i]);
        const dayName = i === 0 ? 'Today' : date.toLocaleDateString('en-US', { weekday: 'short' });
        
        const maxTemp = convertTemp(maxTemps[i]);
        const minTemp = convertTemp(minTemps[i]);
        const iconName = getWeatherIcon(codes[i]);
        const precip = precipProb[i] || 0;
        
        const dayCard = document.createElement('div');
        dayCard.className = 'forecast-day';
        dayCard.innerHTML = `
            <div class="day-name">${dayName}</div>
            <img src="images/${iconName}" class="wx-icon" alt="weather" />
            <div class="temp-range">
                <span class="max">${Math.round(maxTemp)}°</span>
                <span class="min">${Math.round(minTemp)}°</span>
            </div>
            ${precip > 0 ? `<div class="precip-prob">💧 ${Math.round(precip)}%</div>` : ''}
        `;
        
        forecastDiv.appendChild(dayCard);
    }
}

function updateHourlyForecast(hourly, units) {
    const hourlyDiv = document.getElementById('hourlyForecast');
    hourlyDiv.innerHTML = '';
    
    const times = hourly.time;
    const temps = hourly.temperature_2m;
    const codes = hourly.weather_code;
    const precipProb = hourly.precipitation_probability || [];
    
    const now = new Date();
    const currentHour = now.getHours();
    
    for (let i = 0; i < Math.min(24, times.length); i++) {
        const time = new Date(times[i]);
        const hour = time.getHours();
        const timeStr = i === 0 ? 'Now' : time.toLocaleTimeString('en-US', { hour: 'numeric', hour12: true });
        
        const temp = convertTemp(temps[i]);
        const iconName = getWeatherIcon(codes[i]);
        const precip = precipProb[i] || 0;
        
        const hourlyItem = document.createElement('div');
        hourlyItem.className = 'hourly-item';
        hourlyItem.innerHTML = `
            <div class="hourly-time">${timeStr}</div>
            <img src="images/${iconName}" class="hourly-icon wx-icon" alt="weather" />
            <div class="hourly-temp">${Math.round(temp)}°</div>
            ${precip > 10 ? `<div class="hourly-precip">💧 ${Math.round(precip)}%</div>` : ''}
        `;
        
        hourlyDiv.appendChild(hourlyItem);
    }
}

function updateDetailedConditions(forecast) {
    const current = forecast.current;
    const daily = forecast.daily;
    
    // Sunrise/Sunset
    if (daily && daily.sunrise && daily.sunrise[0]) {
        const sunrise = new Date(daily.sunrise[0]);
        const sunset = new Date(daily.sunset[0]);
        document.getElementById('sunrise').textContent = sunrise.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true });
        document.getElementById('sunset').textContent = sunset.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true });
    }
    
    // UV Index
    const uvIndex = current.uv_index !== undefined ? current.uv_index : '—';
    const uvCategory = getUVCategory(current.uv_index);
    document.getElementById('uvIndex').textContent = uvIndex !== '—' ? `${Math.round(uvIndex)} (${uvCategory})` : '—';
    
    // Wind
    const windSpeed = Math.round(current.wind_speed_10m || 0);
    const windDir = getWindDirection(current.wind_direction_10m);
    document.getElementById('windDetail').textContent = `${windSpeed} km/h ${windDir}`;
    
    // Precipitation
    const precip = current.precipitation || 0;
    document.getElementById('precipitation').textContent = precip > 0 ? `${precip.toFixed(1)} mm` : 'None';
    
    // Visibility
    const visibility = current.visibility ? Math.round(current.visibility / 1000) : '—';
    document.getElementById('visibility').textContent = visibility !== '—' ? `${visibility} km` : '—';
    
    // Pressure
    const pressure = current.pressure_msl || current.surface_pressure;
    document.getElementById('pressure').textContent = pressure ? `${Math.round(pressure)} hPa` : '—';
    
    // Humidity
    document.getElementById('humidityDetail').textContent = `${current.relative_humidity_2m}%`;
}

function getUVCategory(uv) {
    if (uv === undefined || uv === null) return 'Unknown';
    if (uv <= 2) return 'Low';
    if (uv <= 5) return 'Moderate';
    if (uv <= 7) return 'High';
    if (uv <= 10) return 'Very High';
    return 'Extreme';
}

function getWindDirection(degrees) {
    if (degrees === undefined || degrees === null) return '';
    const dirs = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
    const index = Math.round(degrees / 45) % 8;
    return dirs[index];
}

function updateAirQuality(aqi) {
    if (!aqi) {
        document.getElementById('aqiIndex').textContent = '—';
        document.getElementById('aqiLabel').textContent = 'No data';
        document.getElementById('pm25').textContent = '—';
        document.getElementById('pm10').textContent = '—';
        document.getElementById('no2').textContent = '—';
        return;
    }
    
    document.getElementById('aqiIndex').textContent = aqi.index !== null && aqi.index !== undefined ? aqi.index : '—';
    document.getElementById('aqiLabel').textContent = categorizeAQI(aqi.index);
    document.getElementById('pm25').textContent = aqi.pm25 !== null && aqi.pm25 !== undefined ? aqi.pm25 : '—';
    document.getElementById('pm10').textContent = aqi.pm10 !== null && aqi.pm10 !== undefined ? aqi.pm10 : '—';
    document.getElementById('no2').textContent = aqi.no2 !== null && aqi.no2 !== undefined ? aqi.no2 : '—';
}

function handleChat(e) {
    e.preventDefault();
    const question = chatInput.value.trim().toLowerCase();
    
    if (!currentCity) {
        addChatMessage('bot', 'Please search for a city first!');
        return;
    }
    
    addChatMessage('user', chatInput.value);
    chatInput.value = '';
    
    // Simple chatbot responses
    let answer = '';
    
    if (question.includes('wear') || question.includes('outfit')) {
        const temp = currentCity.forecast?.current?.temperature_2m;
        if (temp < 10) answer = '🧥 Wear a warm jacket, it\'s cold!';
        else if (temp < 20) answer = '👕 A light jacket should be fine.';
        else answer = '👚 Light clothes are perfect!';
    } else if (question.includes('rain')) {
        const code = currentCity.forecast?.current?.weather_code;
        answer = (code >= 51 && code <= 82) ? '🌧️ Yes, it might rain.' : '☀️ No rain expected.';
    } else if (question.includes('wind')) {
        const wind = currentCity.forecast?.current?.wind_speed_10m;
        answer = `💨 Wind speed is ${Math.round(wind)} km/h`;
    } else if (question.includes('aqi') || question.includes('air quality')) {
        const aqi = currentCity.air_quality?.index;
        answer = aqi ? `🌫️ Air quality index is ${aqi} (${categorizeAQI(aqi)})` : '❓ No air quality data available';
    } else if (question.includes('temp') || question.includes('hot') || question.includes('cold')) {
        const temp = convertTemp(currentCity.forecast?.current?.temperature_2m);
        answer = `🌡️ Current temperature is ${Math.round(temp)}°${currentUnit}`;
    } else {
        answer = '🤔 Try asking: "What should I wear?", "Is it rainy?", "How windy?", or "What\'s the AQI?"';
    }
    
    setTimeout(() => addChatMessage('bot', answer), 500);
}

function addChatMessage(type, text) {
    const bubble = document.createElement('div');
    bubble.className = `bubble ${type}`;
    bubble.textContent = text;
    chatBox.appendChild(bubble);
    chatBox.scrollTop = chatBox.scrollHeight;
}

// Helper functions
function convertTemp(celsius) {
    return currentUnit === 'F' ? (celsius * 9/5 + 32) : celsius;
}

function getWeatherDescription(code) {
    const descriptions = {
        0: 'Clear', 1: 'Mainly clear', 2: 'Partly cloudy', 3: 'Overcast',
        45: 'Fog', 48: 'Rime fog',
        51: 'Light drizzle', 53: 'Drizzle', 55: 'Heavy drizzle',
        61: 'Light rain', 63: 'Rain', 65: 'Heavy rain',
        71: 'Light snow', 73: 'Snow', 75: 'Heavy snow',
        80: 'Light showers', 81: 'Showers', 82: 'Heavy showers',
        95: 'Thunderstorm', 96: 'Thunderstorm with hail', 99: 'Severe thunderstorm'
    };
    return descriptions[code] || 'Unknown';
}

function getWeatherIcon(code) {
    if (code === 0 || code === 1) return 'sun-smile.svg';
    if (code === 2 || code === 3) return 'cloud.svg';
    if (code >= 45 && code <= 48) return 'cloud.svg';
    if (code >= 51 && code <= 82) return 'cloud-rain.svg';
    if (code >= 71 && code <= 77) return 'cloud.svg';
    if (code >= 95) return 'cloud-rain.svg';
    return 'sun-smile.svg';
}

function categorizeAQI(index) {
    if (!index) return 'Unknown';
    if (index <= 20) return 'Very Low';
    if (index <= 40) return 'Low';
    if (index <= 60) return 'Medium';
    if (index <= 80) return 'High';
    return 'Very High';
}

function showMessage(text, type) {
    msgDiv.textContent = text;
    msgDiv.className = type === 'error' ? 'offline-hint error' : 'offline-hint';
    msgDiv.style.display = text ? 'block' : 'none';
}
