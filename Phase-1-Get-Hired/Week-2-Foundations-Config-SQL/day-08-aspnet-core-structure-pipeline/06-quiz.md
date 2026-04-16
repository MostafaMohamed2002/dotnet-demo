# QUIZ
Answer all 5 questions. Paste your answers into the chat when ready.

1. **Conceptual**: Explain the difference between the "Builder" phase (`builder.Services`) and the "App" phase (`app.Use...`) in `Program.cs`. What happens if you try to register a service in the App phase?
2. **Code-Based**: Look at the following snippet:
   ```csharp
   app.UseAuthorization();
   app.UseAuthentication();
   app.MapControllers();
   ```
   What is wrong with this configuration?
3. **Conceptual**: What is the specific purpose of the `[ApiController]` attribute on a controller class?
4. **Code-Based**: You need to return a response indicating that a Doctor exists, but the user does not have the "Admin" role required to view their details. Which HTTP status code should you return, and what is the corresponding method in `ControllerBase`?
5. **Scenario**: A request comes in for `/api/doctors/abc`. The `GetById` method expects an `int id`. What happens during the "Model Binding" phase, and what status code will the user likely receive?
