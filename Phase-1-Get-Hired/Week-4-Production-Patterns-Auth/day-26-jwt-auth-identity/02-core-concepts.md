# CORE CONCEPT BREAKDOWN

## 1. ASP.NET Identity (The Core)
Identity is a framework for managing users, passwords, and roles. We use `AddIdentityCore<TUser>()` to get the services without the heavy, outdated Razor Pages UI.

### Key Services
- **`UserManager<TUser>`**: The primary API for managing users (creating, deleting, updating passwords, checking roles).
- **`RoleManager<TUser>`**: Manages roles (Admin, Doctor, Patient) and their permissions.

### Password Hashing
**Never** store passwords in plaintext. `UserManager.CreateAsync()` uses a salted hash (PBKDF2) by default. This means even if your database is stolen, the passwords cannot be easily reversed.

## 2. JWT (JSON Web Tokens)
A JWT is a string composed of three parts: `Header.Payload.Signature`.

- **Header**: Algorithm and token type.
- **Payload (Claims)**: Information about the user (e.g., `UserId`, `Email`, `Role`).
- **Signature**: A cryptographic hash of the header and payload using a **Secret Key** known only to the server.

### JWT Generation Flow
1. Client sends credentials (Email/Password).
2. Server verifies credentials via `UserManager.CheckPasswordAsync()`.
3. Server creates a `SecurityTokenDescriptor` containing **Claims**.
4. Server signs the token using a `SymmetricSecurityKey` and returns the string to the client.

## 3. The `[Authorize]` Attribute
Once JWT middleware is configured in `Program.cs`, you can protect your controllers using attributes.

- **`[Authorize]`**: Only authenticated users can enter.
- **`[Authorize(Roles = "Admin")]`**: Only users with the "Admin" role can enter.
- **`[AllowAnonymous]`**: Opens a specific endpoint (like `/login` or `/register`) to everyone.

## 4. Configuration in `Program.cs`
You must tell ASP.NET Core how to validate the tokens that come back.

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

## 5. Refresh Tokens (Concept)
JWTs are usually short-lived (e.g., 15 minutes) for security. If a token is stolen, the attacker only has 15 minutes. 
To avoid forcing the user to log in every 15 minutes, we use a **Refresh Token**: a long-lived, random string stored in the DB. When the JWT expires, the client sends the Refresh Token to a special endpoint to get a new JWT. (Implementation skipped for now, but know the flow).
