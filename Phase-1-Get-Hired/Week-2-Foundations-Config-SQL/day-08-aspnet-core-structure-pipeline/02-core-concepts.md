# CORE CONCEPT BREAKDOWN

## 1. The Entry Point: `Program.cs`
In .NET 6+, the `Program.cs` file uses "Top-Level Statements," removing the boilerplate `Main` method. It is split into two distinct phases: **The Builder** and **The App**.

```csharp
var builder = WebApplication.CreateBuilder(args);

// PHASE 1: THE BUILDER (Service Registration)
// This is where you configure the DI container. 
// "I want the app to know how to create a DoctorRepository"
builder.Services.AddControllers(); 
builder.Services.AddDbContext<ClinicDbContext>(options => ...);

var app = builder.Build();

// PHASE 2: THE APP (Middleware Pipeline)
// This is where you define the order of request processing.
// "First check auth, then route, then execute controller"
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 2. The Middleware Pipeline
Middleware are components that assemble into a pipeline. Each component decides:
1. Does it process the request?
2. Does it pass the request to the `next` middleware in the chain?
3. Does it "short-circuit" (return a response immediately and stop the chain)?

**The Request Flow:**
`Request` $\rightarrow$ `Middleware 1` $\rightarrow$ `Middleware 2` $\rightarrow$ `Controller` $\rightarrow$ `Middleware 2` $\rightarrow$ `Middleware 1` $\rightarrow$ `Response`

**ASCII Pipeline Visualization:**
```text
Client Request
      |
      v
+-------------------+
|  HTTPS Redirection|  <-- "Are you using HTTP? Go to HTTPS."
+-------------------+
      |
      v
+-------------------+
|   Authentication |  <-- "Who are you? Is your token valid?"
+-------------------+
      |
      v
+-------------------+
|   Authorization   |  <-- "You are 'User', but are you an 'Admin'?"
+-------------------+
      |
      v
+-------------------+
|      Routing      |  <-- "Which Controller Action matches /api/doctors/1?"
+-------------------+
      |
      v
+-------------------+
| Controller Action |  <-- Business Logic Execution
+-------------------+
      |
      v
[Response travels back up the pipeline]
```

## 3. Controllers & Attribute Routing
While Minimal APIs exist, enterprise .NET uses **Controllers**. These are classes inheriting from `ControllerBase` that group related actions.

```csharp
[ApiController] // Enables automatic 400 Bad Request responses for model validation
[Route("api/[controller]")] // Route will be /api/doctors
public class DoctorsController : ControllerBase
{
    // GET /api/doctors
    [HttpGet]
    public IActionResult GetAll() 
    {
        return Ok(new List<string> { "Dr. Ahmed", "Dr. Sara" });
    }

    // GET /api/doctors/5
    [HttpGet("{id}")] // Route parameter
    public IActionResult GetById(int id) 
    {
        if (id <= 0) return BadRequest("Invalid ID");
        return Ok($"Doctor {id} details");
    }
}
```

## 4. The Life of a Request (The Sequence)
1. **Routing**: The framework matches the URL to a Controller Action.
2. **Model Binding**: The framework converts the JSON body or Query String into C# objects (e.g., `int id`).
3. **Action Filters**: Code that runs before/after the action.
4. **Controller Action**: Your logic executes.
5. **Response Serialization**: The return value (e.g., `Ok()`) is converted back to JSON.
