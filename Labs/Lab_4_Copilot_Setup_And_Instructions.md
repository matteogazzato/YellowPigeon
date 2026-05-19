# Lab 4 — "Copilot-Ready Project: Setup, Instructions, and Smarter Context"

## Goal
Configure GitHub Copilot correctly on an existing project so it respects the codebase's conventions, generates consistent output, and reduces the need to repeat context in every prompt.

**📋 Prompts:**
Reusable Copilot prompts for this lab are in [Lab_4_Copilot_Setup_And_Instructions_Prompts.md](../PromptsLabs/Lab_4_Copilot_Setup_And_Instructions_Prompts.md).

## Scenario
You've just joined the **BrewTrack** team — a C# ASP.NET Core minimal API for managing a brewery's beer catalog and production batches. The project is in production, has clear conventions, and has zero Copilot configuration. You'll set up global instructions, pattern instruction files, and reusable prompt files, then validate the difference before and after.

## What They Practice
* Writing a `copilot-instructions.md` that reflects a real project's stack and rules
* Creating pattern-specific instruction files with `applyTo` glob rules
* Building reusable `.github/prompts/` files for recurring team tasks
* Validating the setup with a before/after comparison
* Committing the Copilot configuration as a shared team asset

## 60-min flow
* 0–5: Intro — why Copilot gives generic answers without project configuration
* 5–20: Write `copilot-instructions.md` using Copilot itself
* 20–35: Add pattern instruction files (tests, controllers) with `applyTo`
* 35–50: Create two reusable prompt files for team workflows
* 50–60: Before/after comparison, commit, recap

## Deliverables
* A working `copilot-instructions.md` committed to `.github/`
* At least two instruction files with `applyTo` glob rules
* Two reusable prompt files in `.github/prompts/`
* A before/after comparison showing improved Copilot output

---

## Context: Starting Point

**BrewTrack** is a C# ASP.NET Core project with:
- Minimal API endpoints (no controllers — just `app.MapGet / MapPost / MapPut / MapDelete`)
- A layered architecture: `Endpoints → Services → Repositories`
- Dapper for data access (no Entity Framework)
- xUnit for tests, following Arrange/Act/Assert and `MethodName_Scenario_ExpectedBehavior` naming
- SQL Server database with snake_case table and column names (`beers`, `batches`)

The domain covers two aggregates:
- **Beer** — the brewery's recipe catalog (`beer_id`, `name`, `style`, `abv`, `description`, `created_at`)
- **Batch** — production runs linked to a beer, with a status workflow: `planning → brewing → conditioning → ready`

There is currently **no `.github/` folder** and **no Copilot configuration**.

---

## What We Practice

* **Writing global instructions that reflect real conventions** — stack, architecture rules, naming, and what Copilot should avoid
* **Scoping rules with `applyTo`** — different instructions for test files, services, and repositories without polluting the global config
* **Encoding team workflows as prompt files** — reusable prompts for adding endpoints or writing tests, shared through git
* **Validating with before/after comparison** — measuring whether the setup actually improved Copilot output
* **Committing Copilot config as a team asset** — making the setup available to every developer who clones the repo

---

## Hands-On Flow

### Phase 1: The Baseline — Before Any Setup (0–5 min)

Before touching any configuration, run a baseline test. Ask Copilot a question that depends on project knowledge.

**Baseline question (save the response):**
```
How should I add a new endpoint to this project? What files do I need to create or modify?
```

Copilot will give a generic answer — probably mentioning controllers, Entity Framework, or some other pattern that doesn't match this project. Save the response to compare later.

---

### Phase 2: Write `copilot-instructions.md` (5–20 min)

You'll now create the global instruction file that Copilot reads for every interaction in this repo.

