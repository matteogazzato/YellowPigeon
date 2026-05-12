# Lab 3 - Debugging with Copilot - Prompts (English Only)

## 1. Analyze the Failure
```text
I have a failing test in my C# API, what could cause this result? Walk me through the calculation step-by-step.
The test is [Test Name]. The Expected result is [ExpectedValue]. The actual value is [ActualValue]
Here's the method [FileName]
The test case is defined in [TestFileMethodName]
```

## 2. Root Cause Analysis - First Pass
```text
Look at the calculation method and the failing test.
What are the most likely sources of this bug?
List 3-5 potential causes, ordered by probability.
For each cause, explain what would need to be true in the code.
```

## 3. Root Cause Analysis - Deep Dive (Opzionale)
```text
The test shows a [discount percentage]% discount on [subtotal] subtotal.
Walk me through what the code does step-by-step:
1. Start: subtotal = [value]
2. Discount applied: ?
3. After discount: ?
4. Tax applied: ?
5. Final: ?

Which step produces the incorrect result?
```

## 4. Propose the Fix
```text
Based on the root cause analysis, what's the minimal fix?
Show me the corrected code for lines in [MethodFileName].

I want to make sure the fix:
- Doesn't break other tests
- Only changes the problematic logic
- Includes a comment explaining why the change is necessary
```

## 5. Write a Regression Test
```text
Write an xUnit test case named with a proper name, using naming style TestName_Condition_ExpectedResult.
Use the same test structure as existing tests in the suite.
It should:
- Create the exact scenario that triggered the bug
- Assert that the issue no longer occurs
- Include any boundary or edge case checks relevant to the bug
```

## 6. Verify the Fix (Pattern)
```text
Apply the proposed fix. 
Will this fix the problem without side effects?
Could it break anything else?
Are there edge cases I'm missing?
```

## 7. Validate All Tests Pass
```text
Run the full test suite and report:
- How many tests pass?
- How many tests fail?
- Does the previously failing test now pass?
- Are any new failures introduced?

If new failures appear, what do they tell us about the fix?
```

## 8. Debugging Prompting Checklist (Reusable Reference)
```text
When debugging with Copilot, follow this flow:

1. COLLECT: Gather the failing test, logs, stack trace, and relevant code
2. SHARE: Paste all context into the chat
3. EXPLAIN: Ask "What could cause this?" (not "Fix this")
4. ANALYZE: Review Copilot's explanation step-by-step
5. VALIDATE: Walk through the code path with sample values
6. FIX: Ask for a minimal fix with explanation
7. TEST: Run all tests to verify the fix
8. REGRESS: Write a test that would catch this bug in the future
9. DOCUMENT: Note the root cause and solution for team learning
```
