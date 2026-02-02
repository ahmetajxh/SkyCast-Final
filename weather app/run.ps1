# Quick Launch Script for Weather App
# Run this in PowerShell

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Weather App Launcher" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Select an option:" -ForegroundColor Yellow
Write-Host "1. Run Flask App (Main - Port 8000)" -ForegroundColor Green
Write-Host "2. Run ASP.NET API (Port 5000)" -ForegroundColor Green
Write-Host "3. Run Both Servers" -ForegroundColor Green
Write-Host ""

$choice = Read-Host "Enter choice (1-3)"

Set-Location "c:\Users\User\Desktop\Final Weather App\weather app"

switch ($choice) {
    "1" {
        Write-Host "`nStarting Flask App..." -ForegroundColor Cyan
        Write-Host "Open browser to: http://localhost:8000`n" -ForegroundColor Yellow
        .\.venv\Scripts\python.exe app.py
    }
    "2" {
        Write-Host "`nStarting ASP.NET API..." -ForegroundColor Cyan
        Write-Host "API: http://localhost:5000" -ForegroundColor Yellow
        Write-Host "Swagger: http://localhost:5000/swagger`n" -ForegroundColor Yellow
        dotnet run --project WeatherAPI.csproj
    }
    "3" {
        Write-Host "`nStarting Both Servers..." -ForegroundColor Cyan
        Write-Host "Flask will start in this window" -ForegroundColor Yellow
        Write-Host "ASP.NET will start in new window`n" -ForegroundColor Yellow
        
        # Start ASP.NET in new window
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\User\Desktop\Final Weather App\weather app'; Write-Host 'ASP.NET API Starting...' -ForegroundColor Cyan; dotnet run --project WeatherAPI.csproj"
        
        # Wait a moment
        Start-Sleep -Seconds 2
        
        # Start Flask in current window
        Write-Host "Starting Flask..." -ForegroundColor Cyan
        Write-Host "Flask: http://localhost:8000" -ForegroundColor Yellow
        Write-Host "ASP.NET: http://localhost:5000`n" -ForegroundColor Yellow
        .\.venv\Scripts\python.exe app.py
    }
    default {
        Write-Host "Invalid choice. Exiting." -ForegroundColor Red
    }
}
