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