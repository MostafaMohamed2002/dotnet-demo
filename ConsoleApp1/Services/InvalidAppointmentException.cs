#nullable enable

namespace ConsoleApp1.Services;

/// <summary>
/// Thrown when an appointment operation is invalid based on business rules.
/// </summary>
public class InvalidAppointmentException : Exception
{
    public InvalidAppointmentException(string message) : base(message)
    {
    }

    public InvalidAppointmentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
