## Task

Implement a complete Authentication system for the Clinic Booking API.

## Files to Create / Modify

1. **Configuration**: `appsettings.json` (add Jwt section) and `Program.cs` (Identity and JWT setup).
2. **Services**: `Interfaces/IAuthService.cs` and `AuthService.cs` (handle Register and Login logic).
3. **JWT Service**: `Interfaces/IJwtProvider.cs` and `JwtProvider.cs` (handle token generation).
4. **Controllers**: `AuthController.cs` (endpoints for `/register` and `/login`).
5. **Protection**: Add `[Authorize]` and `[Authorize(Roles = "Admin")]` to existing Appointment and Doctor controllers.

## Expected Output / Behavior

1. **Registration**: User can create an account. The password must be hashed by `UserManager`.
2. **Login**: User provides credentials $\rightarrow$ API returns a JWT string.
3. **Authorization**:
   - A request with no token $\rightarrow$ `401 Unauthorized`.
   - A request with a "Patient" token to an Admin endpoint $\rightarrow$ `403 Forbidden`.
   - A request with a valid "Admin" token $\rightarrow `200 OK`.
4. **Verification**: Use Postman to login, copy the token, and add it to the `Authorization: Bearer <token>` header for subsequent requests.

## Do NOT Do This

- **Do NOT store passwords in plaintext**.
- **Do NOT return the JWT secret key in any API response**.
- **Do NOT use the full Identity scaffolded UI (Razor Pages)**.

## Self-Review Checklist

- [ ] Did I use `AddIdentityCore<TUser>()` in `Program.cs`?
- [ ] Does the `JwtProvider` use `SymmetricSecurityKey` for signing?
- [ ] Is the `[Authorize]` attribute applied to the correct controllers?
- [ ] Does the login endpoint verify the password using `UserManager.CheckPasswordAsync()`?
