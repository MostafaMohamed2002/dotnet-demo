#nullable enable // Enable nullable reference types for static analysis

using System;
using ConsoleApp1.Domain;

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
    ReasonForVisit = "Chest pain"
};

// Verify computed properties and methods work correctly
Console.WriteLine($"Doctor full name: {doctor.FullName}");               // "Alice Smith"
Console.WriteLine($"Patient full name: {patient.FullName}");             // "Bob Johnson"
Console.WriteLine($"Appointment upcoming: {appointment.IsUpcoming()}");  // true (7 days from now)
Console.WriteLine($"Patient insurance: {patient.InsuranceNumber ?? "none"}");  // "none" (not set)
Console.WriteLine($"Appointment rescheduled: {appointment.RescheduledTime?.ToString() ?? "no"}");  // "no" (not set)
