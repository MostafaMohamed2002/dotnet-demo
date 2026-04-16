# Day 7: Consolidation Task – CSV Parser & LINQ Analysis

## What You're Building

A **CLI tool** that reads clinic appointment data from a CSV file, parses it into C# objects, analyzes with LINQ, and outputs a formatted summary report.

**Duration**: 2–3 hours  
**Difficulty**: Intermediate (bridges all Days 1–6)  
**Portfolio Value**: High (real data parsing + LINQ + reporting)

---

## The Challenge (In One Sentence)

Read `appointments.csv` → Parse into `List<Appointment>` → Group by doctor using LINQ → Output formatted summary showing each doctor's appointment stats.

---

## Files in This Directory

| File | Purpose |
|------|---------|
| **01-task-spec.md** | Full requirements, success criteria, time breakdown |
| **02-starter-code.md** | Scaffolding, hints, function templates |
| **03-walkthrough.md** | Complete working solution with explanations |
| **04-enhancements.md** | 10 optional extensions (search, export, interactive menu) |
| **appointments.csv** | Sample data (10 appointments, 3 doctors) |
| **README.md** | This file |

---

## Quick Start

### 1. Create Project
```bash
dotnet new console --name AppointmentCsvParser
cd AppointmentCsvParser
```

### 2. Copy Files
- Copy `appointments.csv` into the project directory
- Use code from `02-starter-code.md` or `03-walkthrough.md` in `Program.cs`

### 3. Build & Run
```bash
dotnet build
dotnet run
```

### Expected Output
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

