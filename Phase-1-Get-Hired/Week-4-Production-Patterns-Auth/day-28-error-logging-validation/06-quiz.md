# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: What is the a "ProblemDetails" response, and why is it preferred over a simple string or custom error object?
2. **Conceptual**: Explain the "Performance Cliff" of using string interpolation in logs versus structured logging. What happens at the log-aggregation level?
3. **Code-based**: You are writing a validator. Which FluentValidation method would you use to ensure a string is not null or empty, and that it has a maximum length of 100 characters?
4. **Code-based**: Look at this middleware snippet. Where should the `_next(context)` call be placed to ensure that the middleware can catch exceptions from subsequent components in the pipeline?
   ```csharp
   public async Task InvokeAsync(HttpContext context) {
       // Line A
       try {
           // Line B
       } catch (Exception ex) {
           // Line C
       }
   }
   ```
5. **Scenario**: A user reports a bug that happened "yesterday at 3 PM." You have 10 GB of logs. How does a **Correlation ID** allow you to find the exact sequence of events for that specific user's request in seconds?
