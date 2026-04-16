## Task

Refactor the Clinic Booking API to follow the Repository and Service patterns.

## Files to Create / Modify

1. **Repositories**:
   - `Interfaces/IAppointmentRepository.cs` and `AppointmentRepository.cs`.
   - `Interfaces/IDoctorRepository.cs` and `DoctorRepository.cs`.
2. **Services**:
   - `Interfaces/IAppointmentService.cs` and `AppointmentService.cs`.
3. **Core**:
   - `Common/Result.cs` — Implement the generic Result pattern.
4. **Controllers**:
   - `AppointmentController.cs` — Refactor to be "Thin."

## Expected Output / Behavior

1. The `AppointmentController` should only depend on `IAppointmentService`.
2. The `AppointmentService` should contain the logic: "If doctor exists AND slot is available $\rightarrow$ Book."
3. If a booking fails (e.g., slot taken), the API should return a `400 BadRequest` with the error message from the `Result` object.
4. All `DbContext` calls must be moved out of the Controller and Service, and placed into the Repository.

## Do NOT Do This

- **Do NOT put business logic in the Repository**. Repositories only do CRUD and basic querying.
- **Do NOT put `DbContext` in the Controller**.
- **Do NOT throw exceptions for business rules** (e.g., don't throw `Exception("Slot taken")`). Use the `Result` pattern.

## Self-Review Checklist

- [ ] Is my `AppointmentController` "Thin"? (No logic, just calling the service).
- [ ] Does `AppointmentService` depend on interfaces, not concrete classes?
- [ ] Did I use the `Result<T>` pattern for the booking process?
- [ ] Does the `AppointmentRepository` hide all EF Core logic from the service?
