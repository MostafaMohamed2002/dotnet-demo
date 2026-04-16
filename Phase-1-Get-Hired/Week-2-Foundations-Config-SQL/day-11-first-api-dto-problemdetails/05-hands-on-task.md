# HANDS-ON TASK

## Task
Build a CRUD API for "Products" and "Categories" using an in-memory list. Implement a strict DTO strategy and standardized error handling.

## Files to Create / Modify
- `Models/Entities/Product.cs` & `Category.cs` (Internal models)
- `Models/Dtos/ProductDtos.cs` (Create, Update, and Response DTOs)
- `Controllers/ProductsController.cs` (CRUD logic)
- `Program.cs` (Add ProblemDetails)

## Expected Behavior
1. **POST `/api/products`**: Accepts `CreateProductDto`. Validates that `Price > 0`. If not, returns a `Problem()` response (400).
2. **GET `/api/products/{id}`**: Returns `ProductResponseDto`. Returns `NotFound()` (404) if the ID doesn't exist.
3. **PUT `/api/products/{id}`**: Accepts `UpdateProductDto`. Returns 204 No Content on success.
4. **GET `/api/products?category=Electronics`**: Returns a filtered list of `ProductResponseDto`.

## Do NOT Do This
- Do not return the `Product` entity directly from any controller method.
- Do not return raw strings for errors (e.g., `return BadRequest("Error here")`). Use `Problem()`.

## Self-Review Checklist
- [ ] Are there 3 distinct DTO classes for the Product resource?
- [ ] Does `Program.cs` have `builder.Services.AddProblemDetails()`?
- [ ] Does the `Get` method use `[FromRoute]` and the `Filter` method use `[FromQuery]`?
- [ ] Does the `Create` method use `[FromBody]`?
- [ ] Does the `Update` method return `NoContent()`?
