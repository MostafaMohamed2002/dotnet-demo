# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: You have a list of 1,000 Doctors. You want to get the total count of all appointments across all doctors. Which is better: looping through the doctors and summing their appointment counts, or using a single `.Select()` projection? Why?
2. **Conceptual**: If you see `SELECT * FROM Patients` in your logs, but your C# code has a `.Where(p => p.Id == 5)`, what is the most likely cause of this behavior?
3. **Code-based**: Look at this code. How many queries will be executed against the database?
   ```csharp
   var doctors = await _context.Doctors.Include(d => d.Appointments).ToListAsync();
   foreach(var d in doctors) {
       Console.WriteLine(d.Appointments.Count);
   }
   ```
4. **Code-based**: Rewrite the following code to avoid the N+1 problem using eager loading:
   ```csharp
   var patients = await _context.Patients.ToListAsync();
   foreach(var p in patients) {
       var lastAppt = p.Appointments.OrderByDescending(a => a.Date).First();
   }
   ```
5. **Scenario**: You are reviewing a colleague's code. They used `.AsNoTracking()` on a query, then modified one of the resulting objects, and called `await _context.SaveChangesAsync()`. Will the change be saved to the database? Why or why not?
