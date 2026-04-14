# YellowPigeon — Orders API

Skeleton project for the hands-on lab **"From Ticket To Working Code"** with GitHub Copilot.

This repository currently contains only the **base project structure** and database setup scripts. During the demo, we will use prompts from [Prompt_Pack.md](../Prompt_Pack.md) to generate the C# code required to implement the ticket described in [Ugly_Ticket.md](../Ugly_Ticket.md).

---

## Current Status

✅ **Already in place:**
- `.NET 10 ASP.NET Core` project scaffold
- `OrdersLab` database setup script with `Orders`, `OrderItems`, and `Customers` tables
- Folder structure (`Contracts`, `Services`, `Data`, `Middleware`, `Controllers`)
- Minimal `Program.cs` (basic HTTP pipeline only)

❌ **To be implemented during the lab (following Prompt_Pack.md):**
- DTOs: `CreateOrderRequest`, `CreateOrderItemRequest`, `CreateOrderResponse`
- Service layer: `IOrderService`, `OrderService`
- Repository layer: `IOrderRepository`, `OrderRepository`, `SqlConnectionFactory`
- Middleware: `GlobalExceptionMiddleware`
- Controller: `OrdersController`
- Validation and error handling
- Manual test file (`requests.http`)

---

## Table of Contents

- [YellowPigeon — Orders API](#yellowpigeon--orders-api)
  - [Current Status](#current-status)
  - [Table of Contents](#table-of-contents)
  - [1. Project Context](#1-project-context)
  - [2. Project Structure](#2-project-structure)
  - [3. Prerequisites](#3-prerequisites)
  - [4. Database Setup](#4-database-setup)
    - [4a. Start SQL Server in Docker](#4a-start-sql-server-in-docker)
    - [4b. Run setup-db.sql](#4b-run-setup-dbsql)
  - [5. Run the Application](#5-run-the-application)
  - [6. During the Demo](#6-during-the-demo)

---

## 1. Project Context

This project is based on the ticket in [Ugly_Ticket.md](../Ugly_Ticket.md), which defines a REST API user story:

> As a client application, I want to create a new order with one or more items so the order is saved in the database and can be processed later.

**Acceptance Criteria (AC1-AC5):**
- `POST /api/orders` endpoint with validation
- Atomic persistence (SQL transaction)
- `201 Created` response with `Location` header
- Error handling: `400` for validation, `409` for DB conflicts, `500` for unexpected failures

---

## 2. Project Structure

```
YellowPigeon/
├── README.md                        <- this file
├── scripts/
│   └── setup-db.sql                 <- SQL script to create DB and tables
└── OrdersApi/                       # Web API project
    ├── OrdersApi.csproj
    ├── Program.cs                   <- minimal startup, DI still to be added
    ├── appsettings.json
    ├── Properties/launchSettings.json
    ├── Controllers/                 <- empty, add OrdersController during lab
    ├── Contracts/                   <- empty, add DTOs during lab
    ├── Services/                    <- empty, add Service + Interface during lab
    ├── Data/                        <- empty, add Repository + Factory during lab
    └── Middleware/                  <- empty, add GlobalExceptionMiddleware during lab
```

---

## 3. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [VS Code](https://code.visualstudio.com/) with extensions:
  - **C# Dev Kit** (`ms-dotnettools.csdevkit`)
  - **REST Client** (`humao.rest-client`) - optional, for manual HTTP tests
  - **SQL Server (mssql)** (`ms-mssql.mssql`) - optional

---

## 4. Database Setup

### 4a. Start SQL Server in Docker

**Mac Intel:**
```bash
docker run --name sql-orders \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_password123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

**Mac Apple Silicon:**
```bash
docker run --name sql-orders \
  --platform linux/amd64 \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Your_password123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Wait ~30 seconds, then verify:
```bash
docker ps | grep sql-orders
```

### 4b. Run setup-db.sql

Connect to SQL Server with:
- **Server:** `localhost,1433`
- **Username:** `sa`
- **Password:** `Your_password123!`

Use the VS Code **SQL Server (mssql)** extension or any SQL client.

Open [scripts/setup-db.sql](scripts/setup-db.sql) and execute it. The script creates:
- `OrdersLab` database
- `Customers`, `Orders`, `OrderItems` tables
- Two sample customers (`CustomerId` 1 and 2)

---

## 5. Run the Application

From the `YellowPigeon/OrdersApi` folder:

```bash
dotnet run
```

The app usually starts on `http://localhost:5149` (HTTP) or on the port configured in `launchSettings.json`.

Make sure compilation succeeds before starting the lab steps.

---

## 6. During the Demo

Follow prompts from [Prompt_Pack.md](../Prompt_Pack.md) to build the feature incrementally:

1. **Prompt 1-2:** planning and design decisions
2. **Prompt 5:** generate DTOs -> add 3 files in `Contracts/`
3. **Prompt 8:** generate Service -> add `IOrderService.cs` and `OrderService.cs` in `Services/`
4. **Prompt 9:** generate Repository -> add `IOrderRepository.cs`, `OrderRepository.cs`, `SqlConnectionFactory.cs` in `Data/`
5. **Prompt 6 or 7:** generate Controller -> add `OrdersController.cs` in `Controllers/`
6. **Prompt 10:** generate error-handling middleware -> add `GlobalExceptionMiddleware.cs` in `Middleware/`
7. Update `Program.cs` to register services in DI
8. **Prompt 13:** add/use `requests.http` to test manually
9. **Prompt 14:** add structured logging

After each step, run `dotnet build` and `dotnet run` to ensure everything still compiles and starts correctly.
