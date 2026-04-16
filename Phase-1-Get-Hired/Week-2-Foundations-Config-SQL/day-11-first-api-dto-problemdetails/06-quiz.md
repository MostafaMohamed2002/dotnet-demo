# QUIZ
Answer all 5 questions. Paste your answers into the chat when ready.

1. **Conceptual**: Explain the "Over-posting Attack." Why does using a DTO instead of an Entity prevent this?
2. **Code-Based**: Which attribute would you use if you want to extract a value from the URL `api/products/123` into a parameter `int id`?
3. **Conceptual**: What is the primary advantage of using `ProblemDetails` (RFC 7807) over returning a simple string like `BadRequest("Invalid ID")`?
4. **Scenario**: You are designing an API. You need a parameter that is optional and used for filtering a list (e.g., `?minPrice=10`). Which model binding attribute should you use?
5. **Scenario**: A user tries to update a product. The request is valid JSON, but the `CategoryId` they provided does not exist in the database. Which HTTP status code is most appropriate here, and should you use `Problem()` to return it?
