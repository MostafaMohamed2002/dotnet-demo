## Task

Configure CORS for the Clinic Booking API to allow requests from a simulated frontend.

## Files to Create / Modify

- `Program.cs` — Implement a named CORS policy and apply it to the middleware pipeline.

## Expected Output / Behavior

1. Configure the API to allow requests from `http://localhost:3000` (the standard React port).
2. Ensure the policy allows all headers and all methods (GET, POST, PUT, DELETE).
3. **Verification**: Since you don't have a frontend, use a tool like `curl` or a specialized CORS tester to simulate the preflight request:
   - Run: `curl -X OPTIONS -H "Origin: http://localhost:3000" -v http://localhost:YOUR_PORT/api/doctors`
   - **Success**: You should see `HTTP/1.1 200 OK` and the header `Access-Control-Allow-Origin: http://localhost:3000`.

## Do NOT Do This

- **Do NOT use `.AllowAnyOrigin()` in the final submission**. Use a specific origin to demonstrate production-readiness.
- **Do NOT put `UseCors` after `UseAuthorization`**.

## Self-Review Checklist

- [ ] Is `builder.Services.AddCors()` called before `builder.Build()`?
- [ ] Is `app.UseCors("PolicyName")` called after `app.UseRouting()` and before `app.UseAuthorization()`?
- [ ] Did I use a named policy instead of a default anonymous policy?
- [ ] Does the `OPTIONS` request return a `200 OK`?
