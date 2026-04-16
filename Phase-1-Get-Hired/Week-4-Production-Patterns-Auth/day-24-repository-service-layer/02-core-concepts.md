# CORE CONCEPT BREAKDOWN

## 1. The Architectural Flow
The data flows in a strict one-way direction:
**Request** $\rightarrow$ **Controller** $\rightarrow$ **Service** $\rightarrow$ **Repository** $\rightarrow$ **Database** $\rightarrow$ **Repository** $\rightarrow$ **Service** $\rightarrow$ **Controller** $\rightarrow$ **Response**.

### Controller (The Thin Layer)
The controller should only handle the "Web" part.
```csharp
[HttpPost]
public async Task<IActionResult> Book(BookAppointmentRequest request) {
    // 1. Simple validation (e.g., check if DTO is null)
    // 2. Call the service
    var result = await _appointmentService.BookAppointmentAsync(request);
    
    // 3. Return the response based on the result
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

## 2. The Repository Pattern
The repository acts as a "collection" of entities. It hides the `DbContext`.

### Specific vs Generic Repositories
Many tutorials teach a `GenericRepository<T>`, but in production, **Specific Repositories win**. Why? Because complex queries (like "Get all available slots for Doctor X on Date Y") cannot be defined in a generic interface.

**Correct Implementation:**
```csharp
public interface IAppointmentRepository {
    Task<Appointment> GetByIdAsync(int id);
    Task<bool> IsSlotAvailableAsync(int slotId);
    Task AddAsync(Appointment appointment);
}

public class AppointmentRepository : IAppointmentRepository {
    private readonly ClinicDbContext _context;
    public AppointmentRepository(ClinicDbContext context) => _context = context;

    public async Task<bool> IsSlotAvailableAsync(int slotId) {
        return await _context.Appointments
            .AnyAsync(a => a.SlotId == slotId && a.Status == "Scheduled");
    }
}
```

## 3. The Service Layer
The service contains the **Business Logic**. It coordinates multiple repositories.

```csharp
public class AppointmentService : IAppointmentService {
    private readonly IAppointmentRepository _apptRepo;
    private readonly IDoctorRepository _docRepo;

    public AppointmentService(IAppointmentRepository apptRepo, IDoctorRepository docRepo) {
        _apptRepo = apptRepo;
        _docRepo = docRepo;
    }

    public async Task<Result<AppointmentDto>> BookAppointmentAsync(BookAppointmentRequest request) {
        // Rule 1: Does the doctor exist?
        if (!await _docRepo.ExistsAsync(request.DoctorId)) 
            return Result<AppointmentDto>.Failure("Doctor does not exist.");

        // Rule 2: Is the slot available?
        if (await _apptRepo.IsSlotAvailableAsync(request.SlotId)) 
            return Result<AppointmentDto>.Failure("Slot is already booked.");

        // Rule 3: Perform the action
        var appt = new Appointment { ... };
        await _apptRepo.AddAsync(appt);
        
        return Result<AppointmentDto>.Success(new AppointmentDto { ... });
    }
}
```

## 4. The Result Pattern
Instead of throwing exceptions (which are expensive and confuse the control flow), use a `Result` object to communicate success or failure.

```csharp
public class Result<T> {
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    protected Result(T value) { IsSuccess = true; Value = value; }
    protected Result(string error) { IsSuccess = false; Error = error; }

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(string error) => new Result<T>(error);
}
```
*Note: This signals "maturity" in a code review because it shows you understand that business rule violations are not "exceptional" events, but expected outcomes.*
