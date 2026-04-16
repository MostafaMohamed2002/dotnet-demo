# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: Why is returning a DTO instead of an Entity considered a security requirement and not just a "coding preference"?
2. **Conceptual**: Describe the "Performance Cliff" that happens when you call `.ToList()` before a `.Where()` clause. What happens to the data in terms of RAM and Network?
3. **Code-based**: You are implementing a `GET /products/{id}` endpoint. Write the minimal C# code to fetch the product and its category, ensuring it is read-only and uses the correct EF Core method for related data.
4. **Code-based**: If you are using a named CORS policy called `"FrontendPolicy"`, what two specific lines of code are required in `Program.cs` to make it work? (One in services, one in middleware).
5. **Scenario**: You have successfully implemented the API. A senior dev asks you to "harden" the API for production. They suggest removing `AllowAnyOrigin()` and adding `AsNoTracking()` to a few remaining GET queries. Which of these two changes has a more significant impact on **security**, and which has a more significant impact on **performance**? Explain.
