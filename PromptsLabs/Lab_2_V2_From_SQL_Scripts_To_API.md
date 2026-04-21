# Lab 2 V2 - From SQL Scripts to C# APIs: Working Effectively with GitHub Copilot

## 1. Understand the SQL logic
```text
Analyze the following SQL script.Explain step by step what the logic does and what conditions drive the behavior.Highlight assumptions and edge cases.
```

## 2. Understand the existing C# script
```text
Analyze this C# script.
Explain how it maps (or does not map) to the SQL logic.
Call out similarities and differences.
```

## 3. Define the variation
```text
Based on both implementations, describe the changes needed to support:
- searching outgoing segments
- stopping at the first EndNode that is NOT a No Stop node
Do not generate code yet.
```

## 4. Generate the new C# logic
```text
Now generate the updated C# implementation.
Constraints:
- reuse existing methods where possible
- do not change behavior outside the variation described
- keep the code readable and defensive
```

## 5. Review & challenge Copilot
```text
Review the generated code.
List potential edge cases or ambiguity in node traversal.
Suggest improvements without changing the core logic.
```

