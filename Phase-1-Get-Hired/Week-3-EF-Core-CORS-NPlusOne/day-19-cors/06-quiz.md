# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: If an API works perfectly in Postman but fails in a Chrome browser with a "CORS error," does this mean the server-side logic (the Controller) is failing? Explain why.
2. **Conceptual**: What is the purpose of the `OPTIONS` request (Preflight), and why is it sent?
3. **Code-based**: In `Program.cs`, you have the following order:
   ```csharp
   app.UseRouting();
   app.UseAuthorization();
   app.UseCors("MyPolicy");
   ```
   Why is this order problematic? What will happen if a user is not authorized?
4. **Code-based**: Which method should you use if you want to allow only specific domains to access your API while also allowing the use of cookies/credentials?
5. **Scenario**: Your frontend team tells you that they are adding a new custom header called `X-Clinic-Client-Version`. Suddenly, all their requests are failing with CORS errors. What is the most likely fix in your `AddCors` configuration?