**Step 1: Open GitHub Copilot Chat and use Prompt 1** from [Lab_4_Copilot_Setup_And_Instructions_Prompts.md](../PromptsLabs/Lab_4_Copilot_Setup_And_Instructions_Prompts.md#1-draft-copilot-instructionsmd).

Review the output. Make sure it covers:
- Technology stack: C# ASP.NET Core minimal API, Dapper, SQL Server, xUnit
- Architecture pattern: Endpoints → Services → Repositories
- Naming conventions: PascalCase for classes, snake_case for SQL
- What NOT to use: Entity Framework, MVC controllers
- Test conventions: Arrange/Act/Assert, `MethodName_Scenario_ExpectedBehavior`

**Step 2: Create the file**
```
.github/copilot-instructions.md
```
Paste the reviewed content. Save and commit.

**Reference output for `.github/copilot-instructions.md`:**
```markdown
## Project Overview
BrewTrack is a C# ASP.NET Core application for managing a brewery's beer catalog
and production batches. The architecture follows a layered pattern:
Endpoints → Services → Repositories. Data access uses Dapper (no Entity Framework).
The database is SQL Server with snake_case table and column names.

## Technology Stack
- C# / ASP.NET Core (.NET 10)
- Minimal API (no MVC controllers, no [ApiController] attribute)
- Dapper for all data access
- SQL Server
- xUnit for unit tests
- Dependency injection via built-in ASP.NET Core DI

## Architecture Rules
- New features follow the Endpoints → Services → Repositories layering strictly.
- Endpoints are defined in `Program.cs` or dedicated `*Endpoints.cs` files using `app.Map*`.
- Services contain business logic and are injected via interfaces.
- Repositories contain all SQL queries. Use parameterized queries with Dapper always.
- Never use Entity Framework, LINQ-to-SQL, or raw string interpolation in SQL.

## Domain
- **beers** table: `beer_id`, `name`, `style`, `abv`, `description`, `created_at`
- **batches** table: `batch_id`, `beer_id`, `brewed_at`, `volume_liters`, `status`, `notes`
- Batch status workflow (enforced at the service layer): `planning → brewing → conditioning → ready`

## Naming Conventions
- C#: PascalCase for classes, methods, properties. camelCase for local variables.
- SQL: snake_case for table and column names (e.g., `beers`, `brewed_at`).
- Tests: `MethodName_Scenario_ExpectedBehavior` naming pattern.

## Testing Rules
- Use xUnit. Arrange/Act/Assert pattern for all tests.
- Unit tests mock dependencies using interfaces (Moq).
- Do not test private methods directly.

## What Copilot Should NOT Do
- Do not suggest Entity Framework or DbContext.
- Do not generate MVC-style controllers or [HttpGet] / [HttpPost] attributes.
- Do not generate LINQ-to-Objects queries as a replacement for SQL.
- Do not skip the service layer — endpoints must never call repositories directly.
```

---

### Phase 3: Add Pattern Instruction Files (20–35 min)

Pattern instruction files let you give Copilot **different rules depending on which file is open**. You'll create two.

#### 3a — Instruction file for test files

**Use Prompt 3** from [Lab_4_Copilot_Setup_And_Instructions_Prompts.md](../PromptsLabs/Lab_4_Copilot_Setup_And_Instructions_Prompts.md#3-draft-instruction-file-for-tests).

Create: `.github/instructions/tests.instructions.md`

```markdown
---
applyTo: "**/*.Tests/**/*.cs,**/*Tests.cs,**/*Test.cs"
---

When generating or modifying test files for BrewTrack:
- Use xUnit with Arrange/Act/Assert structure.
- Name tests as `MethodName_Scenario_ExpectedBehavior`.
- Mock service or repository dependencies using Moq and constructor injection with interfaces.
- Do not use static helpers or test frameworks other than xUnit.
- Always include at least one edge case (null input, zero, empty collection, invalid status transition).
- Assert on specific values, not just that the result is not null.
```

#### 3b — Instruction file for service and repository files

**Use Prompt 4** from [Lab_4_Copilot_Setup_And_Instructions_Prompts.md](../PromptsLabs/Lab_4_Copilot_Setup_And_Instructions_Prompts.md#4-draft-instruction-file-for-services-and-repositories).

Create: `.github/instructions/services-repositories.instructions.md`

```markdown
---
applyTo: "**/*Service.cs,**/*Repository.cs"
---

When generating or modifying Service or Repository files for BrewTrack:
- Services expose an interface (e.g., `IBeersService`, `IBatchesService`) and are registered in DI.
- Services contain business logic only — no SQL queries, no HTTP concerns.
- Repositories contain only data access logic using Dapper.
- All SQL queries in repositories must be parameterized. No string interpolation.
- Repositories are injected into services via their interface.
- Return domain models or DTOs from repositories, never raw DataTable or dynamic.
- Batch status transitions (planning → brewing → conditioning → ready) are validated
  in the service layer, never in the repository or endpoint.
```

**Verify the setup:** open a `*Service.cs` file (e.g., `BeersService.cs`) and ask Copilot to add a method. Check that the suggestion follows the interface/Dapper conventions without being told.

---

### Phase 4: Create Reusable Prompt Files (35–50 min)

Prompt files are Markdown files in `.github/prompts/` that you can call from chat with `/` or `#`. They encode team-level workflows as reusable prompts.

#### 4a — Prompt file for adding a new endpoint

**Use Prompt 5** from [Lab_4_Copilot_Setup_And_Instructions_Prompts.md](../PromptsLabs/Lab_4_Copilot_Setup_And_Instructions_Prompts.md#5-draft-prompt-file-for-adding-an-endpoint).

Create: `.github/prompts/add-endpoint.prompt.md`

```markdown
---
description: Add a new minimal API endpoint following BrewTrack conventions
mode: ask
---

I need to add a new endpoint to BrewTrack. Here is the feature:

[DESCRIBE THE FEATURE: which entity (beer or batch), what operation, what input, what output]

Generate:
1. The endpoint registration in `Program.cs` or a dedicated `*Endpoints.cs` file using `app.Map*`.
2. A service method in the relevant `*Service.cs`, injected via interface.
3. A repository method in the relevant `*Repository.cs` using Dapper with parameterized SQL.
4. Any required request/response DTOs.

Follow all conventions in `.github/copilot-instructions.md` and `.github/instructions/services-repositories.instructions.md`.
```

#### 4b — Prompt file for writing tests

Create: `.github/prompts/write-tests.prompt.md`

```markdown
---
description: Write xUnit tests for a BrewTrack service or repository method
mode: ask
---

Write xUnit tests for the following method in BrewTrack:

[PASTE THE METHOD CODE OR DESCRIBE IT]

Requirements:
- Follow Arrange/Act/Assert structure.
- Use `MethodName_Scenario_ExpectedBehavior` naming.
- Include the happy path and at least two edge cases.
- Mock all dependencies using Moq and interfaces.
- Follow conventions in `.github/instructions/tests.instructions.md`.
```

**Try it:** open chat, type `#add-endpoint.prompt.md` (or use `/` in VS Code), and describe a new feature. For example: *"Add an endpoint to get all batches for a specific beer by ID."* Observe how the response follows all the project conventions automatically.

---

### Phase 5: Before/After Comparison and Commit (50–60 min)

**Repeat the baseline question** you asked in Phase 1:
```
How should I add a new endpoint to this project? What files do I need to create or modify?
```

Compare the two responses:
- Does Copilot now mention minimal API, Dapper, and the layered pattern?
- Does it avoid suggesting Entity Framework or controllers?
- Does it reference the right naming conventions?

**Commit everything:**
```bash
git add .github/
git commit -m "Add Copilot configuration: instructions and prompt files"
```

From now on, every developer who clones this repo gets Copilot pre-configured for the project.

---

## Reusable Setup Patterns

| Pattern | When to use |
|---|---|
| `copilot-instructions.md` | Always — one per repo, global rules for the whole project |
| `*.instructions.md` with `applyTo` | When specific file types need specific rules (tests, SQL, controllers) |
| `*.prompt.md` in `.github/prompts/` | For recurring tasks the whole team does (add feature, write tests, generate migration) |
| Before/after comparison | When training the team or validating that instructions are working |
| Commit Copilot config in git | Always — the setup travels with the repo and helps onboarding |

---

## Success Criteria

By the end of Lab 4, you should have:
- ✅ A `copilot-instructions.md` reflecting the real project conventions
- ✅ At least two pattern instruction files with `applyTo` glob rules
- ✅ Two reusable prompt files committed in `.github/prompts/`
- ✅ Visible improvement in Copilot output quality (before/after comparison)
- ✅ All Copilot configuration committed and shareable with the team

---

## Takeaways

1. **Copilot is only as good as the context it has** — global instructions give it permanent, repo-level context.
2. **Pattern instructions are surgical** — they apply only where relevant, without polluting the global rules.
3. **Prompt files encode team knowledge** — once a prompt works, share it so everyone gets the same quality.
4. **The setup is a one-time investment** — after committing, every developer on the team benefits immediately.
5. **Iterate** — improve instructions over time as you find gaps in Copilot's output.
