# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: What is the primary goal of the Service Layer in a 3-tier architecture?
2. **Conceptual**: Why is a "Specific Repository" (e.g., `IAppointmentRepository`) generally preferred over a "Generic Repository" (`IRepository<T>`) in complex business applications?
3. **Code-based**: Look at this controller method. What is "wrong" with it from an architectural perspective?
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Create(Product p) {
       if (p.Price < 0) return BadRequest("Price cannot be negative");
       _context.Products.Add(p);
       await _context.SaveChangesAsync();
       return Ok(p);
   }
   ```
4. **Code-based**: You are implementing the Result pattern. Write a simple `Result` class that can return either a value of type `T` on success, or a string error message on failure.
5. **Scenario**: You are reviewing a PR. The developer has created a `PatientService` that takes an `IAppointmentRepository`. They are using the repository to perform complex calculations on appointment dates. Should this logic stay in the repository or move to the service? Why?
