# CORE CONCEPT BREAKDOWN

## 1. The Configuration Hierarchy (Layering)
ASP.NET Core uses a layered approach to configuration. If a key exists in multiple sources, the **last one registered wins**.

**The Default Order of Precedence:**
1. `appsettings.json` (The base)
2. `appsettings.{Environment}.json` (Overrides base based on `ASPNETCORE_ENVIRONMENT`)
3. User Secrets (Local development only)
4. Environment Variables (The "Cloud" way)
5. Command-line Arguments (The "Immediate" way)

**Example:**
If `appsettings.json` has `"LogLevel": "Warning"` but `appsettings.Development.json` has `"LogLevel": "Debug"`, the app will use `"Debug"` when running in the Development environment.

## 2. Strongly-Typed Configuration with `IOptions<T>`
Reading strings like `Configuration["Jwt:Secret"]` throughout your code is error-prone and makes testing difficult. Instead, we bind configuration sections to POCO (Plain Old CLR Object) classes.

**Step 1: Create the Settings Class**
```csharp
public class JwtSettings 
{
    public string Secret { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}
```

**Step 2: Bind in `Program.cs`**
```csharp
// Bind the "Jwt" section of appsettings.json to the JwtSettings class
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
```

**Step 3: Inject via `IOptions<T>`**
```csharp
public class AuthService
{
    private readonly JwtSettings _settings;

    public AuthService(IOptions<JwtSettings> options)
    {
        // .Value retrieves the bound object
        _settings = options.Value;
    }

    public void CreateToken() 
    {
        var secret = _settings.Secret; 
        // ...
    }
}
```

## 3. Connection Strings
The `ConnectionStrings` section in JSON is a special convention in .NET.

**JSON:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=myServer;Database=ClinicDb;User Id=sa;Password=pw;"
  }
}
```

**C# Access:**
```csharp
// Shortcut method to get values from the ConnectionStrings section
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

## 4. User Secrets (The Developer's Shield)
For local development, you should **never** put real secrets in `appsettings.json` because that file is committed to Git. 
- `dotnet user-secrets init`: Initializes a secret file in your local user profile (outside the project folder).
- `dotnet user-secrets set "Jwt:Secret" "super-secret-key"`: Saves the secret locally.
- At runtime, .NET automatically merges these secrets into the configuration.
