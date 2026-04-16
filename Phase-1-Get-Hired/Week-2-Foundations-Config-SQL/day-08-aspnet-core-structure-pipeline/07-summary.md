# Summary — Day 08: ASP.NET Core Project Structure & Request Pipeline

## What You Learned
- The two-phase lifecycle of `Program.cs`: Service registration (DI) $\rightarrow$ Middleware pipeline configuration.
- The sequential nature of Middleware and the "Russian Doll" model of request/response flow.
- Controller-based routing using `[Route]` and `[HttpGet("{id}")]`.
- The critical sequence of Routing $\rightarrow$ Model Binding $\rightarrow$ Action Execution $\rightarrow$ Serialization.
- The semantic difference between 401 (Who are you?) and 403 (You aren't allowed).
- How to use `ControllerBase` methods like `Ok()`, `BadRequest()`, and `NotFound()` to control HTTP responses.

## What This Unlocks
You can now build the routing layer for any API. You understand how to intercept requests globally (Middleware) and how to handle specific resources (Controllers).

## Revisit Before Next Session
- Ensure you are comfortable with the "Middleware Order" concept, as it's the #1 source of bugs in ASP.NET Core security.
- Review the difference between Route parameters (`{id}`) and Query strings (`?name=...`).
