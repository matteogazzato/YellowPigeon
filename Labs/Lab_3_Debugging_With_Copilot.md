# Lab 3 — "Debugging with Copilot: Faster Root Cause Analysis and Safer Fixes"

## Goal
Use Copilot to analyze failing scenarios, explain root causes, and implement focused fixes with confidence.

**📋 Prompts:**
Reusable Copilot prompts for this lab are in [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md).

## Scenario
An order endpoint is returning incorrect totals under specific conditions. Students receive a failing test, logs, and stack traces—and use Copilot to identify the bug, fix it safely, and add a regression test.

## What They Practice
* Analyzing error logs and stack traces with Copilot
* Asking Copilot to explain likely root causes (not just "fix it")
* Implementing a minimal, surgical fix
* Writing a regression test to prevent the issue from reoccurring
* Validating the fix doesn't break existing behavior

## 60-min flow
* 0–5: Explain "debugging with Copilot" philosophy (minimal theory)
* 5–15: Introduce the failing scenario (test, logs, code context)
* 15–30: Use Copilot to analyze root cause and propose fix
* 30–45: Implement the fix and verify with existing tests
* 45–55: Write a regression test that catches the original bug
* 55–60: Recap prompting patterns and debugging workflow

## Deliverables
* Root cause analysis (documented in chat or notes)
* Fixed implementation in the service/endpoint
* Regression test that catches the original bug
* A reusable "debugging prompting checklist"

---

## Context: Starting Point

This lab builds upon **Lab 1** and **Lab 2** and assumes:
- ✅ A working `POST /api/orders` endpoint
- ✅ `OrderService.CalculateOrderTotal()` method exists
- ✅ Unit tests are in place from Lab 2
- ✅ The project compiles and runs

**What we focus on in Lab 3:**
- **Diagnosing failures**: understanding logs, stack traces, and error patterns
- **Root cause analysis**: using Copilot to reason about why a bug exists
- **Surgical fixes**: implementing minimal changes that solve the problem without side effects
- **Regression prevention**: writing tests that would catch this bug in the future

---

## Scenario: The Bug

### The Problem
A customer reports that orders with **multiple items and bulk discounts** are returning a **negative total** or an **incorrect tax amount**. 

Some orders show:
- `Total: -$5.50` (impossible)
- `Tax: $0.00` when it should be calculated
- Intermittent failures depending on item count and discount percentage

### Starting Evidence
**Failing Test Output:**
```
FAIL: CalculateOrderTotal_WithBulkDiscountAnd3Items_ShouldReturnPositiveTotal

Expected: $150.50
Actual: -$5.50

Stack trace:
  at OrdersApi.Services.OrderService.CalculateOrderTotal(Order order)
  at OrdersApi.Tests.OrderServiceTests.CalculateOrderTotal_WithBulkDiscountAnd3Items_ShouldReturnPositiveTotal()
```

**Log Entry (from POST /api/orders):**
```
[ERROR] 2026-05-06 14:23:15 - Order calculation failed for OrderId=4521
Exception: System.InvalidOperationException: Subtotal after discount is negative
Stack Trace:
  at OrdersApi.Services.OrderService.CalculateOrderTotal(Order order) line 48
  at OrdersApi.Services.OrderService.CreateOrder(Order order) line 72
```

---

## Implementation Reference

The following code is available for inspection and debugging:

- [OrdersApi/Services/OrderService.cs](OrdersApi/Services/OrderService.cs): Contains `CalculateOrderTotal(Order order)` with the calculation logic, discount application, tax computation, and rounding.
- [OrdersApi/Contracts/OrderModels.cs](OrdersApi/Contracts/OrderModels.cs): Defines `Order` and `OrderItem` models.
- [OrdersApi.Tests/UnitTest1.cs](OrdersApi.Tests/UnitTest1.cs): Existing test suite from Lab 2.

The bug exists in the discount or tax calculation logic and must be diagnosed using Copilot.

---

## What We Practice

