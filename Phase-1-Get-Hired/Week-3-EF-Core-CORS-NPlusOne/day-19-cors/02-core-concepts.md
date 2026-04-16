# CORE CONCEPT BREAKDOWN

## 1. What is "Origin"?
An origin is defined by three things: **Protocol**, **Domain**, and **Port**.
- `https://api.clinic.com`
- `https://frontend.clinic.com` (Different domain $\rightarrow$ Different Origin)
- `http://localhost:5000` vs `http://localhost:3000` (Different port $\rightarrow$ Different Origin)

If the origin of the frontend does not match the origin of the API, the browser triggers the CORS check.

## 2. The Preflight Request (`OPTIONS`)
For "unsafe" requests (POST, PUT, DELETE, or requests with custom headers), the browser doesn't send the actual request immediately. Instead, it sends a **Preflight Request**.

- **The Request**: The browser sends an `OPTIONS` request to the server asking: "I am from `localhost:3000`. Do you allow me to send a `POST` request with a `Content-Type: application/json` header?"
- **The Response**: The server must respond with specific headers (e.g., `Access-Control-Allow-Origin: http://localhost:3000`).
- **The Outcome**: If the server responds with `200 OK` and the correct headers, the browser finally sends the actual `POST` request. If the server returns `404` or `405`, the browser blocks the request and the C# code in your controller is **never even executed**.

## 3. Configuring CORS in ASP.NET Core
CORS is implemented as middleware. Order is critical.

### Step 1: Define the Policy
In `Program.cs`, we tell the service container how to handle origins.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClinicFrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://clinic-app.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### Step 2: Apply the Middleware
You must place `UseCors` in the pipeline **after** `UseRouting` but **before** `UseAuthorization`.

```csharp
var app = builder.Build();

app.UseRouting();

// CORRECT ORDER
app.UseCors("ClinicFrontendPolicy"); 

app.UseAuthorization();

app.MapControllers();
app.Run();
```

## 4. Common Policy Configurations

| Method | Use Case | Risk |
|---|---|---|
| `.AllowAnyOrigin()` | Local development / Public APIs | **High**. Any site on the internet can call your API. |
| `.WithOrigins(...)` | Production | **Low**. Only trusted domains can access the data. |
| `.AllowCredentials()` | Sending Cookies/Auth headers | **Medium**. Cannot be used with `.AllowAnyOrigin()`. Requires a specific origin. |
