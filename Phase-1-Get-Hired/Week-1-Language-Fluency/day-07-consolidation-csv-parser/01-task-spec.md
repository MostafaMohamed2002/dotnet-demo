# Day 7: Consolidation Task – CSV Parser & LINQ Analysis

## Overview

Build a **CLI tool** that reads appointment data from a CSV file, parses it into strongly-typed objects, analyzes with LINQ, and outputs a formatted summary.

**Duration**: 2–3 hours (intentionally meaty)

**This bridges everything from Days 1–6** in one integrated project:
- C# syntax and data classes (Day 1–2)
- String parsing and error handling (Day 2)
- LINQ grouping and aggregation (Day 4)
- File I/O and async patterns (Day 3, 5)
- Console output formatting (Day 6)

---

## The Task

### Part 1: CSV Input
Create an `appointments.csv` file with appointment records:

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

### Part 2: Console App
Build a C# console app that:

1. **Reads** the CSV file (handle missing files gracefully)
2. **Parses** each line into an `Appointment` object (with validation)
3. **Analyzes** with LINQ:
   - Groups appointments by doctor
   - Counts total, scheduled, and completed appointments per doctor
   - Identifies the **most-booked doctor** (by total appointments)
4. **Outputs** a formatted summary:
   ```
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
   ```

---

## Success Criteria

### Core Requirements (Must-Have)

- [ ] Create `appointments.csv` with valid data
- [ ] Parse CSV into `List<Appointment>` without external CSV libraries
  - Handle quoted fields (e.g., `"Smith, John"`)
  - Handle missing/empty lines gracefully
  - Validate date parsing (yyyy-MM-dd HH:mm format)
- [ ] Define `Appointment` class with:
  - `int Id`
  - `string PatientName`
  - `string DoctorName`
  - `DateTime AppointmentDate`
  - `string Reason`
  - `AppointmentStatus` enum
- [ ] Use LINQ to group appointments by doctor
- [ ] Calculate:
  - Total appointments per doctor
  - Count by status (Scheduled, Completed, Cancelled)
  - Unique doctors and patients
  - Most-booked doctor
- [ ] Output formatted summary to console
- [ ] Handle errors gracefully:
  - Missing CSV file → display helpful message, don't crash
  - Invalid date format → skip line with warning
  - Empty file → display "No appointments found"

### Enhanced (Nice-to-Have)

- [ ] Command-line argument to specify CSV file path
- [ ] Export summary to text file
- [ ] Find busiest day (most appointments on one date)
- [ ] Calculate average appointments per doctor
- [ ] Sort doctors by appointment count (descending)
- [ ] Generate a simple statistics table

### Stretch (Challenge)

- [ ] Read CSV from stdin (pipe in data)
- [ ] Calculate doctor utilization rate (% scheduled vs completed)
- [ ] Generate per-patient appointment history
- [ ] Create a mini report with doctor recommendations

---

## Learning Goals

By completing this task, you'll demonstrate:

✅ **C# fundamentals**
- Strong typing with custom classes and enums
- Property initialization
- String parsing and validation

✅ **LINQ mastery**
- GroupBy (grouping and aggregation)
- Select (projection)
- OrderBy (sorting)
- Count with predicates (conditional counts)

✅ **File I/O**
- Reading files (`File.ReadAllLines()`)
- Error handling (missing files, format errors)

✅ **Data manipulation**
- Manual CSV parsing (no external libraries)
- DateTime parsing
- String manipulation

✅ **Production patterns**
- Graceful error handling
- User-friendly output
- Data validation

---

## Time Breakdown

| Phase | Time | Task |
|-------|------|------|
| **Setup** | 15 min | Create project, CSV file, data classes |
| **CSV Parser** | 30 min | Read file, parse lines, validate data |
| **LINQ Analysis** | 20 min | Group, count, find most-booked doctor |
| **Output Formatting** | 25 min | Build summary string with aligned columns |
| **Error Handling** | 15 min | Handle edge cases (missing file, invalid dates) |
| **Testing & Debug** | 15 min | Run, verify output, fix issues |
| **Enhancements** | 30 min | Add optional features (busiest day, export, etc.) |
| **Total** | **2–3 hours** | Complete, tested, working app |

---

## File Structure After Completion

```
AppointmentCsvParser/
├── Program.cs                    (main application logic)
├── appointments.csv              (sample data)
├── AppointmentCsvParser.csproj   (project file)
├── output_summary.txt            (optional: export)
├── bin/Debug/net8.0/            (compiled app)
└── obj/                          (build artifacts)
```

---

## Starting Point

```bash
# Create project
$ dotnet new console --name AppointmentCsvParser
$ cd AppointmentCsvParser

# Create CSV file (see Part 1 above, or use provided sample)
# Add data classes and parsing logic (see 02-starter-code.md)

# Build and run
$ dotnet build
$ dotnet run
```

---

## Important Notes

### Manual CSV Parsing (No Libraries)
Don't use `CsvHelper` or `OpenCsv`. Parse manually with `.Split(',')` and handle quoted fields:

```csharp
var line = @"""Smith, John"",Alice,2026-04-15";
var fields = line.Split(',');  // Simple case
// But quoted fields containing commas need special handling
```

**Hint**: If a field is quoted, it can contain commas safely. Strip quotes after splitting.

### DateTime Format
Appointments are in `yyyy-MM-dd HH:mm` format:
```csharp
var date = DateTime.ParseExact("2026-04-15 10:30", "yyyy-MM-dd HH:mm", null);
```

### Error Handling Strategy
- **File not found**: Catch `FileNotFoundException`, display message
- **Invalid date**: Catch `FormatException`, skip line with warning
- **Malformed line**: Check field count, skip gracefully
- **Empty file**: Check if appointments list is empty after parsing

---

## Example Output (What You're Building Toward)

```
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
```

---

## Rubric: Self-Assessment

Rate yourself on each dimension:

| Dimension | Nailed It | Mostly | Needs Work |
|-----------|-----------|--------|-----------|
| CSV parsing (handles quoted fields, errors) | ✓ | | |
| Data validation (dates, enum conversion) | ✓ | | |
| LINQ grouping and aggregation | ✓ | | |
| Output formatting (aligned, readable) | ✓ | | |
| Error handling (graceful, user-friendly) | ✓ | | |
| Code organization (classes, methods) | ✓ | | |
| Enhancements (at least 2) | ✓ | | |

**Goal**: All items "Nailed It" before moving to Week 2.

---

## What Comes Next

After this consolidation, you're ready for **Week 2 (Backend Infrastructure)**:

- **Day 8**: Entity Framework Core (replace CSV with database)
- **Day 9**: ASP.NET Core APIs (expose endpoints instead of CLI)
- **Day 10**: Integration (wire CSV parser into API)

This CSV app is a **prototype**. Week 2, you'll replace file I/O with database queries and console output with HTTP responses.

---

## Pro Tips

1. **Start simple**: Get CSV reading working first, then add parsing.
2. **Test incrementally**: Parse one line, verify it works, then parse all.
3. **Use LINQ interactively**: Write each LINQ query in isolation, test it.
4. **Format output last**: Get data right first, then make it pretty.
5. **Error handling**: Add try-catch **after** the happy path works.

---

## Next: Starter Code & Hints

See `02-starter-code.md` for scaffolding and approach hints.
