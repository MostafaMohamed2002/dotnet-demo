# MENTAL MODEL CHECK

## The Analogy: The Corporate Mailroom
Imagine the ASP.NET Core pipeline as a corporate mailroom in a large hospital.

1. **The Mailroom (Program.cs)**: The building's layout and the list of employees (DI Container) are decided here.
2. **The Receptionist (Middleware 1)**: Checks if the mail is addressed correctly. If it's "spam" (bad request), they throw it away immediately.
3. **The Security Guard (Middleware 2)**: Checks the sender's ID badge. If they don't have a badge, they are kicked out (401 Unauthorized).
4. **The Floor Manager (Routing)**: Looks at the envelope and says, "This is for the Cardiology Department" (DoctorController).
5. **The Specialist (Controller Action)**: The doctor reads the request, does the work, and writes a response.
6. **The Return Path**: The response goes back through the Floor Manager $\rightarrow$ Security $\rightarrow$ Receptionist before leaving the building.

## Where the analogy breaks down
In a real building, the specialist just sends the mail back. In ASP.NET Core, the response **must** travel back through the middleware in reverse order. This allows middleware to modify the response (e.g., adding a custom header or compressing the JSON) *after* the controller has finished its work.