... (rest of output)
```

---

## Core Requirements

✅ **Must-Have**:
- Parse CSV without external libraries
- Create `Appointment` class + `AppointmentStatus` enum
- Use LINQ to group by doctor
- Calculate counts (total, scheduled, completed, cancelled)
- Output formatted summary
- Handle errors gracefully (missing file, invalid dates)

✅ **Enhanced**:
- Command-line arguments
- Export to file
- Calculate statistics (busiest day, average appointments)

✅ **Stretch**:
- Interactive menu
- Search functionality
- Multiple export formats (JSON, HTML)
- Doctor utilization report

---

## Learning Outcomes

By completing this task, you'll demonstrate:

| Skill | Evidence |
|-------|----------|
| **C# Fundamentals** | Custom classes, enums, string parsing, DateTime handling |
| **LINQ Mastery** | GroupBy, Select, OrderBy, Count with predicates, Distinct |
| **File I/O** | Reading CSV, error handling, file not found |
| **Data Validation** | Checking field counts, parsing dates, enum conversion |
| **Output Formatting** | StringBuilder, aligned columns, readable summaries |
| **Error Handling** | Try-catch, graceful degradation, user-friendly messages |
| **Code Organization** | Separate classes, methods, concerns |

---

## Suggested Approach

**Phase 1: Setup (15 min)**
1. Create project
2. Copy `appointments.csv`
3. Define `Appointment` class and enum

**Phase 2: CSV Parser (30 min)**
1. Read file with `File.ReadAllLines()`
2. Split lines by comma
3. Validate field count
4. Parse DateTime
5. Handle errors and skip invalid lines

**Phase 3: LINQ Analysis (20 min)**
1. Group appointments by doctor
2. Count total, scheduled, completed, cancelled per doctor
3. Find unique doctors and patients
4. Identify most-booked doctor

**Phase 4: Output Formatting (25 min)**
1. Build summary string with `StringBuilder`
2. Align columns and format nicely
3. Display results

**Phase 5: Error Handling (15 min)**
1. Missing CSV file
2. Invalid dates
3. Invalid enum values
4. Empty fields

**Phase 6: Testing (15 min)**
1. Run with valid data
2. Test error cases (missing file, invalid line)
3. Verify output matches expected format

**Phase 7: Enhancements (30 min)** (Optional)
1. Command-line arguments
2. Export to file
3. One additional feature (search, utilization, statistics)

---

## Code Quality Checklist

- [ ] No magic numbers (use constants or named variables)
- [ ] Meaningful variable names (`appointmentsByDoctor` not `dict1`)
- [ ] Comments on complex logic
- [ ] Error handling with specific exceptions (not bare `catch`)
- [ ] No dead code or commented-out lines
- [ ] Consistent formatting and indentation
- [ ] Separation of concerns (parsing, analysis, output in different methods/classes)

---

## Common Pitfalls (Avoid These)

| Pitfall | Solution |
|---------|----------|
| String concatenation in loops (slow) | Use `StringBuilder` |
| Not validating field count | Check `fields.Length == 6` |
| Not trimming fields | Use `.Trim()` on each field |
| Hardcoding date format | Use `DateTime.ParseExact()` with format string |
| Not handling missing file | Catch `FileNotFoundException` |
| Not handling invalid dates | Catch `FormatException` |
| Single large `Main()` method | Extract logic to separate methods/classes |
| Not grouping LINQ results | Remember to `.ToList()` after grouping |

---

## Testing Scenarios

### Test 1: Happy Path ✅
- Run with valid `appointments.csv`
- Verify output shows correct counts

### Test 2: Missing File ❌
- Delete or rename `appointments.csv`
- Run and verify error message (not crash)

### Test 3: Invalid Date ❌
- Edit one line to have invalid date (e.g., `2026-99-99`)
- Run and verify warning, line skipped, others processed

### Test 4: Invalid Status ❌
- Edit one line to have invalid status (e.g., `Pending`)
- Run and verify warning, line skipped

### Test 5: Empty Lines ❌
- Add blank lines in CSV
- Run and verify program continues

---

## Expected Folder Structure

```
AppointmentCsvParser/
├── Program.cs                    (Your code)
├── AppointmentCsvParser.csproj   (Project file)
├── appointments.csv              (Sample data)
├── bin/
│   └── Debug/
│       └── net8.0/
│           └── AppointmentCsvParser.exe
├── obj/
│   └── (Build artifacts)
└── appointment_summary.txt       (Output, if you add export)
```

---

## Progression: After Completing This Task

✅ You understand **data parsing** (CSV input validation)  
✅ You can **manipulate collections** with LINQ (grouping, counting, filtering)  
✅ You can **format output** for human readers (tables, summaries)  
✅ You handle **errors gracefully** (missing files, format issues)  
✅ You write **production-quality code** (separation of concerns, error handling)

**Next**: Week 2 will replace CSV with a **database** (EF Core) and replace console output with **HTTP endpoints** (ASP.NET Core). Same data model, new I/O layers.

---

## Reading Order

1. **Start here**: `01-task-spec.md` (understand what you're building)
2. **Build**: Use hints from `02-starter-code.md` OR full solution from `03-walkthrough.md`
3. **Extend**: Pick 2–3 enhancements from `04-enhancements.md`
4. **Refine**: Ensure all error cases are handled

---

## Example: What "Done" Looks Like

```
$ dotnet run

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

## Pro Tips

1. **Start with the happy path**: Get CSV parsing working before adding error handling
2. **Test incrementally**: Parse one line, print it, verify format. Then parse all.
3. **Use LINQ interactively**: Write one query at a time, test each before combining
4. **Format output last**: Get data right first, then make it pretty
5. **Save your code**: You'll build on this in Week 2 (replace CSV with database)

---

## Questions?

If stuck:
- Check the hints in `02-starter-code.md`
- Look at the complete solution in `03-walkthrough.md`
- Test error cases one at a time
- Verify CSV format matches expected schema

---

## Your Mission

Build a production-quality CLI tool that reads, parses, analyzes, and reports appointment data. No external CSV libraries. All manual string parsing. Pure LINQ analysis. Graceful error handling.

**Time**: 2–3 hours  
**Difficulty**: Intermediate  
**Reward**: A real, working application that bridges everything from Week 1

---

**You've got this. Start with 01-task-spec.md and build toward done.** 🚀
