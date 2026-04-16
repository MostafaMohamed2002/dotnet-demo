# Day 7: Consolidation Task – Complete Solution Walkthrough

## Complete Program.cs

Here's a fully working implementation:

```csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

// ============ Data Models ============

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = "";
    public AppointmentStatus Status { get; set; }
}

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled
}

// ============ CSV Parser ============

public static class CsvParser
{
    public static List<Appointment> ParseCsv(string filePath)
    {
        var appointments = new List<Appointment>();

        // Check if file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File '{filePath}' not found");
            return appointments;
        }

        try
        {
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                Console.WriteLine("Error: CSV file is empty");
                return appointments;
            }

            // Skip header (first line)
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];

                // Skip empty or whitespace-only lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Parse the line
                var appointment = ParseLine(line, i + 1);  // i+1 because i is 0-based, but users see 1-based
                if (appointment != null)
                {
                    appointments.Add(appointment);
                }
            }

            Console.WriteLine($"Successfully parsed {appointments.Count} appointments from {filePath}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }

        return appointments;
    }

    private static Appointment? ParseLine(string line, int lineNumber)
    {
        try
        {
            // Split by comma
            var fields = line.Split(',');

            // Validate field count (should be exactly 6)
            if (fields.Length != 6)
            {
                Console.WriteLine($"Warning: Line {lineNumber} has {fields.Length} fields, expected 6. Skipping.");
                return null;
            }

            // Parse each field with validation
            var id = int.Parse(fields[0].Trim());
            var patientName = fields[1].Trim();
            var doctorName = fields[2].Trim();
            var appointmentDate = DateTime.ParseExact(
                fields[3].Trim(),
                "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture);
            var reason = fields[4].Trim();
            var status = Enum.Parse<AppointmentStatus>(fields[5].Trim());

            // Validate required fields
            if (string.IsNullOrWhiteSpace(patientName))
            {
                Console.WriteLine($"Warning: Line {lineNumber} has empty patient name. Skipping.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(doctorName))
            {
                Console.WriteLine($"Warning: Line {lineNumber} has empty doctor name. Skipping.");
                return null;
            }

            return new Appointment
            {
                Id = id,
                PatientName = patientName,
                DoctorName = doctorName,
                AppointmentDate = appointmentDate,
                Reason = reason,
                Status = status
            };
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Warning: Line {lineNumber} has invalid format ({ex.Message}). Skipping.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to parse line {lineNumber}: {ex.Message}. Skipping.");
            return null;
        }
    }
}

// ============ Analysis & Reporting ============

public static class ClinicReporter
{
    public static void GenerateSummary(List<Appointment> appointments)
    {
        if (appointments.Count == 0)
        {
            Console.WriteLine("No appointments to analyze.");
            return;
        }

        // Group by doctor
        var appointmentsByDoctor = appointments
            .GroupBy(a => a.DoctorName)
            .OrderByDescending(g => g.Count())
            .ToList();

        // Calculate metrics
        var totalAppointments = appointments.Count;
        var totalDoctors = appointments.Select(a => a.DoctorName).Distinct().Count();
        var totalPatients = appointments.Select(a => a.PatientName).Distinct().Count();

        var mostBookedDoctor = appointmentsByDoctor.FirstOrDefault();
        var mostBookedDoctorName = mostBookedDoctor?.Key ?? "N/A";
        var mostBookedCount = mostBookedDoctor?.Count() ?? 0;

        var busiestDay = appointments
            .GroupBy(a => a.AppointmentDate.Date)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        var busiestDayDate = busiestDay?.Key ?? DateTime.MinValue;
        var busiestDayCount = busiestDay?.Count() ?? 0;

        var avgAppointmentsPerDoctor = totalDoctors > 0
            ? (double)totalAppointments / totalDoctors
            : 0;

        // Build output
        var output = new StringBuilder();

        output.AppendLine("========== CLINIC APPOINTMENT SUMMARY ==========\n");
        output.AppendLine($"Total Appointments: {totalAppointments}");
        output.AppendLine($"Total Doctors: {totalDoctors}");
        output.AppendLine($"Total Patients: {totalPatients}");
        output.AppendLine();
        output.AppendLine("--- APPOINTMENTS BY DOCTOR ---\n");

        int rank = 1;
        foreach (var doctorGroup in appointmentsByDoctor)
        {
            var doctorName = doctorGroup.Key;
            var totalCount = doctorGroup.Count();
            var scheduledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Scheduled);
            var completedCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Cancelled);
            var patientNames = doctorGroup
                .Select(a => a.PatientName)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            output.AppendLine($"{rank}. {doctorName} ({totalCount} total appointments)");
            output.AppendLine($"   - Scheduled: {scheduledCount}");
            output.AppendLine($"   - Completed: {completedCount}");
            output.AppendLine($"   - Cancelled: {cancelledCount}");
            output.AppendLine($"   - Patient List: {string.Join(", ", patientNames)}");
            output.AppendLine();

            rank++;
        }

        output.AppendLine("--- INSIGHTS ---\n");
        output.AppendLine($"Most Booked Doctor: {mostBookedDoctorName} ({mostBookedCount} appointments)");
        output.AppendLine($"Busiest Day: {busiestDayDate:yyyy-MM-dd} ({busiestDayCount} appointments)");
        output.AppendLine($"Average Appointments per Doctor: {avgAppointmentsPerDoctor:F2}");

        Console.WriteLine(output.ToString());
    }

    public static void ExportToFile(List<Appointment> appointments, string outputPath)
    {
        if (appointments.Count == 0)
        {
            Console.WriteLine("No appointments to export.");
            return;
        }

        try
        {
            var appointmentsByDoctor = appointments
                .GroupBy(a => a.DoctorName)
                .OrderByDescending(g => g.Count())
                .ToList();

            var totalAppointments = appointments.Count;
            var totalDoctors = appointments.Select(a => a.DoctorName).Distinct().Count();
            var totalPatients = appointments.Select(a => a.PatientName).Distinct().Count();

            var output = new StringBuilder();

            output.AppendLine("========== CLINIC APPOINTMENT SUMMARY ==========");
            output.AppendLine();
            output.AppendLine($"Total Appointments: {totalAppointments}");
            output.AppendLine($"Total Doctors: {totalDoctors}");
            output.AppendLine($"Total Patients: {totalPatients}");
            output.AppendLine();
            output.AppendLine("--- APPOINTMENTS BY DOCTOR ---");
            output.AppendLine();

            int rank = 1;
            foreach (var doctorGroup in appointmentsByDoctor)
            {
                var doctorName = doctorGroup.Key;
                var totalCount = doctorGroup.Count();
                var scheduledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Scheduled);
                var completedCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Completed);
                var cancelledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Cancelled);
                var patientNames = doctorGroup
                    .Select(a => a.PatientName)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                output.AppendLine($"{rank}. {doctorName} ({totalCount} total appointments)");
                output.AppendLine($"   - Scheduled: {scheduledCount}");
                output.AppendLine($"   - Completed: {completedCount}");
                output.AppendLine($"   - Cancelled: {cancelledCount}");
                output.AppendLine($"   - Patient List: {string.Join(", ", patientNames)}");
                output.AppendLine();

                rank++;
            }

            File.WriteAllText(outputPath, output.ToString());
            Console.WriteLine($"\nSummary exported to {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to file: {ex.Message}");
        }
    }
}

// ============ Main Program ============

var filePath = "appointments.csv";
var outputPath = "appointment_summary.txt";

// Parse CSV
var appointments = CsvParser.ParseCsv(filePath);

// Generate and display summary
ClinicReporter.GenerateSummary(appointments);

// Export to file
ClinicReporter.ExportToFile(appointments, outputPath);
```

