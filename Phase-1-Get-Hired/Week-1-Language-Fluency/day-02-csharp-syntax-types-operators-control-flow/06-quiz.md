# QUIZ — Day 2: C# Types, Pattern Matching, and Exception Handling

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): Switch Expressions vs. If-Else

Why is a switch expression preferred over nested if-else statements in C#? Mention at least two benefits.

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

---

## Q4 (Scenario-Based): Exception Handling Design

You have a method that reschedules an appointment. List the exceptions you would explicitly catch and why you would NOT catch base `Exception`. What would you do in each case (log? return error? re-throw?)?

---

## Q5 (Conceptual): Type Inference with `var`

When is it appropriate to use `var` instead of an explicit type? Give an example where using `var` is clear and an example where it's not.

---

**When you've answered all five, paste them into the chat and I'll grade them.**
