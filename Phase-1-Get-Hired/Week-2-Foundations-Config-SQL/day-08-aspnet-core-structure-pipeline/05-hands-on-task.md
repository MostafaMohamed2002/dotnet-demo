# HANDS-ON TASK

## Task
Build a skeletal "Clinic Routing" API. You will implement a `DoctorsController` that handles basic requests and a custom "Request Logging" middleware that prints the request path to the console.

## Files to Create / Modify
- `Program.cs` — Configure the pipeline and add the logging middleware.
- `Controllers/DoctorsController.cs` — Implement the requested endpoints.

## Expected Behavior
1. **GET `/api/doctors`**: Return a list of 3 mock doctor names (200 OK).
2. **GET `/api/doctors/{id}`**: 
   - If `id` is 1, 2, or 3 $\rightarrow$ Return "Doctor [id] Details" (200 OK).
   - If `id` is $\le 0 \rightarrow$ Return "Invalid ID" (400 Bad Request).
   - Otherwise $\rightarrow$ Return "Doctor not found" (404 Not Found).
3. **Console Output**: Every request made to the API must print `[LOG]: Request received for /api/...` to the terminal.

## Do NOT Do This
- Do not use Minimal APIs (`app.MapGet`). Use a Controller class.
- Do not hardcode the list of doctors inside the `GetById` method; keep them as a static list in the class.

## Self-Review Checklist
- [ ] Does the `DoctorsController` inherit from `ControllerBase`?
- [ ] Is the `[ApiController]` attribute present?
- [ ] Is the custom middleware registered **before** `app.MapControllers()`?
- [ ] Does the `GetById` method correctly return 400, 404, and 200 based on the input?
- [ ] Does the console show the log for every request?
