## Task

Rebuild the Products + Categories CRUD API from scratch, applying every professional standard learned in Week 3. This will be your second major GitHub repository.

## Requirements

### 1. Technical Stack
- **SQL Server** + **EF Core** (Code-First).
- **C# 12** / **.NET 8**.
- **DTOs** for all Requests and Responses.
- **ProblemDetails** for error responses.
- **CORS** configured for `http://localhost:3000`.

### 2. Performance Constraints
- All `GET` requests must use `.AsNoTracking()`.
- All related data (Product $\rightarrow$ Category) must be fetched via `.Include()` to avoid N+1.
- Connection strings must reside in `appsettings.json`.

### 3. Git Standard
You must exhibit a "living" commit history. Do not push a single "Initial Commit."

## Expected Output / Behavior

- A fully functional API where you can:
  - Create/Read/Update/Delete Categories.
  - Create/Read/Update/Delete Products (linked to a Category).
- When a Product is requested, the response must include the Category name.
- When a non-existent Product is requested, the API must return a `ProblemDetails` response with a `404 Not Found` status.
- The API must be accessible from `http://localhost:3000` (verify via `curl` OPTIONS request).

## Do NOT Do This

- **Do NOT return Entities** in your controllers.
- **Do NOT hardcode connection strings**.
- **Do NOT use `IEnumerable` for filtering**; use `IQueryable`.

## Self-Review Checklist

- [ ] Did I use `AsNoTracking()` on every single GET endpoint?
- [ ] Did I verify the N+1 problem is gone by checking the SQL logs?
- [ ] Is the CORS policy named and restricted to `localhost:3000`?
- [ ] Does the commit history show a logical evolution (Entities $\rightarrow$ DbContext $\rightarrow$ CRUD $\rightarrow$ Optimization)?
- [ ] Are the error responses using `ProblemDetails`?
