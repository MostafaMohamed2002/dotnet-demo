# REAL-WORLD CONTEXT

## Production Context
In a professional CI/CD pipeline, you typically have different CORS policies for different environments.

- **Development**: `AllowAnyOrigin()` is common to allow developers to run frontends on various ports.
- **Production**: Strictly defined origins (e.g., `https://app.clinic.com`) are mandated by security audits.

### The Clinic Case Study: The "Auth Header" Failure
A developer implements JWT authentication. The frontend starts sending the token in a custom header: `Authorization: Bearer <token>`.

**The Symptom**: The API works in Postman but returns a CORS error in the browser.
**The Reason**: Adding a custom header (`Authorization`) makes the request "non-simple." The browser triggers a preflight `OPTIONS` request. If the server isn't configured with `.AllowAnyHeader()`, the `OPTIONS` request fails.

## Common Junior Mistake: Putting `UseCors` at the Bottom
A developer puts `app.UseCors()` after `app.UseAuthorization()`.

**Why this fails code review:**
The `Authorization` middleware checks for a valid JWT. If the user isn't logged in, it returns `401 Unauthorized`. Because `UseCors` hasn't run yet, the `401` response doesn't contain the `Access-Control-Allow-Origin` headers. The browser sees a `401` without CORS headers and reports a **CORS Error**, masking the actual `401` error. The developer wastes hours debugging CORS when the real problem is a missing token.
