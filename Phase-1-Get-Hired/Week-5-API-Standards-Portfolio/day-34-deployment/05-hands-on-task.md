## Task
Deploy the Clinic Appointment Booking API to a cloud provider (Railway.app or Azure App Service) and verify it is fully operational.

## Files to Create / Modify
- `Program.cs` — Ensure Swagger is configured to be accessible in Production if you want to verify endpoints via UI, or ensure it's strictly Development.
- `appsettings.json` — Remove all real secrets; replace them with placeholders (e.g., `"JwtKey": "Your_Key_Here"`) to indicate where EnvVars will override.
- `.gitignore` — Ensure `appsettings.Development.json` and any `.env` files are ignored to prevent accidental secret leaks to GitHub.

## Expected Output / Behavior
1. **Public URL**: The API is accessible via an `https://...` address.
2. **Environment Overrides**: The API connects to the production database using the connection string set in the platform's Dashboard.
3. **Authentication**: You can register and login via the public URL, receiving a JWT that is valid for that environment.
4. **Verification**: You can hit at least one protected endpoint (e.g., `/auth/me`) using the production JWT.

## Do NOT Do This
- **Do not commit your production connection string to Git.**
- **Do not use `http` for production requests.**
- **Do not use a weak JWT secret in production** (use at least 32 characters of random alphanumeric text).

## Self-Review Checklist
- [ ] I have checked the platform logs to ensure the app started without `DbUpdateException`.
- [ ] I have verified that `ASPNETCORE_ENVIRONMENT` is set correctly.
- [ ] I have tested the full "Happy Path": Register $\rightarrow$ Login $\rightarrow$ Book Appointment $\rightarrow$ View Appointment.
- [ ] I have confirmed that the public Swagger UI is either working or intentionally disabled.
