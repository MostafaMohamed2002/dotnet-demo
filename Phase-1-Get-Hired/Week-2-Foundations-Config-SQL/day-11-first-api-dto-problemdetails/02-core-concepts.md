# CORE CONCEPT BREAKDOWN

## 1. The DTO Pattern (Data Transfer Objects)
A common junior mistake is returning a database entity directly: `return Ok(doctorEntity);`. 
**Why this is dangerous:**
- **Security**: You might accidentally leak a `PasswordHash` or `InternalNotes` field.
- **Coupling**: If you rename a column in SQL, the JSON property name changes, and the Android app crashes.
- **Over-fetching**: You might only need the `DoctorName`, but the entity returns 50 fields.

**The Professional Approach: Triple-DTO Strategy**
For a `Product` resource, we create:
1. `CreateProductDto`: Only fields required for creation (e.g., `Name`, `Price`).
2. `UpdateProductDto`: All fields optional or specific to updates.
3. `ProductResponseDto`: The "View" model. Includes the `Id` and formatted data.

```csharp
// The Entity (Internal/DB)
public class Product { 
    public int Id { get; set; } 
    public string Name { get; set; } 
    public decimal Price { get; set; } 
    public decimal CostPrice { get; set; } // SECRET! Never expose this.
}

// The Response DTO (External/API)
public record ProductResponseDto(int Id, string Name, decimal Price);
```

## 2. ProblemDetails (RFC 7807)
Stop returning `return BadRequest("Invalid age");`. Professional APIs use **ProblemDetails**. It is a standardized JSON format for errors so that clients (like Android apps) can parse errors programmatically.

**Standard ProblemDetails JSON:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One Of Your Fields Is Wrong",
  "status": 400,
  "detail": "The 'Price' field must be greater than zero.",
  "instance": "/api/products/create"
}
```

**Implementation in .NET 8:**
```csharp
// In Program.cs
builder.Services.AddProblemDetails();

// In Controller
if (price <= 0) {
    return Problem(
        detail: "Price must be greater than zero", 
        statusCode: 400
    );
}
```

## 3. Model Binding Attributes
You must explicitly tell .NET where to find the data for a method parameter:

| Attribute | Source | Use Case | Example |
|---|---|---|---|
| `[FromBody]` | Request Body | Complex objects (JSON) | `CreateProductDto` |
| `[FromRoute]` | URL Path | Unique identifiers | `/api/products/{id}` |
| `[FromQuery]` | Query String | Filtering, Pagination | `/api/products?category=electronics` |

**Code Example:**
```csharp
[HttpPost]
public IActionResult Create(
    [FromBody] CreateProductDto dto, // JSON body
    [FromQuery] string source // ?source=web
) { ... }
```
