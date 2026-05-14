# Copilot Instructions for YellowPigeon

## Repository at a glance
- This is a small .NET 10 solution centered on an ASP.NET Core API in `OrdersApi/` and an xUnit test project in `OrdersApi.Tests/`.
- The solution entrypoint is `/home/runner/work/YellowPigeon/YellowPigeon/OrdersLab.slnx`.
- The current production surface is intentionally small: `OrdersApi/Program.cs` wires services and exposes `GET /health`.
- Core business logic currently lives in `OrdersApi/Services/OrderService.cs`.
- Core models live in `OrdersApi/Contracts/OrderModels.cs`.
- Database diagnostics live in `OrdersApi/Data/HealthCheckService.cs`.

## How to work efficiently
- Start from the repository root: `/home/runner/work/YellowPigeon/YellowPigeon`.
- Read `README.md` first; it already documents the expected local workflow and DB setup.
- For most tasks, inspect `Program.cs`, `Services/`, `Contracts/`, `Data/`, and `OrdersApi.Tests/` before changing code.
- Keep changes small and aligned with the existing layered structure: API/hosting, contracts, services, data, tests.
- Prefer extending the existing .NET solution instead of introducing new tooling or project structure.

## Build and test commands
- Restore: `dotnet restore OrdersLab.slnx`
- Build: `dotnet build OrdersLab.slnx --no-restore`
- Test: `dotnet test OrdersLab.slnx --no-build`
- Run API: `dotnet run --project OrdersApi/OrdersApi.csproj`

## Current conventions and expectations
- Target framework is `net10.0`; nullable reference types and implicit usings are enabled.
- The app uses minimal hosting in `Program.cs`.
- Existing code favors straightforward service classes and simple contracts over heavy abstraction.
- Tests use xUnit; supporting packages include FluentAssertions, Moq, and `Microsoft.AspNetCore.Mvc.Testing`.
- The existing test suite is still minimal (`OrdersApi.Tests/UnitTest1.cs` is a template-style placeholder), so validate new behavior with focused tests when adding logic.

## Database and runtime notes
- The health endpoint depends on SQL Server connectivity through the `OrdersDb` connection string in `OrdersApi/appsettings.json`.
- Local DB setup is driven by `scripts/setup-db.sql`; README documents the Docker-based SQL Server workflow.
- If SQL Server is not running or not initialized, `/health` is expected to return `503` with error details.

## Known issues observed during onboarding
- `dotnet build OrdersLab.slnx --no-restore` succeeds but emits warning `ASPDEPR002` because `Program.cs` uses deprecated `WithOpenApi()`. Treat this as a known warning unless your task is specifically to modernize OpenAPI wiring.
- The repository originally ignored the entire `.github/` directory via `.gitignore`, which would prevent committing this onboarding file. The workaround was to narrow the ignore rule and explicitly allow `.github/copilot-instructions.md`.

## When making future changes
- If you add API behavior, check whether it should be registered in `Program.cs` and covered by tests in `OrdersApi.Tests/`.
- If you add DB-facing features, keep the existing SQL Server assumptions in mind and update `scripts/setup-db.sql` and README only when behavior/setup actually changes.
- Avoid broad refactors unless the task asks for them; this repository is used as a teaching/demo baseline and benefits from incremental evolution.
