#nullable enable // Enable nullable reference types for static analysis

namespace ConsoleApp1.Domain;

public class Patient
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    // Optional field: nullable + set (can be assigned or changed after creation)
    public string? InsuranceNumber { get; set; }

    // Computed property: derives full name from first and last name (no backing field)
    public string FullName => $"{FirstName} {LastName}";
}
