# QUIZ — Day 3: Async/Await and Task Patterns

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): Task vs Coroutines

Explain how C#'s `async Task<T>` differs from Kotlin's `suspend fun`, focusing on:
1. Scheduler/runtime management (where execution happens)
2. Cancellation model (how interruption works)
3. Synchronization context (is it a concept in both languages?)

---

## Q2 (Code-Based): Blocking on Async

```csharp
public Task<Appointment> GetAppointmentAsync(int id)
{
    var result = _db.Appointments.FindAsync(id).Result;  // Line X
    return Task.FromResult(result);
}
```

What problem does Line X create? Why is this bad, and what should you do instead?

---

## Q3 (Code-Based): ValueTask vs Task

```csharp
public ValueTask<Doctor> GetDoctorAsync(int id)
{
    if (_cache.TryGetValue(id, out var doc))
        return new ValueTask<Doctor>(doc);
    return new ValueTask<Doctor>(_db.Doctors.FindAsync(id));
}

// Later in code:
var doctor = await GetDoctorAsync(1);
var sameDoctor = await GetDoctorAsync(1);
```

Is this code safe? If there's an issue, what's the rule about ValueTask reuse?

---

## Q4 (Scenario-Based): ConfigureAwait(false)

You're writing a shared library method that fetches user data from an HTTP API. Should you use `ConfigureAwait(false)` on the `await client.GetAsync(url)` call? Explain why or why not, considering different consumer contexts (ASP.NET Core, WinForms, console app).

---

## Q5 (Code-Based): Task Coordination

```csharp
public async Task<(Appointment, Doctor, Patient)> GetFullDataAsync(int appointmentId)
{
    var apt = await _db.Appointments.FindAsync(appointmentId);
    var doc = await _db.Doctors.FindAsync(apt.DoctorId);
    var pat = await _db.Patients.FindAsync(apt.PatientId);
    
    return (apt, doc, pat);
}
```

This code fetches data sequentially (one query after another). Rewrite it to fetch all three in parallel and explain why parallel is better.

---

**When you've answered all five, paste them into the chat and I'll grade them.**
