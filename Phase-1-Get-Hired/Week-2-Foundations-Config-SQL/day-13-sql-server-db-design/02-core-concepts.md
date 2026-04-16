# CORE CONCEPT BREAKDOWN

## 1. Design Thinking: Normalization
Normalization is the process of organizing data to reduce redundancy and improve data integrity.

- **1NF (Atomic Values)**: No "lists" in a cell. You don't store `PhoneNumbers` as "010..., 012..." in one column. You create a separate `PhoneNumbers` table.
- **2NF (Full Functional Dependency)**: Every non-key column must depend on the *entire* primary key.
- **3NF (No Transitive Dependency)**: Columns should only depend on the primary key, not on other non-key columns. (e.g., Don't store `City` and `State` in the `User` table if they depend on a `ZipCode` column).

**The "Feeling" of a Wrong Schema**: If you find yourself updating the same piece of information (like a Doctor's phone number) in three different tables, your schema is **denormalized** and wrong.

## 2. The Clinic Schema (Manual Design)
Before using an ORM, we map the relationships:
- **Users**: `Id (PK)`, `Username`, `PasswordHash`, `Email`, `Role` (Admin/Doctor/Patient).
- **Doctors**: `Id (PK)`, `UserId (FK)`, `Specialization`, `ConsultationFee`.
- **Patients**: `Id (PK)`, `UserId (FK)`, `DateOfBirth`, `Gender`.
- **TimeSlots**: `Id (PK)`, `DoctorId (FK)`, `StartTime`, `EndTime`, `IsBooked`.
- **Appointments**: `Id (PK)`, `PatientId (FK)`, `TimeSlotId (FK)`, `Status` (Scheduled/Cancelled/Completed).

**Junction Tables**: If a Patient can have multiple Doctors and a Doctor has multiple Patients, you need a `PatientDoctor` junction table.

## 3. Essential T-SQL Mastery
You must be fluent in these operations:

```sql
-- Create with Constraints
CREATE TABLE Patients (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Patients_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Complex Retrieval (The "Money" Query)
SELECT p.FullName, d.Specialization, a.AppointmentDate
FROM Appointments a
INNER JOIN Patients p ON a.PatientId = p.Id
INNER JOIN Doctors d ON a.DoctorId = d.Id
WHERE a.Status = 'Scheduled' AND d.Specialization = 'Cardiology'
ORDER BY a.AppointmentDate ASC;
```

## 4. Indexes: Speed vs. Storage
An index is like the index at the back of a book—it tells SQL Server exactly where the data is without scanning the whole table.

- **Clustered Index**: The table itself is sorted by this key. You get **one** per table (usually the Primary Key).
- **Non-Clustered Index**: A separate "lookup table."
  - *Example*: Adding an index to `DoctorId` in the `Appointments` table. 
  - *Why?* Because you will frequently run `SELECT * FROM Appointments WHERE DoctorId = @id`. Without an index, SQL performs a "Full Table Scan" (reads every single row), which is slow.

**The Trade-off**: Every index slows down `INSERT`, `UPDATE`, and `DELETE` because the index must also be updated.
