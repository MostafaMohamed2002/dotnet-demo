# REAL-WORLD CONTEXT

## Production Implementation
In a professional CI/CD pipeline (GitHub Actions, Azure DevOps, Jenkins), we use the hierarchy to maximize security:
- **Local Dev**: Use `appsettings.Development.json` for non-sensitive defaults and `dotnet user-secrets` for local DB passwords.
- **Production**: Use **Environment Variables** or a Key Vault (Azure Key Vault, HashiCorp Vault). 

## The "Code Review Red Flag"
If a senior engineer sees a connection string or an API key inside a `.json` file in a Pull Request, it is an immediate **Request Changes**. 

**Why?** Even if the repo is private, committing secrets creates a "permanent record" in the git history. Rotating a leaked secret is a nightmare because you have to rewrite git history or invalidate the key immediately.

## Clinic API Case Study: JwtSettings
In our Clinic API, we will have a `JwtSettings` class. 
- In `appsettings.json`, we might set `ExpiryMinutes: 60`.
- In `appsettings.Development.json`, we might set `ExpiryMinutes: 1440` (24 hours) because developers don't want to re-login every hour while debugging.
- The `Secret` key will live in User Secrets locally and as an Environment Variable in the production container.
