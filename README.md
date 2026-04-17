# YellowPigeon - Orders API

ASP.NET Core Web API solution for order management with SQL Server persistence and a dedicated test project.

## Evolution Through Dedicated Sessions

This project is intentionally maintained and used as a baseline codebase that evolves over time through dedicated and distinct hands-on sessions.

These sessions are designed to leverage GitHub Copilot as a practical co-development assistant for architecture, implementation, refactoring, and testing activities.

References for each lab/lesson are available in [Labs](Labs).

The prompts used during the sessions (or prepared for upcoming sessions) are available in [PromptsLabs](PromptsLabs).

## Project Purpose

YellowPigeon provides a clean starting point for building and evolving an Orders backend service with:
- API layer based on ASP.NET Core
- business logic layer for order rules and calculations
- data layer connected to SQL Server
- test layer for automated verification

The current codebase includes a health endpoint and core domain/service components for order total calculation.

## Goals

- Expose HTTP endpoints for order workflows
- Keep code organized by responsibility (contracts, services, data, middleware)
- Ensure maintainability and testability through a dedicated test project
- Provide a reproducible local setup (database + application + test execution)

## Architecture Overview

The solution follows a layered structure:

- API / Hosting: application bootstrap and HTTP pipeline
- Contracts: domain request/response and shared models
- Services: business rules (for example total calculation)
- Data: persistence and infrastructure services
- Middleware: cross-cutting concerns and error handling
- Tests: unit/integration test scaffolding

### Main Components

- `OrdersApi/Program.cs`
  - Configures services, middleware, OpenAPI, and routes
  - Exposes `GET /health`
- `OrdersApi/Data/HealthCheckService.cs`
  - Validates SQL Server connectivity for health diagnostics
- `OrdersApi/Contracts/OrderModels.cs`
  - Defines `Order` and `OrderItem` models
- `OrdersApi/Services/OrderService.cs`
  - Implements `CalculateOrderTotal(Order order)` with validation and rounding
- `OrdersApi.Tests/`
  - Test project (`xUnit`, `Moq`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`)

## Repository Structure

```text
YellowPigeon/
|- README.md
|- OrdersLab.slnx
|- scripts/
|  |- setup-db.sql
|- OrdersApi/
|  |- OrdersApi.csproj
|  |- Program.cs
|  |- appsettings.json
|  |- appsettings.Development.json
|  |- Properties/
|  |  |- launchSettings.json
|  |- Contracts/
|  |  |- OrderModels.cs
|  |- Services/
|  |  |- OrderService.cs
|  |- Data/
|  |  |- HealthCheckService.cs
|  |- Middleware/
|- OrdersApi.Tests/
|  |- OrdersApi.Tests.csproj
|  |- UnitTest1.cs
```

## Prerequisites

- .NET 10 SDK
- Docker Desktop
- SQL Server client tool (optional but recommended)
  - VS Code extension `ms-mssql.mssql` or any SQL client
- VS Code extension `ms-dotnettools.csdevkit` (recommended)

## Setup From Scratch

### 1. Restore dependencies

From repository root:

```bash
dotnet restore OrdersLab.slnx
```

### 2. Start SQL Server in Docker

Mac Intel:

```bash
docker run --name sql-orders \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_password123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Mac Apple Silicon:

```bash
docker run --name sql-orders \
  --platform linux/amd64 \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_password123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Verify container status:

```bash
docker ps | grep sql-orders
```

### 3. Initialize database schema

Run `scripts/setup-db.sql` against SQL Server:
- Server: `localhost,1433`
- User: `sa`
- Password: `Your_password123!`

The script creates:
- `OrdersLab` database
- `Customers`, `Orders`, `OrderItems` tables
- sample customers

### 4. Verify connection string

Check `OrdersApi/appsettings.json` and/or `OrdersApi/appsettings.Development.json` for `ConnectionStrings:OrdersDb`.

The `HealthCheckService` expects this key:

```json
"ConnectionStrings": {
  "OrdersDb": "Server=localhost,1433;Database=OrdersLab;User Id=sa;Password=Your_password123!;TrustServerCertificate=True;"
}
```

## Build, Run, Test

### Build solution

```bash
dotnet build OrdersLab.slnx
```

### Run API

From repository root:

```bash
dotnet run --project OrdersApi/OrdersApi.csproj
```

or from `OrdersApi/`:

```bash
dotnet run
```

### Run tests

```bash
dotnet test OrdersLab.slnx
```

List discovered tests:

```bash
dotnet test OrdersApi.Tests --list-tests
```

Run tests with detailed logs:

```bash
dotnet test OrdersApi.Tests -v detailed
```

Run tests with coverage (collector enabled):

```bash
dotnet test OrdersApi.Tests /p:CollectCoverage=true
```

## Useful Commands

```bash
# Stop SQL container
docker stop sql-orders

# Restart SQL container
docker start sql-orders

# Remove SQL container (data loss if not persisted)
docker rm -f sql-orders

# Clean solution outputs
dotnet clean OrdersLab.slnx

# Restore + build in one flow
dotnet restore OrdersLab.slnx && dotnet build OrdersLab.slnx
```

## Available Endpoint

### Health Check

- Method: `GET`
- Path: `/health`
- Behavior:
  - `200` when SQL connectivity is healthy
  - `503` when SQL connectivity fails

Example:

```bash
curl http://localhost:5149/health
```

## Troubleshooting

### Build succeeds but `/health` returns 503

Possible causes:
- SQL container not running
- wrong connection string
- schema script not executed

Checks:

```bash
docker ps | grep sql-orders
```

Verify `ConnectionStrings:OrdersDb` and rerun `scripts/setup-db.sql` if needed.

### `dotnet test` shows no tests or only template tests

The test project is set up correctly, but only template tests may exist.
Add real unit tests under `OrdersApi.Tests/` and rerun:

```bash
dotnet test OrdersApi.Tests --list-tests
```

### Port conflicts on startup

If default ports are busy, update launch settings in `OrdersApi/Properties/launchSettings.json` or run with custom URLs.
