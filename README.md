# GreenStackAPI

A RESTful API built with ASP.NET Core 8.0 that analyzes UK electricity generation mix data and recommends optimal charging windows based on clean energy availability. It integrates with the [UK Carbon Intensity API](https://api.carbonintensity.org.uk) to provide insights into renewable energy percentages for electric vehicle (EV) charging decisions.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Run with .NET CLI](#run-with-net-cli)
  - [Run with Docker](#run-with-docker)
  - [Run with Visual Studio](#run-with-visual-studio)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Configuration](#configuration)

## Features

- Fetches live UK electricity generation data from the Carbon Intensity API
- Calculates average daily energy mix over the next 3 days
- Identifies the optimal time window for EV charging based on clean energy availability
- Swagger UI for interactive API documentation (available in Development mode)
- Docker support for containerized deployment
- Clean architecture using the Repository and Service patterns with dependency injection

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 8.0 / C# | Application framework and language |
| ASP.NET Core | Web API framework |
| Swashbuckle (Swagger) | API documentation |
| HttpClient | External API integration |
| Docker | Containerization |

**External dependency:** [UK Carbon Intensity API](https://api.carbonintensity.org.uk/generation/)

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for local development)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Run with .NET CLI

```bash
# Clone the repository
git clone https://github.com/KKubis03/GreenStackAPI.git
cd GreenStackAPI

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The API will be available at `http://localhost:5032`.  
Swagger UI is accessible at `http://localhost:5032/swagger` when running in Development mode.

### Run with Docker

```bash
# Build the Docker image
docker build -t greenstack-api .

# Run the container
docker run -p 8080:8080 -p 8081:8081 greenstack-api
```

The API will be available at `http://localhost:8080`.

### Run with Visual Studio

1. Open `GreenStackAPI.sln` in Visual Studio
2. Select a launch profile (`http`, `https`, or `Container (Dockerfile)`)
3. Press **F5** to start debugging

## API Endpoints

Base URL: `http://localhost:5032/api/mix`

### GET `/three-days-averages`

Returns the average electricity generation mix for the next 3 days, including clean energy percentages for each day.

**Example request:**
```
GET /api/mix/three-days-averages
```

**Example response:**
```json
[
  {
    "date": "2024-01-15T00:00:00",
    "averageMix": [
      { "fuel": "wind", "perc": 35.2 },
      { "fuel": "solar", "perc": 5.1 },
      { "fuel": "gas", "perc": 40.0 }
    ],
    "cleanEnergyPercentage": 52.4
  }
]
```

### GET `/optimal-charging-window`

Finds the optimal 2-day window within the next 3 days with the highest clean energy percentage for EV charging.

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `windowHours` | integer (1–6) | Yes | Size of the charging window in hours |

**Example request:**
```
GET /api/mix/optimal-charging-window?windowHours=2
```

**Example response:**
```json
{
  "start": "2024-01-16T00:00:00",
  "end": "2024-01-17T00:00:00",
  "averageCleanEnergyPercentage": 61.8
}
```

## Project Structure

```
GreenStackAPI/
├── Controllers/
│   └── MixController.cs          # API endpoint handlers
├── Models/
│   ├── ApiResponse.cs            # External API response wrapper
│   ├── GenerationMixData.cs      # Time-interval energy data
│   ├── GenerationMixItem.cs      # Fuel type percentage
│   ├── DailyMix.cs               # Aggregated daily energy data
│   └── OptimalChargingWindow.cs  # Optimal charging window result
├── Repositories/
│   ├── IMixApiRepository.cs      # Repository interface
│   └── MixApiRepository.cs      # External API data access
├── Services/
│   └── MixService.cs             # Business logic
├── Properties/
│   └── launchSettings.json       # Launch profiles
├── Program.cs                    # Application entry point & DI setup
├── appsettings.json              # Production configuration
├── appsettings.Development.json  # Development configuration
├── Dockerfile                    # Docker build configuration
└── GreenStackAPI.csproj          # Project file & NuGet dependencies
```

## Configuration

The application uses standard ASP.NET Core configuration. No environment variables are required for basic operation.

| Setting | Default (HTTP) | Default (Docker) |
|---|---|---|
| HTTP port | `5032` | `8080` |
| HTTPS port | `7207` | `8081` |
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Production` |

Swagger UI is only enabled when `ASPNETCORE_ENVIRONMENT` is set to `Development`.
