#!/bin/bash
# Setup script for C# utilities in Weather App
# This script builds the C# components on Linux/macOS

echo "========================================"
echo "Weather App - C# Integration Setup"
echo "========================================"
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found!"
    echo "Please install .NET 8.0 from https://dotnet.microsoft.com/download"
    echo ""
    exit 1
fi

echo "✓ .NET SDK found: $(dotnet --version)"
echo ""

# Get the directory of this script
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo "Building C# utilities..."
echo ""

# Restore NuGet packages
echo "Step 1: Restoring NuGet packages..."
dotnet restore WeatherAppUtils.csproj
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to restore packages"
    exit 1
fi
echo "✓ Packages restored"
echo ""

# Build the project
echo "Step 2: Building C# library..."
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi
echo "✓ Build successful"
echo ""

# Publish CLI tool
echo "Step 3: Publishing CLI tool..."
dotnet publish --configuration Release --output ./bin/Release --no-build
if [ $? -ne 0 ]; then
    echo "ERROR: Publish failed"
    exit 1
fi
echo "✓ CLI tool published"
echo ""

echo "========================================"
echo "Setup Complete!"
echo "========================================"
echo ""
echo "The C# utilities are now ready to use."
echo ""
echo "To test the CLI tool, run:"
echo "  dotnet run -- validate-city Paris"
echo "  dotnet run -- format-temp 25 C"
echo "  dotnet run -- categorize-aqi 35"
echo ""
echo "Your Weather App will automatically use C# utilities if available."
echo ""
