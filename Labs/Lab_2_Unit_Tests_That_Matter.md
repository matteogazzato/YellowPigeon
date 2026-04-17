# Lab 2 — "Unit Tests that Matter" (xUnit + edge cases + mocking)

## Goal
Generate high-quality tests, not just coverage noise.

# Scenario
Service method CalculateOrderTotal() with discounts, taxes, rounding, edge cases.

# What they practice
* Prompting for test strategy first (equivalence classes, boundaries)
* Generating xUnit tests + meaningful names
* Using Copilot to identify missing cases
* Refactoring tests for readability

# 60-min flow
* 0–10: explain “good tests vs bad tests” (minimal theory)
* 10–25: generate baseline tests with Copilot
* 25–45: improve with edge cases, parameterized tests
* 45–55: reduce duplication, fixtures/builders
* 55–60: recap prompt patterns

# Deliverables
* Test suite with edge cases + structure
* A reusable “test prompting checklist”

---

## Context: Starting Point

This lab builds upon **Lab 1 ("From Ticket to Working Code")** and assumes:
- ✅ A working `POST /api/orders` endpoint has been implemented
- ✅ Basic DTOs, Services, and Repositories exist
- ✅ The `OrdersApi` project is compiled and running

**What we focus on in Lab 2:**
- Testing the **business logic** layer (e.g., `OrderService.CalculateOrderTotal()`)
- Writing **comprehensive unit tests** with xUnit that cover edge cases and boundaries
- Using **mocking** (Moq) to isolate units under test
- Applying **readable assertion syntax** (FluentAssertions)
- Identifying and documenting **strategic test cases** (not just coverage noise)

---

## Project Setup: What's Been Added

### 1. **New Test Project Created**
- Project: `OrdersApi.Tests` (.NET 10, xUnit template)
- Location: `YellowPigeon/OrdersApi.Tests/`
- Added to solution: `OrdersLab.slnx`

### 2. **Project References**
- `OrdersApi.Tests` references `OrdersApi` (main project)
- Allows test code to import and test production classes

### 3. **NuGet Packages Added**
- **xunit** `2.9.3` — Unit testing framework
- **xunit.runner.visualstudio** `3.1.4` — Test Explorer integration
- **Microsoft.NET.Test.Sdk** `17.14.1` — Test infrastructure
- **Moq** `4.20.72` — Mocking framework for isolating dependencies
- **FluentAssertions** `8.9.0` — Fluent assertion syntax
- **Microsoft.AspNetCore.Mvc.Testing** `10.0.6` — Integration testing helpers
- **coverlet.collector** `6.0.4` — Code coverage

### 4. **Program.cs Made Testable**
- Added: `public partial class Program { }` at the end of `Program.cs`
- Reason: Enables `WebApplicationFactory<Program>` in integration tests
- Impact: No runtime changes; purely for test infrastructure

### 5. **Build Verified**
- ✅ Full solution builds successfully
- ✅ Both `OrdersApi` and `OrdersApi.Tests` compile without errors

---

## Scenario
Service method `CalculateOrderTotal()` with discounts, taxes, rounding, and edge cases.

## Implementation Reference

To support this lab, the following implementation has been added to the project:
- [OrdersApi/Services/OrderService.cs](OrdersApi/Services/OrderService.cs): contains `CalculateOrderTotal(Order order)` with the business formula, input validation, and final rounding (`MidpointRounding.AwayFromZero`).
- [OrdersApi/Contracts/OrderModels.cs](OrdersApi/Contracts/OrderModels.cs): defines the `Order` and `OrderItem` models used as inputs for the calculation logic.

This means Lab 2 can start directly from real code and focus on test quality: equivalence classes, boundary values, validation failures, and rounding behavior.

## What We Practice 
* Prompting for test strategy first (equivalence classes, boundaries)
* Generating xUnit tests + meaningful names
* Using Copilot to identify missing cases
* Refactoring tests for readability
* Isolating logic with mocks
