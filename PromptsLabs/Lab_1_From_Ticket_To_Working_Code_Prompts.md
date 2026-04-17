# Lab 1 - From Ticket To Working Code - Prompts (English Only)

## 1. Implementation Plan
```text
You are a senior C# developer. Based on the user story and acceptance criteria below, propose a step-by-step implementation plan for a minimal ASP.NET Core API. Keep it incremental, with checkpoints after each step.
```

## 2. Design Decisions
```text
Propose a minimal architecture for this feature (endpoint -> service -> repository). Keep it pragmatic. Provide file names and responsibilities. Assume we use an existing SQL Server DB with Orders and OrderItems.
```

## 3. Explain Schema
```text
Here is the SQL schema (SCHEMA_VARIABLE) for Orders and OrderItems, check setup-db.sql used as script to set up the database. Explain it briefly and suggest how to map it to request/response DTOs for a Create Order endpoint.
```

## 4. Identify Constraints and Pitfalls
```text
Given this schema, what validations should the API enforce before inserting data? List them and map each one to an HTTP status code.
```

## 5. Request/Response DTOs
```text
Generate C# DTOs for CreateOrderRequest, CreateOrderItemRequest, and CreateOrderResponse matching the feature requirements.
Use C# records, add basic data annotations only where appropriate.
```

## 6. Minimal API Endpoint Skeleton
```text
Create a minimal API endpoint POST /api/orders. Add necessary documentation, keep code clean and simple.
```

## 7. Controller Alternative
```text
Create the same endpoint using an ASP.NET Core Controller (OrdersController) with [HttpPost].
```

## 8. Service Contract
```text
Define IOrderService and an implementation OrderService for creating an order. Keep it simple and testable.
```

## 9. Repository with Dapper
```text
Implement OrderRepository using SQL Server to insert into Orders and OrderItems. Provide the SQL statements and C# code.
```

## 10. Error Handling Strategy
```text
Add pragmatic error handling. Show where to implement this (endpoint vs service vs middleware).
```

## 13. requests.http
```text
Create a requests.http file with sample HTTP requests to test valid order creation, missing items (400), invalid quantity (400). Assume base URL is the one defined in launch settings file (LAUNCH_SETTINGS_FILE_VARIABLE).
```
