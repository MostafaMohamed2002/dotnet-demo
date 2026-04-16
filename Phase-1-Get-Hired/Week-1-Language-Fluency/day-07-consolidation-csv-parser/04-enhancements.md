# Day 7: Consolidation Task – Enhancements & Extensions

## After You Complete the Core Task

Here are optional enhancements. Pick 2–3 to level up your skills.

---

## Enhancement 1: Command-Line Arguments

Allow users to specify CSV file path:

```csharp
// At the top of Main
string filePath = args.Length > 0 ? args[0] : "appointments.csv";
string outputPath = args.Length > 1 ? args[1] : "appointment_summary.txt";

// Usage:
// $ dotnet run
// Uses default: appointments.csv

// $ dotnet run path/to/my-data.csv
// Uses custom file: path/to/my-data.csv

// $ dotnet run path/to/my-data.csv output.txt
// Uses custom file and output path
```

---

## Enhancement 2: Doctor Utilization Report

Calculate doctor efficiency:

```csharp
public static void GenerateUtilizationReport(List<Appointment> appointments)
{
    var appointmentsByDoctor = appointments
        .GroupBy(a => a.DoctorName)
        .OrderByDescending(g => g.Count())
        .ToList();

    Console.WriteLine("\n--- DOCTOR UTILIZATION REPORT ---\n");
    Console.WriteLine($"{"Doctor",-15} {"Total",-8} {"Completed",-12} {"Scheduled",-12} {"Utilization"}");
    Console.WriteLine(new string('-', 65));

    foreach (var doctorGroup in appointmentsByDoctor)
    {
        var doctorName = doctorGroup.Key;
        var totalCount = doctorGroup.Count();
        var completedCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Completed);
        var scheduledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Scheduled);
        var utilization = totalCount > 0 ? (completedCount * 100.0 / totalCount) : 0;

        Console.WriteLine($"{doctorName,-15} {totalCount,-8} {completedCount,-12} {scheduledCount,-12} {utilization:F1}%");
    }
}
```

**Usage**: Call in main after `GenerateSummary()`:
```csharp
ClinicReporter.GenerateUtilizationReport(appointments);
```

---

## Enhancement 3: Patient History

Show each patient's appointment history:

```csharp
public static void GeneratePatientHistories(List<Appointment> appointments)
{
    var appointmentsByPatient = appointments
        .GroupBy(a => a.PatientName)
        .OrderBy(g => g.Key)
        .ToList();

    Console.WriteLine("\n--- PATIENT APPOINTMENT HISTORY ---\n");

    foreach (var patientGroup in appointmentsByPatient)
    {
        var patientName = patientGroup.Key;
        var appointments = patientGroup.OrderBy(a => a.AppointmentDate).ToList();

        Console.WriteLine($"{patientName}:");
        foreach (var apt in appointments)
        {
            Console.WriteLine($"  - {apt.AppointmentDate:yyyy-MM-dd HH:mm} with {apt.DoctorName}: {apt.Reason} ({apt.Status})");
        }
        Console.WriteLine();
    }
}
```

---

## Enhancement 4: Export to JSON

Save appointments as JSON instead of text:

```csharp
using System.Text.Json;

public static void ExportToJson(List<Appointment> appointments, string outputPath)
{
    try
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(appointments, options);
        File.WriteAllText(outputPath, json);

        Console.WriteLine($"\nAppointments exported to {outputPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error exporting to JSON: {ex.Message}");
    }
}

// Usage:
// ClinicReporter.ExportToJson(appointments, "appointments.json");
```

---

## Enhancement 5: Search Functionality

Find appointments by criteria:

```csharp
public static class AppointmentSearch
{
    public static List<Appointment> SearchByDoctor(
        List<Appointment> appointments, 
        string doctorName)
    {
        return appointments
            .Where(a => a.DoctorName.Contains(doctorName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Appointment> SearchByPatient(
        List<Appointment> appointments, 
        string patientName)
    {
        return appointments
            .Where(a => a.PatientName.Contains(patientName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Appointment> SearchByDate(
        List<Appointment> appointments, 
        DateTime date)
    {
        return appointments
            .Where(a => a.AppointmentDate.Date == date.Date)
            .ToList();
    }

    public static List<Appointment> SearchByStatus(
        List<Appointment> appointments, 
        AppointmentStatus status)
    {
        return appointments
            .Where(a => a.Status == status)
            .ToList();
    }
}

// Usage:
// var drSmithAppointments = AppointmentSearch.SearchByDoctor(appointments, "Smith");
// var aliceAppointments = AppointmentSearch.SearchByPatient(appointments, "Alice");
// var upcomingAppointments = AppointmentSearch.SearchByStatus(appointments, AppointmentStatus.Scheduled);
```

