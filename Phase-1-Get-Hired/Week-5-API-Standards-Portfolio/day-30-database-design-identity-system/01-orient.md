# ORIENT

## Why this exists
A production-grade backend is only as stable as its data model. Before writing a single line of API logic, we must define the "Source of Truth." In the Clinic API, the relationship between Patients, Doctors, and Appointments is the core of the business. Establishing a rigorous schema prevents "data drift" and ensures that business rules (like "one doctor cannot be in two places at once") are enforced at the lowest possible level.

## Android/Kotlin Parallels
If you've used **Room** in Android, you're familiar with `@Entity` and `@ForeignKey`. EF Core Migrations are the equivalent of Room's migration scripts, but instead of manually writing SQL for every version, EF Core generates the delta between your C# classes (the "Desired State") and the DB schema (the "Current State"). 

C#'s **Identity Framework** is a high-level abstraction similar to how you might use **Firebase Auth** or a custom Auth library in Kotlin—it handles the boilerplate of password hashing, user storage, and role management so you don't have to implement the security primitives from scratch.

## The Big Picture
Today is about the **Infrastructure Layer**. We are building the bedrock:
1. **Persistence**: Designing the tables and relationships in SQL Server.
2. **Identity**: Implementing a secure wall using JWT (JSON Web Tokens) to distinguish between an Admin, a Doctor, and a Patient.
Everything built in the next three days will sit on top of this foundation.
