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

---

## Bonus Scenario (If Time Remains): ApplyBulkDiscount()

### Context
In addition to `CalculateOrderTotal()`, the lab includes a second method to test if time allows:

**`ApplyBulkDiscount(Order order)`**
- Applies a **progressive discount** based on **total item quantity** (not value)
- Returns the discount percentage to apply (e.g., 5m for 5%)
- Tier structure:
  - 1–10 items → 0% discount
  - 11–25 items → 5% discount
  - 26–50 items → 10% discount
  - 51+ items → 15% discount

### Why This Bonus Scenario?
This method complements `CalculateOrderTotal()` by introducing:
1. **Range-based logic** (boundaries at 10, 25, 50, 51 are critical)
2. **Quantity aggregation** (sum across items, not per-item logic)
3. **Simpler calculation** (just returns a percentage, no complex formula)
4. **Switch expression patterns** (modern C# style to test)

### What Students Practice
* Testing **range boundaries** (off-by-one errors, edge cases at tier thresholds)
* Identifying **equivalence classes** for quantity bins
* Using **parameterized tests** efficiently with multiple boundary scenarios
* Recognizing when a method is **too simple to over-test** vs. needing thorough edge-case coverage

### Implementation Reference
- See [OrdersApi/Services/OrderService.cs](OrdersApi/Services/OrderService.cs): `ApplyBulkDiscount(Order order)` method
- Follows the same validation and error-handling patterns as `CalculateOrderTotal()`

### Suggested Flow (10–15 minutes)
1. **Review the code**: Identify tier boundaries and potential off-by-one errors
2. **List test cases**: Use Given-When-Then for each tier + boundary conditions
3. **Generate baseline tests**: Use `[Theory]` with `[InlineData]` for all tier transitions (e.g., 10, 11, 25, 26, 50, 51)
4. **Add validation tests**: Null orders, empty items, invalid quantities
5. **Compare with `CalculateOrderTotal()` tests**: Discuss why this method is simpler and how test strategy differs
