# CORE CONCEPT BREAKDOWN

## The Integration Blueprint
To rebuild the Products + Categories API, you must apply the following architectural constraints.

### 1. The Data Layer (EF Core)
- **Entities**: Create `Product` and `Category` entities.
- **Relationship**: One Category $\rightarrow$ Many Products.
- **Code-First**: Use `dotnet ef migrations` to evolve the schema.
- **Connection**: Use `GetConnectionString("DefaultConnection")` from `appsettings.json`.

### 2. The Application Layer (DTOs)
Never return Entities directly to the client. This prevents "Overposting" attacks and avoids circular reference errors during JSON serialization.
- **Request DTOs**: `CreateProductRequest`, `UpdateProductRequest`.
- **Response DTOs**: `ProductResponse`, `CategoryResponse`.

### 3. The Controller Layer (Performance & Standards)
- **Read-Only Operations**: Every `GET` request must use `.AsNoTracking()`.
- **N+1 Prevention**: Use `.Include(p => p.Category)` when returning a product with its category name.
- **Error Handling**: Use `ProblemDetails` for all errors (e.g., `return Problem(detail: "Product not found", statusCode: 404)`).
- **CORS**: Configure a named policy allowing `http://localhost:3000`.

### 4. The Git Workflow (Professionalism)
Do not upload a finished project in one commit. A senior dev reviews your **process**, not just the result.

**Recommended Commit Sequence:**
1. `feat: setup project structure and appsettings`
2. `feat: define entities and initial migration`
3. `feat: implement category crud with dtos`
4. `feat: implement product crud with eager loading`
5. `feat: configure cors and problem details`
6. `perf: add asnotracking to all get queries`
