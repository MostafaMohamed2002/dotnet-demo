# Summary — Day 10: Configuration Management

## What You Learned
- The layering system: JSON $\rightarrow$ Environment Variables $\rightarrow$ CLI Args.
- How to use `appsettings.{Environment}.json` to specialize behavior for Development vs Production.
- The `IOptions<T>` pattern for strongly-typed, injectable configuration.
- The security necessity of `dotnet user-secrets` for local development.
- The standard convention for accessing database connection strings via `GetConnectionString()`.

## What This Unlocks
Your application is now decoupled from its environment. You can move the same binary from a laptop to a cloud server and change its database or security keys without recompiling.

## Revisit Before Next Session
- Practice the `dotnet user-secrets` CLI commands.
- Understand why `IOptions<T>` is preferred over reading `IConfiguration` directly (Testability/Dependency Inversion).
