# REAL-WORLD CONTEXT

## Production Secrets Management
In a professional enterprise setting, we don't just use platform dashboards for EnvVars. We use **Secret Vaults** (e.g., Azure Key Vault, HashiCorp Vault). The app authenticates with the vault at startup and pulls its secrets into memory.

## Case Study: The "Localhost" Disaster
A common junior mistake is deploying an API but leaving the connection string pointing to `localhost`.
**Result**: The app starts successfully (because the code is fine), but every single API request returns a `500 Internal Server Error` because the cloud server is trying to find a SQL Server instance *inside its own container*, where none exists.

## The Gotcha: Swagger in Production
By default, ASP.NET Core wraps Swagger in `if (app.Environment.IsDevelopment())`. 
If you deploy to production and see a `404` at `/swagger`, it's not because Swagger is broken—it's because the app is now in `Production` mode and has disabled the documentation for security. You must either move Swagger outside the `if` block or set `ASPNETCORE_ENVIRONMENT` to `Development` (not recommended for production).
