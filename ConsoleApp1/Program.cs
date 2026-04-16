#nullable enable // Enable nullable reference types for static analysis

using System;
using ConsoleApp1.Domain;
using ConsoleApp1.Services;

// Instantiate all three domain entities per the task's expected sample code

var doctor = new Doctor
{
    Id = 1,
    FirstName = "Alice",
    LastName = "Smith",
    Specialty = "Cardiology",
    LicenseExpiryDate = new DateTime(2025, 12, 31)
};

var patient = new Patient
{
    Id = 101,
    FirstName = "Bob",
    LastName = "Johnson",
    Email = "bob@clinic.com",
    PhoneNumber = "555-1234"
};

var appointment = new Appointment
{
    Id = 1001,
    DoctorId = 1,
    PatientId = 101,
    ScheduledTime = DateTime.UtcNow.AddDays(7),
    ReasonForVisit = "Chest pain",
    Status = AppointmentStatus.Scheduled
};

// Verify computed properties and methods work correctly
Console.WriteLine($"Doctor full name: {doctor.FullName}");               // "Alice Smith"
Console.WriteLine($"Patient full name: {patient.FullName}");             // "Bob Johnson"
Console.WriteLine($"Appointment upcoming: {appointment.IsUpcoming()}");  // true (7 days from now)
Console.WriteLine($"Patient insurance: {patient.InsuranceNumber ?? "none"}");  // "none" (not set)
Console.WriteLine($"Appointment rescheduled: {appointment.RescheduledTime?.ToString() ?? "no"}");  // "no" (not set)

// Day 2: Test AppointmentService with pattern matching and enums
Console.WriteLine("\n--- Day 2: Appointment Service Tests ---\n");

var service = new AppointmentService();

// Test 1: Reschedule succeeds (appointment is scheduled)
Console.WriteLine("Test 1: Reschedule scheduled appointment");
var (success, msg) = service.ValidateReschedule(appointment, DateTime.UtcNow.AddDays(14));
Console.WriteLine($"  Success: {success}");       // true
Console.WriteLine($"  Message: {msg}");           // "Valid: can reschedule"

// Test 2: Reschedule fails (appointment completed)
Console.WriteLine("\nTest 2: Try to reschedule completed appointment");
appointment.Status = AppointmentStatus.Completed;
(success, msg) = service.ValidateReschedule(appointment, DateTime.UtcNow.AddDays(14));
Console.WriteLine($"  Success: {success}");       // false
Console.WriteLine($"  Message: {msg}");           // "Cannot reschedule completed appointment"

// Test 3: Reschedule fails (new time is in past)
Console.WriteLine("\nTest 3: Try to reschedule to past time");
appointment.Status = AppointmentStatus.Scheduled;
(success, msg) = service.ValidateReschedule(appointment, DateTime.UtcNow.AddDays(-1));
Console.WriteLine($"  Success: {success}");       // false
Console.WriteLine($"  Message: {msg}");           // "Cannot reschedule to a past time"

// Test 4: Get status display string
Console.WriteLine("\nTest 4: Get status display strings");
var statuses = new[] 
{ 
    AppointmentStatus.Scheduled, 
    AppointmentStatus.Completed, 
    AppointmentStatus.Cancelled, 
    AppointmentStatus.NoShow 
};

foreach (var status in statuses)
{
    var display = service.GetStatusDisplay(status);
    Console.WriteLine($"  {status}: {display}");
}

// Test 5: Check if appointment is final
Console.WriteLine("\nTest 5: Check if appointment is in final state");
appointment.Status = AppointmentStatus.Scheduled;
Console.WriteLine($"  Scheduled appointment is final: {service.IsFinal(appointment)}");  // false

appointment.Status = AppointmentStatus.Completed;
Console.WriteLine($"  Completed appointment is final: {service.IsFinal(appointment)}");  // true

// Test 6: Try to cancel appointment
Console.WriteLine("\nTest 6: Cancel appointment");
appointment.Status = AppointmentStatus.Scheduled;
(success, msg) = service.TryCancel(appointment, "Patient requested");
Console.WriteLine($"  Success: {success}");       // true
Console.WriteLine($"  Message: {msg}");           // "Appointment cancelled: Patient requested"
Console.WriteLine($"  Status after cancel: {service.GetStatusDisplay(appointment.Status)}");  // ❌ Cancelled

// Test 7: Try to cancel already cancelled appointment
Console.WriteLine("\nTest 7: Try to cancel already cancelled appointment");
(success, msg) = service.TryCancel(appointment);
Console.WriteLine($"  Success: {success}");       // false
Console.WriteLine($"  Message: {msg}");           // Cannot cancel appointment with status: ❌ Cancelled

Console.WriteLine("\n✅ All tests completed successfully!");
