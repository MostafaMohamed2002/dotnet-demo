# CORE CONCEPT BREAKDOWN

## 1. Database Schema Design
The Clinic API requires a relational model to ensure data integrity.

### The Entity Relationship
- **Users**: Central table for authentication.
- **Doctors**: Extension of User (One-to-One). Contains specialization and experience.
- **Patients**: Extension of User (One-to-One). Contains medical history/contact info.
- **Appointments**: The junction table. Links one Doctor to one Patient at a specific UTC time.

```ascii
[User] 1 <--- 1 [Doctor]
  ^               |
  |               | 1
  |               |
  |               v
  |          [Appointment]
  |               ^
  |               | 1
  |               |
[User] 1 <--- 1 [Patient]
```

## 2. EF Core Migrations
Migrations allow us to evolve the database schema without losing data.

**The Workflow:**
1. Define `DbContext` and `DbSet<T>`.
2. Run `dotnet ef migrations add [Name]`. This creates a snapshot of the model.
3. Run `dotnet ef database update`. This applies the snapshot to the SQL Server instance.

## 3. JWT (JSON Web Tokens)
JWTs are stateless tokens used for authorization. Unlike session cookies, the server doesn't need to store the token—it only needs the **Secret Key** to verify the signature.

### Token Structure:
`Header.Payload.Signature`

- **Header**: Algorithm and token type.
- **Payload**: Claims (User ID, Role, Expiration).
- **Signature**: HASH(Header + Payload + SecretKey).

### Implementation Flow:
```csharp
// Simplified JWT Generation
var claims = new[] {
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Role, user.Role) // "Admin", "Doctor", or "Patient"
};

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: _config["Jwt:Issuer"],
    audience: _config["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddDays(1),
    signingCredentials: creds
);
```

## 4. Role-Based Access Control (RBAC)
In ASP.NET Core, we use the `[Authorize]` attribute to lock down endpoints.

- `[Authorize(Roles = "Admin")]`: Only users with the Admin role can enter.
- `[Authorize(Roles = "Doctor, Admin")]`: Either role is permitted.
- `[AllowAnonymous]`: Public access (e.g., Register/Login).
