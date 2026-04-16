# HANDS-ON TASK

## Task
Design the relational schema for the Clinic Booking API and implement it using raw SQL in Azure Data Studio/SSMS.

## Files to Create / Modify
- `scripts/01-schema.sql` — The full script to create the database and tables.
- `scripts/02-seed.sql` — Insert at least 5 doctors, 5 patients, and 10 appointments.
- `scripts/03-queries.sql` — A set of a few complex queries to verify the design.

## Expected Behavior
1. **Schema**: All tables must have Primary Keys, and all relationships must be enforced with `FOREIGN KEY` constraints.
2. **Data Integrity**: Attempting to delete a User who has a linked Doctor record should fail (Referential Integrity).
3. **Performance**: Add a non-clustered index to the `DoctorId` and `PatientId` columns of the `Appointments` table.

## Do NOT Do This
- Do not use `VARCHAR` for names.
- Do not use "Generic" IDs (like strings) if an `INT IDENTITY` is more appropriate.
- Do not use a junction table for `Appointments` $\rightarrow$ `TimeSlots` (it's a 1:1 or N:1 relationship, not M:M).

## Self-Review Checklist
- [ ] Does every table have a Primary Key?
- [ ] Are all foreign keys explicitly defined?
- [ ] Did I use `NVARCHAR` for all text fields?
- [ ] Can I successfully run a `JOIN` query to find all appointments for a specific doctor?
- [ ] Did I include the `IDENTITY(1,1)` property for automatic ID increments?