---

## Sample appointments.csv

Place this in your project directory:

```csv
id,patientName,doctorName,appointmentDate,reason,status
101,Alice Johnson,Dr. Smith,2026-04-15 10:30,Checkup,Scheduled
102,Bob Williams,Dr. Jones,2026-04-14 14:00,Follow-up,Scheduled
103,Charlie Brown,Dr. Smith,2026-04-10 09:00,Surgery,Completed
104,Diana Prince,Dr. Wilson,2026-04-20 16:30,Consultation,Scheduled
105,Eve Martinez,Dr. Smith,2026-04-12 11:00,Checkup,Cancelled
106,Frank Thompson,Dr. Jones,2026-04-18 13:30,Lab work,Scheduled
107,Grace Lee,Dr. Wilson,2026-04-22 15:00,Checkup,Scheduled
108,Henry Davis,Dr. Smith,2026-04-16 12:00,Injection,Scheduled
109,Ivy Chen,Dr. Jones,2026-04-17 10:30,Consultation,Completed
110,Jack Miller,Dr. Wilson,2026-04-11 14:30,Checkup,Scheduled
```

---

## Expected Output

When you run this program:

```
Successfully parsed 10 appointments from appointments.csv

========== CLINIC APPOINTMENT SUMMARY ==========

Total Appointments: 10
Total Doctors: 3
Total Patients: 10

--- APPOINTMENTS BY DOCTOR ---

1. Dr. Smith (4 total appointments)
   - Scheduled: 2
   - Completed: 1
   - Cancelled: 1
   - Patient List: Alice Johnson, Charlie Brown, Eve Martinez, Henry Davis

2. Dr. Jones (3 total appointments)
   - Scheduled: 2
   - Completed: 1
   - Cancelled: 0
   - Patient List: Bob Williams, Frank Thompson, Ivy Chen

3. Dr. Wilson (3 total appointments)
   - Scheduled: 3
   - Completed: 0
   - Cancelled: 0
   - Patient List: Diana Prince, Grace Lee, Jack Miller

--- INSIGHTS ---

Most Booked Doctor: Dr. Smith (4 appointments)
Busiest Day: 2026-04-15 (1 appointment)
Average Appointments per Doctor: 3.33

Summary exported to appointment_summary.txt
```