* **Prompting for diagnosis first** — Ask Copilot "what could cause this?" before jumping to fixes
* **Analyzing code behavior** — Walk through the calculation step-by-step with Copilot
* **Identifying the minimal fix** — Avoid over-engineering; target the root cause only
* **Testing the fix** — Verify with existing tests and add a regression test
* **Documenting assumptions** — Clarify business rules before finalizing the fix

---

## Hands-On Flow

### Phase 1: Understanding the Failure (5–15 min)

**Your role:**
1. Open [OrdersApi/Services/OrderService.cs](OrdersApi/Services/OrderService.cs) and locate `CalculateOrderTotal()`
2. Read the existing test that is failing
3. Open the chat and share:
   - The failing test name and expected vs. actual result
   - The code snippet of the calculation method
   - The error message from the logs

**Copilot prompt:**
Use **"Analyze the Failure"** from [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md#1-analyze-the-failure) — substitute your actual test data.

---

### Phase 2: Root Cause Analysis (15–30 min)

**Your role:**
1. Read Copilot's explanation carefully
2. Ask follow-up questions if needed:
   - "Is the discount being applied twice?"
   - "Could the tax calculation be subtracting instead of adding?"
   - "What are the valid ranges for each variable?"
3. Identify the likely culprit and the line(s) where it occurs

**Copilot prompts:**
Use **"Root Cause Analysis - First Pass"** and **"Root Cause Analysis - Deep Dive"** from [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md#2-root-cause-analysis---first-pass) — adapt with your specific values and observations.

---

### Phase 3: Implement the Fix (30–45 min)

**Your role:**
1. Based on root cause analysis, ask Copilot to suggest a fix
2. Review the fix for logic correctness and side effects
3. Apply the fix to the code

**Copilot prompt:**
Use **"Propose the Fix"** from [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md#4-propose-the-fix) — specify the exact lines and criteria for safety.

**After applying the fix:**
- Run all existing tests to ensure nothing else broke
- Verify the failing test now passes

---

### Phase 4: Write a Regression Test (45–55 min)

**Your role:**
1. Add a new test case that would have caught this bug
2. The test should specifically verify the scenario that was failing

**Copilot prompt:**
Use **"Write a Regression Test"** from [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md#5-write-a-regression-test) — adapt the test name and scenario to match your bug.

**After the test is written:**
- Add it to [OrdersApi.Tests/UnitTest1.cs](OrdersApi.Tests/UnitTest1.cs)
- Run the test to confirm it passes with your fix
- Verify it would fail with the old code (optional but valuable)

---

### Phase 5: Recap & Documentation (55–60 min)

**Your deliverables:**
1. ✅ Root cause documented (in chat or notes)
2. ✅ Fixed code committed or ready to commit
3. ✅ Regression test added and passing
4. ✅ All existing tests still passing

**Reflection questions to discuss:**
- How did Copilot help narrow down the cause faster than manual inspection?
- What assumptions about the business logic were important?
- How does the regression test protect against this bug in the future?

---

## Reusable Debugging Patterns

After this lab, you'll have a toolkit of reusable prompts for debugging scenarios:

- **"Explain the Failure"** — Use when you have an error and want Copilot to reason about causes
- **"Verify the Fix"** — Use when proposing a fix and want validation before committing
- **"Regression Test"** — Use to create tests that prevent the same bug from reoccurring
- **"Debugging Prompting Checklist"** — Use as a reference flow for any future debugging session

All patterns are available in [Lab_3_Debugging_With_Copilot_Prompts.md](../PromptsLabs/Lab_3_Debugging_With_Copilot_Prompts.md).

---

## Success Criteria

By the end of Lab 3, you should have:
- ✅ Identified the root cause of the bug
- ✅ Implemented a fix that solves the problem
- ✅ All existing tests passing
- ✅ A new regression test that catches the original bug
- ✅ A clear understanding of how Copilot accelerates debugging workflows

---

## Takeaways

1. **Debugging with Copilot is about dialogue**: Ask why, then how. Don't jump to the fix.
2. **Regression tests are insurance**: They prevent the same bug from reoccurring silently.
3. **Small fixes are safer fixes**: Minimal changes are easier to review and less likely to introduce new bugs.
4. **Logs + code + tests tell a story**: Copilot is better when you provide all three.
