# QUIZ
Answer all 5 questions. Paste your answers into the chat when ready.

**1. Conceptual**: Explain the difference between Authentication and Authorization in the context of the Clinic API. Which one happens first?

**2. Code-based**: Look at the following snippet:
```csharp
app.UseAuthorization();
app.UseAuthentication();
```
Is this correct? If not, what is the problem and what is the fix?

**3. Conceptual**: Why do we use a "Secret Key" to sign a JWT, and what happens if that key is leaked to the public?

**4. Code-based**: You need to ensure that only users with the "Admin" role can access the `DeleteDoctor` method in a controller. Write the specific C# attribute you would place above the method.

**5. Scenario-based**: A junior developer suggests storing the user's email and phone number inside the JWT claims so the frontend doesn't have to call a `/profile` endpoint. Based on the "Real-World Context" section, why is this a bad idea?
