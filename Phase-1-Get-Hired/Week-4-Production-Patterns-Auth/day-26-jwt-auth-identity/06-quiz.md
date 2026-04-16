# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: What is the fundamental difference between "Authentication" and "Authorization"?
2. **Conceptual**: If a JWT is stateless, how does the server know that a user has the "Admin" role without checking the database on every request?
3. **Code-based**: You are seeing a `401 Unauthorized` response, but you are sure you sent a valid token in the header. What is the most likely configuration error in `Program.cs` regarding the `TokenValidationParameters`?
4. **Code-based**: Write the C# line that allows a specific endpoint (like `/login`) to be accessed by anyone, even if the controller is marked with `[Authorize]`.
5. **Scenario**: A client-side developer tells you that the JWT is too large and is slowing down requests. They suggest removing the "Claims" (User ID, Role) from the token and just sending the `UserId` in the token. How would this change the server's behavior?