---

## Enhancement 6: Interactive CLI Menu

Let users choose what to do:

```csharp
public static void InteractiveMenu(List<Appointment> appointments)
{
    while (true)
    {
        Console.WriteLine("\n--- CLINIC MANAGEMENT SYSTEM ---");
        Console.WriteLine("1. View Summary");
        Console.WriteLine("2. View Doctor Utilization");
        Console.WriteLine("3. View Patient Histories");
        Console.WriteLine("4. Search by Doctor");
        Console.WriteLine("5. Search by Patient");
        Console.WriteLine("6. Export to JSON");
        Console.WriteLine("0. Exit");
        Console.Write("\nChoose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ClinicReporter.GenerateSummary(appointments);
                break;
            case "2":
                ClinicReporter.GenerateUtilizationReport(appointments);
                break;
            case "3":
                ClinicReporter.GeneratePatientHistories(appointments);
                break;
            case "4":
                Console.Write("Enter doctor name: ");
                var doctorName = Console.ReadLine() ?? "";
                var doctorResults = AppointmentSearch.SearchByDoctor(appointments, doctorName);
                DisplaySearchResults(doctorResults, $"Doctor: {doctorName}");
                break;
            case "5":
                Console.Write("Enter patient name: ");
                var patientName = Console.ReadLine() ?? "";
                var patientResults = AppointmentSearch.SearchByPatient(appointments, patientName);
                DisplaySearchResults(patientResults, $"Patient: {patientName}");
                break;
            case "6":
                Console.Write("Enter output file path: ");
                var outputPath = Console.ReadLine() ?? "appointments.json";
                ClinicReporter.ExportToJson(appointments, outputPath);
                break;
            case "0":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }
}

private static void DisplaySearchResults(List<Appointment> results, string title)
{
    Console.WriteLine($"\n--- SEARCH RESULTS: {title} ---\n");
    if (results.Count == 0)
    {
        Console.WriteLine("No results found.");
        return;
    }

    foreach (var apt in results)
    {
        Console.WriteLine($"ID: {apt.Id}");
        Console.WriteLine($"  Patient: {apt.PatientName}");
        Console.WriteLine($"  Doctor: {apt.DoctorName}");
        Console.WriteLine($"  Date: {apt.AppointmentDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Reason: {apt.Reason}");
        Console.WriteLine($"  Status: {apt.Status}");
        Console.WriteLine();
    }
}

// Usage in Main:
// InteractiveMenu(appointments);
```

---

## Enhancement 7: Multiple CSV Files

Process and merge multiple CSV files:

```csharp
public static List<Appointment> MergeAppointments(params string[] filePaths)
{
    var allAppointments = new List<Appointment>();

    foreach (var filePath in filePaths)
    {
        var appointments = CsvParser.ParseCsv(filePath);
        allAppointments.AddRange(appointments);
    }

    return allAppointments;
}

// Usage:
// var allAppointments = CsvParser.MergeAppointments(
//     "appointments_clinic1.csv",
//     "appointments_clinic2.csv",
//     "appointments_clinic3.csv"
// );
```

---

## Enhancement 8: Statistics & Charts

Generate statistical insights:

```csharp
public static void GenerateStatistics(List<Appointment> appointments)
{
    if (appointments.Count == 0) return;

    var statusCounts = appointments
        .GroupBy(a => a.Status)
        .ToDictionary(g => g.Key, g => g.Count());

    var reasonCounts = appointments
        .GroupBy(a => a.Reason)
        .OrderByDescending(g => g.Count())
        .ToDictionary(g => g.Key, g => g.Count());

    Console.WriteLine("\n--- STATISTICS ---\n");

    Console.WriteLine("Status Distribution:");
    foreach (var status in statusCounts)
    {
        var percentage = (status.Value * 100.0 / appointments.Count);
        Console.WriteLine($"  {status.Key}: {status.Value} ({percentage:F1}%)");
    }

    Console.WriteLine("\nMost Common Reasons:");
    foreach (var reason in reasonCounts.Take(5))
    {
        var percentage = (reason.Value * 100.0 / appointments.Count);
        Console.WriteLine($"  {reason.Key}: {reason.Value} ({percentage:F1}%)");
    }
}
```

