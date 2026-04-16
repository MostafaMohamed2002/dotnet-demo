# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: Why is it considered a "best practice" to return a `PagedResult<T>` envelope instead of a raw JSON array?
2. **Conceptual**: Describe the "Performance Cliff" that occurs when you call `.ToList()` before `.Skip()` and `.Take()`.
3. **Code-based**: You have an `IQueryable<Product> query`. Write the LINQ chain to get the 3rd page of results with a page size of 20, sorted by Price descending.
4. **Code-based**: Your API supports a `sortBy` query parameter. How do you handle a request where the user passes a value that doesn't match any of your available columns (e.g., `?sortBy=Sausage`)?
5. **Scenario**: You are building a "Search" feature for patients. The user can search by Name, Email, or Phone. If any of these three fields match, the record should be returned. How do you implement this on the `IQueryable` without causing a performance bottleneck?
