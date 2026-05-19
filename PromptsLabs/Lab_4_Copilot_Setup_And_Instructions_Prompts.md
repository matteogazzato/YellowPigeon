# Lab 4 — Prompts: "Copilot-Ready Project: Setup, Instructions, and Smarter Context"

These prompts are designed for live use during Lab 4. They are organized to follow the hands-on flow. Each prompt is ready to use as-is — adapt the parts in `[BRACKETS]` to match your specific context.

Baseline: "How should I add a new endpoint to this project? What files do I need to create or modify?"

---

## 1. Draft `copilot-instructions.md`

**When:** Phase 2 — writing the global instruction file.

**Prompt:**
```
I'm setting up GitHub Copilot for a project called BrewTrack.
It's a C# ASP.NET Core application that uses minimal APIs (no MVC controllers),
Dapper for data access (no Entity Framework), SQL Server with snake_case naming,
and xUnit for tests.
Read the @README.MD file for any addition info.

Generate a `copilot-instructions.md` file for the `.github/` folder.
It should tell Copilot:
- the tech stack and .NET version
- the architectural pattern and layering rules
- what NOT to generate (no EF, no MVC controllers, no raw string SQL,
  no direct repository calls from endpoints)
- naming conventions for C# and SQL
- testing conventions (naming pattern, structure, framework)

Keep it concise. Use short sections with headers.
```

---

## 2. Review and Improve `copilot-instructions.md`

**When:** After generating the draft — before saving the file.

**Prompt:**
```
Review this `copilot-instructions.md` draft for BrewTrack.
Check for:
- missing conventions that a developer would need to know
- rules that are too vague to be actionable
- anything that could produce incorrect suggestions if followed literally

Suggest specific improvements with examples where relevant.
```

---

## 3. Draft Instruction File for Tests

**When:** Phase 3a — creating `.github/instructions/tests.instructions.md`.

**Prompt:**
```
Generate a `.github/instructions/tests.instructions.md` file for BrewTrack.

It should apply to tests file like [TestFile]

Rules to include:
- xUnit framework only
- Arrange/Act/Assert structure
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Mock dependencies using Moq and constructor injection with interfaces
- Every test class should include at least one edge case
  (null, zero, empty collection, invalid status transition)
- Assert on specific values, not just non-null

Format: Markdown with a YAML frontmatter block containing the `applyTo` field.
```

---

## 4. Draft Instruction File for Services and Repositories

**When:** Phase 3b — creating `.github/instructions/services-repositories.instructions.md`.

**Prompt:**
```
Generate a `.github/instructions/services-repositories.instructions.md` file for BrewTrack.

It should apply to file contained in [ServicesFolder] and [RepositoriesFolder]

Rules to include:
- Services expose an interface (e.g. IBeersService, IBatchesService) and are registered in DI
- Services contain business logic only — no SQL, no HTTP context
- Batch status transitions (planning → brewing → conditioning → ready)
  are validated in the service layer, never in the repository or endpoint
- Repositories contain all data access using Dapper
- All SQL must be parameterized — no string interpolation
- Repositories return domain models or DTOs, never dynamic or DataTable
- Services inject repositories via interfaces

Format: Markdown with a YAML frontmatter block containing the `applyTo` field.
```

---

## 5. Draft Prompt File for Adding an Endpoint

**When:** Phase 4a — creating `.github/prompts/add-endpoint.prompt.md`.

**Prompt:**
```
Create a reusable prompt file for the `.github/prompts/` folder.
The file should be named `add-endpoint.prompt.md` and used when a developer
needs to add a new minimal API endpoint to BrewTrack.

The prompt should ask the user to describe:
- which entity the feature relates to (beer or batch)
- the operation and expected input/output

Then instruct Copilot to generate:
1. The endpoint registration using `app.Map*` in a `*Endpoints.cs` file
2. A service method with the relevant interface
3. A repository method using Dapper with parameterized SQL
4. Any DTOs needed

Include YAML frontmatter with `description` and `mode: ask`.
Reference the relevant instruction files in the prompt body.
```

---

## 6. Draft Prompt File for Writing Tests

**When:** Phase 4b — creating `.github/prompts/write-tests.prompt.md`.

**Prompt:**
```
Create a reusable prompt file for `.github/prompts/write-tests.prompt.md`.
It should be used when a developer wants to generate xUnit tests
for a service or repository method in BrewTrack.

The prompt should ask the user to paste or describe the method under test,
then instruct Copilot to:
- follow Arrange/Act/Assert
- use `MethodName_Scenario_ExpectedBehavior` naming
- include happy path and at least two edge cases
- mock dependencies via Moq and interfaces

Include YAML frontmatter with `description` and `mode: ask`.
Reference [ReferenceToTestFileInstructions].
```

---

## 7. Baseline Comparison — After Setup

**When:** Phase 5 — repeating the baseline prompt after configuration.

**Prompt:**
```
How should I add a new endpoint to this project? What files do I need to create or modify?
```

> **Expected improvement:** Copilot should now reference minimal API (`app.MapPost`),
> the `*Endpoints.cs` / `*Service.cs` / `*Repository.cs` layers, Dapper with parameterized SQL,
> and the correct naming conventions — without being told in the prompt.

---

## 8. Validate Instruction File Scope

**When:** Optional — to verify that `applyTo` is working correctly.

**Prompt (use with a test file open, e.g. `BeersServiceTests.cs`):**
```
I'm about to add a new test method to this file.
Before I do, summarize the rules you're applying from the instruction files
for this file type.
```

> **What to check:** Copilot should reference the test-specific rules
> (xUnit, naming pattern, Moq, edge cases) without you having included them in the prompt.

---

## Take-Home Patterns

| Prompt Goal | When to use it |
|---|---|
| Draft global instructions | When onboarding Copilot to a new or existing project |
| Draft pattern instruction file | When a file type has specific rules (tests, SQL, API layer) |
| Draft reusable prompt file | When a workflow is repeated across the team |
| Baseline comparison | When validating that instructions improved Copilot output |
| Validate instruction scope | When debugging why Copilot isn't following expected rules |