---

## Enhancement 9: Data Validation & Cleanup

Add stricter validation:

```csharp
public static class DataValidator
{
    public static bool IsValidAppointment(Appointment apt)
    {
        // Validate ID is positive
        if (apt.Id <= 0)
            return false;

        // Validate non-empty names
        if (string.IsNullOrWhiteSpace(apt.PatientName) || 
            string.IsNullOrWhiteSpace(apt.DoctorName))
            return false;

        // Validate date is not in past
        if (apt.AppointmentDate < DateTime.Now)
            return false;

        // Validate reason is not empty
        if (string.IsNullOrWhiteSpace(apt.Reason))
            return false;

        return true;
    }
}

// Usage:
// var validAppointments = appointments.Where(DataValidator.IsValidAppointment).ToList();
```

---

## Enhancement 10: Generate HTML Report

Export a formatted HTML report:

```csharp
public static void ExportToHtml(List<Appointment> appointments, string outputPath)
{
    try
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<title>Clinic Appointment Summary</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #4CAF50; color: white; }");
        html.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("<h1>Clinic Appointment Summary</h1>");

        // Summary section
        var totalDoctors = appointments.Select(a => a.DoctorName).Distinct().Count();
        var totalPatients = appointments.Select(a => a.PatientName).Distinct().Count();

        html.AppendLine($"<p><strong>Total Appointments:</strong> {appointments.Count}</p>");
        html.AppendLine($"<p><strong>Total Doctors:</strong> {totalDoctors}</p>");
        html.AppendLine($"<p><strong>Total Patients:</strong> {totalPatients}</p>");

        // Appointments table
        html.AppendLine("<h2>All Appointments</h2>");
        html.AppendLine("<table>");
        html.AppendLine("<tr><th>ID</th><th>Patient</th><th>Doctor</th><th>Date</th><th>Reason</th><th>Status</th></tr>");

        foreach (var apt in appointments.OrderBy(a => a.AppointmentDate))
        {
            html.AppendLine($"<tr><td>{apt.Id}</td><td>{apt.PatientName}</td><td>{apt.DoctorName}</td><td>{apt.AppointmentDate:yyyy-MM-dd HH:mm}</td><td>{apt.Reason}</td><td>{apt.Status}</td></tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        File.WriteAllText(outputPath, html.ToString());
        Console.WriteLine($"\nHTML report exported to {outputPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error exporting to HTML: {ex.Message}");
    }
}

// Usage:
// ClinicReporter.ExportToHtml(appointments, "report.html");
```

---

## Combination: Full-Featured CLI

Combine multiple enhancements:

```csharp
// In Main:
var filePath = args.Length > 0 ? args[0] : "appointments.csv";
var appointments = CsvParser.ParseCsv(filePath);

if (appointments.Count == 0)
{
    Console.WriteLine("No appointments loaded. Exiting.");
    return;
}

// Show summary automatically
ClinicReporter.GenerateSummary(appointments);

// Optionally: Launch interactive menu
Console.WriteLine("\nPress 'i' for interactive menu, or any other key to exit: ");
if (Console.ReadLine()?.ToLower() == "i")
{
    InteractiveMenu(appointments);
}
```

---

## Recommended Progression

1. **Day 1**: Complete core task (CSV parsing, LINQ grouping, summary output)
2. **Day 2**: Add Enhancement 1 (command-line args) + Enhancement 2 (utilization report)
3. **Day 3**: Add Enhancement 3 (patient histories) + Enhancement 5 (search)
4. **Day 4**: Add Enhancement 6 (interactive menu) to tie everything together

---

## Summary: You Now Have

✅ Core CSV parser (manual, no libraries)  
✅ LINQ-based analysis and reporting  
✅ Multiple export formats (text, JSON, HTML)  
✅ Search and filtering capabilities  
✅ Interactive CLI  
✅ Error handling and validation  
✅ Production-quality code organization  

This is a **real, usable application**. The structure you've built is exactly what you'll expand into Week 2 (database backend) and beyond.
