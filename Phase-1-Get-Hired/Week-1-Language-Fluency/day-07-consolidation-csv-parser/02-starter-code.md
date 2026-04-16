# Day 7: Consolidation Task – Starter Code & Hints

## Part 1: Project Setup

```bash
$ dotnet new console --name AppointmentCsvParser
$ cd AppointmentCsvParser
```

---

## Part 2: Data Classes

Define your models at the top of `Program.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
```

---

## Part 3: CSV File Creation

Create `appointments.csv` in the project directory:

```bash
$ cat > appointments.csv << 'EOF'
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
EOF
```

---

## Part 4: CSV Parser Function

Implement a parser function that reads and validates:

```csharp
public static List<Appointment> ParseCsv(string filePath)
{
    var appointments = new List<Appointment>();

    // TODO: Implement
    // 1. Check if file exists
    // 2. Read all lines
    // 3. Skip header
    // 4. For each line:
    //    a. Split by comma
    //    b. Validate field count
    //    c. Parse each field
    //    d. Handle errors gracefully
    //    e. Add to list
    // 5. Return list

    return appointments;
}
```

**Hints**:

```csharp
// Check file exists
if (!File.Exists(filePath))
{
    Console.WriteLine($"Error: File '{filePath}' not found");
    return new List<Appointment>();
}

// Read all lines
var lines = File.ReadAllLines(filePath);

// Skip header and iterate
for (int i = 1; i < lines.Length; i++)
{
    var line = lines[i];
    if (string.IsNullOrWhiteSpace(line))
        continue;  // Skip empty lines

    var fields = line.Split(',');
    
    // Validate field count (should be 6: id, name, doctor, date, reason, status)
    if (fields.Length != 6)
    {
        Console.WriteLine($"Warning: Line {i + 1} has {fields.Length} fields, expected 6. Skipping.");
        continue;
    }

    // Parse fields
    try
    {
        var appointment = new Appointment
        {
            Id = int.Parse(fields[0].Trim()),
            PatientName = fields[1].Trim(),
            DoctorName = fields[2].Trim(),
            AppointmentDate = DateTime.ParseExact(
                fields[3].Trim(),
                "yyyy-MM-dd HH:mm",
                System.Globalization.CultureInfo.InvariantCulture),
            Reason = fields[4].Trim(),
            Status = Enum.Parse<AppointmentStatus>(fields[5].Trim())
        };

        appointments.Add(appointment);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Failed to parse line {i + 1}: {ex.Message}");
    }
}

return appointments;
```

---

## Part 5: LINQ Analysis Functions

Create helper methods for analysis:

```csharp
// Group by doctor and analyze
var appointmentsByDoctor = appointments
    .GroupBy(a => a.DoctorName)
    .OrderByDescending(g => g.Count())
    .ToList();

// For each group, you can:
foreach (var doctorGroup in appointmentsByDoctor)
{
    var doctorName = doctorGroup.Key;  // Doctor name
    var totalCount = doctorGroup.Count();  // Total appointments
    var scheduledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Scheduled);
    var completedCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Completed);
    var cancelledCount = doctorGroup.Count(a => a.Status == AppointmentStatus.Cancelled);
    var patientNames = doctorGroup.Select(a => a.PatientName).Distinct().ToList();

    // Format output
    Console.WriteLine($"{doctorName} ({totalCount} total appointments)");
    Console.WriteLine($"  - Scheduled: {scheduledCount}");
    Console.WriteLine($"  - Completed: {completedCount}");
    Console.WriteLine($"  - Cancelled: {cancelledCount}");
    Console.WriteLine($"  - Patients: {string.Join(", ", patientNames)}");
}
```

---

## Part 6: Summary Statistics

Calculate key metrics:

```csharp
// Total metrics
var totalAppointments = appointments.Count;
var totalDoctors = appointments.Select(a => a.DoctorName).Distinct().Count();
var totalPatients = appointments.Select(a => a.PatientName).Distinct().Count();

// Most booked doctor
var mostBookedDoctor = appointmentsByDoctor.FirstOrDefault();
var mostBookedDoctorName = mostBookedDoctor?.Key ?? "N/A";
var mostBookedCount = mostBookedDoctor?.Count() ?? 0;

// Busiest day
var busiestDay = appointments
    .GroupBy(a => a.AppointmentDate.Date)
    .OrderByDescending(g => g.Count())
    .FirstOrDefault();
var busiestDayDate = busiestDay?.Key ?? DateTime.MinValue;
var busiestDayCount = busiestDay?.Count() ?? 0;

// Average appointments per doctor
var avgAppointmentsPerDoctor = totalDoctors > 0 
    ? (double)totalAppointments / totalDoctors 
    : 0;
```

---

## Part 7: Output Formatting

Build a formatted summary string:

```csharp
var output = new System.Text.StringBuilder();

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
    var patientNames = doctorGroup.Select(a => a.PatientName).Distinct().ToList();

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
```

---

## Part 8: Main Program Flow

Tie it all together:

```csharp
// Main program
var filePath = "appointments.csv";

// Parse CSV
var appointments = ParseCsv(filePath);

if (appointments.Count == 0)
{
    Console.WriteLine("No appointments found. Exiting.");
    return;
}

// Analyze with LINQ
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

// Build and display output
var output = new System.Text.StringBuilder();
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
    var patientNames = doctorGroup.Select(a => a.PatientName).Distinct().ToList();

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
```

---

## Building & Running

```bash
$ dotnet build
Build succeeded.

$ dotnet run
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

... (rest of output)
```

---

## Debugging Tips

**Issue: "appointments.csv not found"**
- Make sure `appointments.csv` is in the same directory as `Program.cs`
- Or pass full path: `var filePath = @"C:\path\to\appointments.csv"`

**Issue: "Invalid date format"**
- Check CSV has dates in `yyyy-MM-dd HH:mm` format
- Example: `2026-04-15 10:30` ✅, not `04-15-2026 10:30 AM` ❌

**Issue: "Enum parse failed"**
- Check CSV status column is exactly: `Scheduled`, `Completed`, or `Cancelled`
- Case-sensitive: `scheduled` ❌, `Scheduled` ✅

**Issue: Console output looks wrong**
- Use `StringBuilder` instead of concatenating strings
- Align columns with spacing: `"   - "` prefix

---

## Next Steps

1. Copy starter code into `Program.cs`
2. Implement `ParseCsv()` function
3. Create `appointments.csv` file
4. Run and verify output
5. Add error handling
6. Implement enhancements (busiest day, export, etc.)

See `03-walkthrough.md` for a complete working solution.
