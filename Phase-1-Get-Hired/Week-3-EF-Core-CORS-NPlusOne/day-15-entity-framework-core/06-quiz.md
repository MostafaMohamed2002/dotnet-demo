# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: Explain the difference between `IQueryable<T>` and `IEnumerable<T>` when using EF Core. Which one results in the filter being executed on the SQL Server?
2. **Conceptual**: Why is `.AsNoTracking()` recommended for read-only API endpoints? What specifically is it disabling?
3. **Code-based**: You want to fetch a Patient, their Appointments, and for each Appointment, the Doctor assigned to it. Write the `.Include()` chain required to achieve this.
4. **Code-based**: Identify the bug in this snippet:
   ```csharp
   public async Task UpdatePatientName(int id, string newName) {
       var p = await _context.Patients.AsNoTracking().FirstAsync(x => x.Id == id);
       p.Name = newName;
       await _context.SaveChangesAsync();
   }
   ```
5. **Scenario**: You are adding a `Specialization` table. A Doctor can have multiple specializations, and a specialization can belong to multiple doctors. How would you model this in EF Core using the "Code-First" approach?
