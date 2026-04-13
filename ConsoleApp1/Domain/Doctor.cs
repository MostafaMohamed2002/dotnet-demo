#nullable enable // Enable nullable reference types for static analysis

namespace ConsoleApp1.Domain;

public class Doctor
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Specialty { get; init; } = string.Empty;
    // Changed from DateOnly? to DateTime? to match the task's sample code (new DateTime(2025, 12, 31))
    public DateTime? LicenseExpiryDate { get; init; }

    // Computed property: derives full name from first and last name (no backing field)
    public string FullName => $"{FirstName} {LastName}";
}