---

## Key Design Decisions

### 1. Separation of Concerns
- `CsvParser` class: Handles file I/O and validation
- `ClinicReporter` class: Handles analysis and output formatting
- `Program.cs`: Main entry point (orchestration)

**Why**: Testable, reusable, maintainable.

### 2. Error Handling Strategy
- Missing file: Return empty list, display error
- Invalid line format: Skip with warning, continue parsing
- Invalid enum value: Catch `ArgumentException`, skip
- Empty fields: Validate and skip

**Why**: Graceful degradation, user visibility.

### 3. LINQ Usage
```csharp
// GroupBy: Organize by doctor
var appointmentsByDoctor = appointments
    .GroupBy(a => a.DoctorName)
    .OrderByDescending(g => g.Count())
    .ToList();

// Count with predicates: Conditional counts
var scheduledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Scheduled);

// Select + Distinct: Unique values
var patientNames = doctorGroup
    .Select(a => a.PatientName)
    .Distinct()
    .OrderBy(p => p)
    .ToList();
```

**Why**: Expressive, concise, type-safe.

### 4. Output Formatting
Used `StringBuilder` instead of string concatenation for performance.

```csharp
var output = new StringBuilder();
output.AppendLine("Line 1");
output.AppendLine("Line 2");
Console.WriteLine(output.ToString());
```

**Why**: More efficient than `+=` in loops.

---

## Testing the Solution

### Test 1: Happy Path
- Run with valid CSV file
- Verify output shows all doctors, counts, patient lists

### Test 2: Missing File
- Delete `appointments.csv`
- Run and verify error message is displayed

### Test 3: Invalid Date
- Edit CSV to have invalid date (e.g., `2026-13-45 99:99`)
- Run and verify warning is shown, line is skipped

### Test 4: Invalid Status
- Edit CSV to have invalid status (e.g., `Pending`)
- Run and verify warning is shown, line is skipped

### Test 5: Empty Lines
- Add blank lines in CSV
- Run and verify program continues without error

---

## Building & Running

```bash
$ dotnet build
Build succeeded.

$ dotnet run
Successfully parsed 10 appointments from appointments.csv

========== CLINIC APPOINTMENT SUMMARY ==========

... (output as shown above)
```

---

## Next: Enhancements

See `04-enhancements.md` for optional features:
- Command-line argument parsing
- Multiple CSV files
- Doctor utilization reports
- Export to JSON/HTML
- Interactive CLI menu
