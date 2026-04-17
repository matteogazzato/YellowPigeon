# Lab 1 — “From Ticket to Working Code” (API endpoint + validation)

## Goal 
Turn a realistic work item into an implementation quickly, with good structure.

# Scenario
Add a feature: Create Order endpoint that validates input, saves to DB, returns a DTO.
* C# (ASP.NET Core minimal API or controller)
* SQL table Orders and OrderItems

# What they practice (Copilot daily usage)
* Asking Copilot to propose an implementation plan from a user story
* Generating scaffolding (DTOs, endpoints, service, repository)
* Iterating: “Now add validation”, “Now handle errors”, “Now return proper HTTP codes”

# 60-min flow
* 0–5: context + acceptance criteria (tiny)
* 5–20: Copilot generates skeleton + wiring
* 20–40: implement business rules + error handling
* 40–55: quick manual test + small improvements
* 55–60: recap: “what prompts worked best”

# Deliverables
* Working endpoint
* DTOs + validation rules

# User Story
## Title: Create Order API — Persist Order with Items and Return Order Summary

As a client application (web/mobile/internal tool)
I want to create a new order with one or more order items
So that the order is saved in the existing database and can be processed later.

# Context / Existing Constraints
* The database already exists with tables: Orders, OrderItems (and optionally Customers, Products).
* The implementation should follow a clean structure (API → Service → Data Access).

# Acceptance Criteria
## AC1 — Endpoint Contract
* POST /api/orders
* Request body contains:
    * customerId (required)
    * currency (required, e.g., “EUR”)
    * items (required, array with at least 1 item)
        * each item has productCode (required), quantity (required), unitPrice (required)

## AC2 — Validation
* Return 400 Bad Request with a clear error payload if:
    * customerId is missing or <= 0
    * currency is missing/empty
    * items is null/empty
    * any item has:
    * missing productCode
    * quantity <= 0
    * unitPrice <= 0
* (Optional advanced validation) total must match computed total if a totalAmount is provided.

## AC3 — Persistence
* Insert one row into Orders and N rows into OrderItems.
* All inserts must be atomic (transaction): if any insert fails, nothing is saved.

## AC4 — Response
* On success return 201 Created with:
    * orderId
    * createdAtUtc
    * customerId
    * currency
    * totalAmount (computed from items)
    * itemsCount
* Include Location header pointing to /api/orders/{orderId} (even if GET is not implemented).

## AC5 — Error Handling
* Unexpected errors return 500 Internal Server Error with a generic message (no stack traces).
* If DB constraints fail (e.g., FK), return 409 Conflict (or 400, choose one and document it).

# Definition of Done
* Endpoint implemented + basic validation + transaction handling + returns correct HTTP status codes.
* Minimal manual test or sample request file is provided.