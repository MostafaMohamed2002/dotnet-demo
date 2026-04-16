# CORE CONCEPT BREAKDOWN

## 1. The Infrastructure: DbContext and DbSet
The `DbContext` is the heart of EF Core. It represents a session with the database and is used to query and save data.

### Configuration in `Program.cs`
```csharp
// Program.cs
builder.Services.AddDbContext<ClinicDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### The Context Implementation
```csharp
public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    // Each DbSet represents a table in the database
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}
```

## 2. Code-First Workflow
Instead of creating tables in SSMS, we define C# classes and "migrate" them to SQL.

**The Workflow:**
1. **Define Entity**: Create a class (e.g., `Doctor.cs`).
2. **Add to Context**: Add `DbSet<Doctor>` to `ClinicDbContext`.
3. **Create Migration**: `dotnet ef migrations add InitialCreate` (Generates a C# script describing the change).
4. **Apply Migration**: `dotnet ef database update` (Executes the script against the DB).

## 3. Modeling Relationships
EF Core uses "Navigation Properties" to represent relationships.

### One-to-Many (Doctor $\rightarrow$ Appointments)
A Doctor has many appointments; an appointment has one doctor.
```csharp
public class Doctor {
    public int DoctorId { get; set; }
    public string Name { get; set; }
    // Navigation property: One Doctor to Many Appointments
    public ICollection<Appointment> Appointments { get; set; } 
}

public class Appointment {
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; } // Foreign Key
    public Doctor Doctor { get; set; } // Navigation property back to Doctor
}
```

### Many-to-Many with Payload (Patient $\leftrightarrow$ Appointment $\leftrightarrow$ Doctor)
In a clinic, a Patient and a Doctor are linked via an `Appointment`. An appointment isn't just a join table; it has its own data (Time, Status). This is modeled as two one-to-many relationships.

```text
[Patient] 1 <---> N [Appointment] N <---> 1 [Doctor]
```
The `Appointment` entity acts as the "join entity with payload."

## 4. Data Operations
Always use **Async** variants to avoid blocking the thread pool in a high-concurrency API.

```csharp
// Create
var doc = new Doctor { Name = "Dr. Ahmed" };
_context.Doctors.Add(doc);
await _context.SaveChangesAsync();

// Read with Eager Loading
// Without .Include(), the 'Appointments' property will be null (Lazy loading is off by default)
var doctorWithApps = await _context.Doctors
    .Include(d => d.Appointments) 
    .FirstOrDefaultAsync(d => d.DoctorId == 1);

// Update
var patient = await _context.Patients.FindAsync(id);
patient.Name = "Updated Name"; 
await _context.SaveChangesAsync(); // EF tracks changes automatically

// Delete
_context.Patients.Remove(patient);
await _context.SaveChangesAsync();
```

## 5. Performance Win: AsNoTracking()
By default, EF Core tracks every entity it fetches in the **Change Tracker**. This is expensive for read-only queries.

**The Wrong Way (Read-Only):**
```csharp
var doctors = await _context.Doctors.ToListAsync(); 
// EF is now watching every doctor object for changes. Waste of CPU/RAM.
```

**The Right Way (Read-Only):**
```csharp
var doctors = await _context.Doctors.AsNoTracking().ToListAsync();
// EF just fetches the data and forgets about it. Much faster.
```
