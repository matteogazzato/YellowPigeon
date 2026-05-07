# Lab 2 - Unit Tests that Matter - Prompts (English Only)

## 1. Strategy First - Identify Test Cases
```text
Before generating tests, list all relevant test cases for CalculateOrderTotal().
Categorize them into: normal cases, boundary cases, and error/exception cases.
Use Given-When-Then style.
```

## 2. Generate the Method Under Test
```text
Create a method CalculateOrderTotal(Order order) in an OrderService class.
The order has a list of OrderItem (Quantity, UnitPrice), a DiscountPercent (0-100),
and a TaxRate (e.g., 0.22 for 22%).
Formula: total = sum(Quantity * UnitPrice) * (1 - DiscountPercent/100) * (1 + TaxRate).
Round to 2 decimals using MidpointRounding.AwayFromZero.
```

## 3. Generate Baseline Tests
```text
Using xUnit, generate tests for CalculateOrderTotal() with names in the format
MethodName_Condition_ExpectedResult. Cover:
- Single item order with no discount
- Multiple items order
- Discount applied correctly
- Tax applied correctly
- Discount + tax combination
```

## 4. Identify Missing Edge Cases
```text
Review these tests for CalculateOrderTotal() and tell me which boundary or
edge cases are missing. Generate the additional tests you think are necessary.
[paste existing test code]
```

## 5. Convert to Parameterized Tests
```text
Convert these xUnit tests that test similar scenarios into a single
[Theory] with [InlineData]. Keep descriptive names and ensure that
each InlineData has an explanatory comment if the case is not obvious.
```

## 6. Add Mocking to Tests
```text
The CalculateOrderTotal() method depends on IOrderRepository.GetById(int id).
Add Moq mocking to the test setup. Use an OrderBuilder or factory method
to construct test orders in a readable and reusable way.
```

## 7. Refactor for Readability
```text
Refactor this test suite:
1. Use FluentAssertions instead of Assert.Equal
2. Extract a private CreateOrder(...) method to eliminate duplication
3. Ensure each test class has a single responsibility
[paste test suite]
```

---

# BONUS: ApplyBulkDiscount() Prompts (If Time Remains)

**Context**: If time allows during the lab, use these prompts to test a second method: `ApplyBulkDiscount(Order order)`.
This method returns a bulk discount percentage (0%, 5%, 10%, 15%) based on total item quantity tiers.

## Bonus 1. Understand the Tier Logic
```text
Review the ApplyBulkDiscount() method and explain:
1. What are the quantity thresholds?
2. What discount percentage is returned for each tier?
3. What are the **critical boundary values** to test?
4. Why is quantity aggregation (sum across items) important to test?
```

## Bonus 2. Identify Tier Boundary Test Cases
```text
For ApplyBulkDiscount(), list all test cases needed to verify correct tier assignment.
Use Given-When-Then format. Focus on:
- Minimum quantity in each tier (e.g., 1, 11, 26, 51)
- Maximum quantity in each tier (e.g., 10, 25, 50, 100+)
- Off-by-one boundary violations (e.g., 10 vs. 11, 25 vs. 26, 50 vs. 51)

Which of these are most critical to catch bugs?
```

## Bonus 3. Generate Tier Boundary Tests
```text
Using xUnit [Theory] with [InlineData], generate parameterized tests for ApplyBulkDiscount()
that cover:
- Each tier minimum quantity (1, 11, 26, 51)
- Each tier maximum quantity (10, 25, 50, 100)
- All tier transitions (10→11, 25→26, 50→51)

Expected format: [InlineData(quantity, expectedDiscount)] with comments explaining each case.
```

## Bonus 4. Validation and Edge Cases
```text
Add tests for ApplyBulkDiscount() that verify:
1. Null order throws ArgumentNullException
2. Empty items collection throws ArgumentException
3. Null item in collection throws ArgumentException
4. Negative or zero quantity in an item throws ArgumentException
5. Single item with high quantity (51+) triggers highest tier

Ensure each test name follows: MethodName_Condition_ExpectedResult
```

## Bonus 5. Comparison with CalculateOrderTotal() Tests
```text
Compare the test structure for ApplyBulkDiscount() with CalculateOrderTotal():
1. How many test cases does each method need? Why?
2. Which method has more complex validation? Why?
3. When should you use a simple [Fact] vs. a [Theory] with multiple [InlineData]?
4. What does this tell you about test granularity and coverage strategy?
```

## Bonus 6. Add to Integration Flow
```text
Create a test that demonstrates the full flow:
1. Create an order with 15 items
2. Call ApplyBulkDiscount() to get the bulk discount (should be 5%)
3. Combine the bulk discount with an existing discount (e.g., 10% promotional discount)
4. Pass the combined discount to CalculateOrderTotal()
5. Assert the final total is correctly calculated

This tests how ApplyBulkDiscount() integrates with CalculateOrderTotal() in real usage.
```