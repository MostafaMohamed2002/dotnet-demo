# REAL-WORLD CONTEXT

## Production Context
In a professional API, you never return the `IdentityUser` object in your response. You create a `UserDto` or `AuthResponse` object.

### The Clinic Case Study: The "Role Escalation" Attack
A junior developer creates a `/update-profile` endpoint that takes a `UserUpdateDto`.
```csharp
public class UserUpdateDto {
    public string Name { get; set; }
    public string Role { get; set; } // Junior included the role here!
}
```
**The Bug**: A malicious patient sends a request to update their name, but they also add `"Role": "Admin"` to the JSON. The developer's code simply maps the DTO to the entity and saves it.
**Result**: The patient is now an Admin and can delete other patients' records.

**The Professional Fix**: Never allow a user to update their own Role via a general-profile endpoint. Role changes must happen in a dedicated `AdminController` protected by `[Authorize(Roles = "Admin")]`.

## Common Junior Mistake: Secret Key in `appsettings.json`
A junior developer puts the JWT Secret Key directly in `appsettings.json` and pushes it to GitHub.

**Why this fails code review**: Anyone with the key can forge valid JWTs for any user, including the Admin.
**The Professional Fix**: Use **Environment Variables** or **Azure Key Vault** for secrets. In `appsettings.Development.json`, you can use a placeholder, but the real key must never be committed to version control.
