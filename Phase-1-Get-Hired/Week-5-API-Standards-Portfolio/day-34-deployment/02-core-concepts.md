# CORE CONCEPT BREAKDOWN

## 1. The Deployment Pipeline
The journey from code to URL generally follows this path:
`Local Code` $\rightarrow$ `Git Push` $\rightarrow$ `CI/CD Pipeline (Build/Test)` $\rightarrow$ `Artifact (Docker Image/Binary)` $\rightarrow$ `Cloud Host` $\rightarrow$ `Public URL`.

## 2. Environment Variables (EnvVars)
Hardcoding secrets (like your DB password) in `appsettings.json` is a critical security failure. In production, we use Environment Variables.

**How .NET handles this:**
The `Configuration` system in .NET is additive. It reads `appsettings.json` first, then overrides those values with environment variables.
- A variable named `ConnectionStrings__DefaultConnection` in the OS will override the `ConnectionStrings:DefaultConnection` key in your JSON.

## 3. Railway vs. Azure
Both are "Platform as a Service" (PaaS) providers.

- **Railway.app**: High developer velocity. It detects your `.NET` project, builds it via Nixpacks/Docker, and provides an automatic HTTPS URL.
- **Azure App Service**: The "industry standard" for .NET. Offers deep integration with Azure SQL Database and Azure Active Directory.

## 4. The Production Checklist
To successfully deploy, the app must satisfy these requirements:
1. **Port Binding**: The app must listen on the port provided by the platform (usually via the `PORT` env var).
2. **HTTPS**: Production traffic must be encrypted. PaaS providers usually handle the SSL termination at the load balancer.
3. **Database Reachability**: The production app must have a network path to the production database (e.g., Azure SQL or Railway PostgreSQL/MySQL).
