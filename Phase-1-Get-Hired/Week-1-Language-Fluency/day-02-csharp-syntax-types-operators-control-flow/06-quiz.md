# QUIZ — Day 2: C# Types, Pattern Matching, and Exception Handling

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): Switch Expressions vs. If-Else

Why is a switch expression preferred over nested if-else statements in C#? Mention at least two benefits.

Answer:

- Readability and conciseness: switch expressions keep multi-branch logic compact and easier to scan than deeply nested if/else blocks.
- Stronger pattern-matching and exhaustiveness: switch expressions support pattern matching (including property, type, and relational patterns) and can make intent explicit; the compiler can help catch missing cases when appropriate.
- (Bonus) They are expressions that produce values, enabling clearer, side-effect-free code and less boilerplate when returning results from branching logic.

---

## Q2 (Code-Based): Pattern Matching

```csharp
var apt = new Appointment 
{ 
    Status = AppointmentStatus.Scheduled, 
    ScheduledTime = DateTime.UtcNow.AddDays(7) 
};

string result = apt switch
{
    { Status: AppointmentStatus.Completed } => "Done",
    { Status: AppointmentStatus.Cancelled } => "Cancelled",
    { Status: AppointmentStatus.Scheduled, ScheduledTime: < DateTime.UtcNow } => "Overdue",
    { Status: AppointmentStatus.Scheduled } => "Upcoming",
    _ => "Unknown"
};

Console.WriteLine(result);
```

What is printed, and explain why the `"Overdue"` case wouldn't match even if `ScheduledTime` were in the past.

Answer:

- Printed: "Upcoming"
- Explanation: The appointment has Status = Scheduled and ScheduledTime = DateTime.UtcNow.AddDays(7), i.e., a future time, so the relational pattern `ScheduledTime: < DateTime.UtcNow` is false and the next `{ Status: AppointmentStatus.Scheduled }` pattern matches. Even if `ScheduledTime` were in the past, the relational pattern only matches when the value is strictly less than the value of `DateTime.UtcNow` at match time; issues that can prevent the `Overdue` case from matching include mismatched DateTime.Kind (Local vs Utc), precision/tick differences, or the fact that the first matching pattern wins — so ordering of patterns matters.

---

## Q3 (Code-Based): Tuple Return Values

```csharp
public (bool Success, string Message) ProcessAppointment(int appointmentId)
{
    if (appointmentId <= 0)
        return (false, "Invalid ID");
    return (true, "Success");
}

var (isOk, note) = ProcessAppointment(5);
Console.WriteLine($"{isOk}: {note}");
```

What is printed, and explain how tuple deconstruction works here.

Answer:

- Printed: "True: Success"
- Explanation: `ProcessAppointment(5)` returns the tuple `(true, "Success")`. The line `var (isOk, note) = ProcessAppointment(5);` deconstructs the returned tuple into two local variables: `isOk` receives the boolean `Success` value and `note` receives the `Message` string. The interpolated WriteLine prints those values.

---

## Q4 (Scenario-Based): Exception Handling Design

You have a method that reschedules an appointment. List the exceptions you would explicitly catch and why you would NOT catch base `Exception`. What would you do in each case (log? return error? re-throw?)?

Answer:

- Exceptions to explicitly catch and how to handle them:
  - `ArgumentException` / `ArgumentNullException`: indicates invalid input from the caller — log at Info/Debug, return a validation error (e.g., 400-like response) with a user-friendly message; do not re-throw.
  - Domain-specific exceptions (e.g., `InvalidAppointmentException`): indicates business rule violations — log at Warn, return a domain error result with a clear message for the caller; do not re-throw.
  - Not-found exceptions (e.g., `NotFoundException`): return a not-found result (404-style) and log concisely.
  - Persistence/concurrency exceptions (e.g., EF's `DbUpdateConcurrencyException` / `DbUpdateException`): log full details at Error, consider retry logic if appropriate, or return a transient error instructing the caller to retry; avoid swallowing.

- Why NOT catch base `Exception`:
  - Catching `Exception` swallows unexpected programming errors (NullReferenceException, OutOfMemoryException, etc.), hiding bugs and making diagnosis harder. Let unexpected exceptions bubble to a global handler/middleware that logs details and returns a generic error response.

- General handling guidance:
  - For expected/handled exceptions: log with appropriate level, return a clear error/result to the caller, do not re-throw.
  - For persistence/concurrency: log details, consider retry or surface a transient error.
  - For truly unexpected exceptions: allow them to propagate to a global handler which logs full diagnostics; preserve stack trace when re-throwing (use `throw;`).

---

## Q5 (Conceptual): Type Inference with `var`

When is it appropriate to use `var` instead of an explicit type? Give an example where using `var` is clear and an example where it's not.

Answer:

- Use `var` when the right-hand side makes the type obvious or the explicit type would be verbose.
- Clear example:

  var customers = new List<Customer>();

  (The type is explicit in the initializer and readability is preserved.)

- Not-clear example:

  var result = GetData();

  (If `GetData()`'s return type isn't obvious from context, `var` hides what `result` actually is — prefer an explicit type or a better method name.)

---

**When you've answered all five, paste them into the chat and I'll grade them.**
