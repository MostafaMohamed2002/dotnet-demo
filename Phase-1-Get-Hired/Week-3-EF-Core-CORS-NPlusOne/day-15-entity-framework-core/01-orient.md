# ORIENT

## Why This Topic Exists
In Day 13–14, you wrote raw T-SQL. In a professional environment, writing every single query manually is slow, error-prone, and creates a maintenance nightmare. **Entity Framework Core (EF Core)** is an Object-Relational Mapper (ORM). It bridges the gap between the object-oriented world of C# and the relational world of SQL Server, allowing you to manipulate data using C# objects while the ORM generates the optimized SQL for you.

## Android/Kotlin Parallels
If you used **Room** in Android, you have already used an ORM.
- **`DbContext` $\approx$ `RoomDatabase`**: The central point of interaction with the database.
- **`DbSet<T>` $\approx$ `@Dao` (Simplified)**: While Room uses DAOs for specific queries, `DbSet<T>` provides a generic gateway to a table.
- **Migrations $\approx$ Room Migrations**: The process of evolving the database schema without losing data.
- **Eager Loading $\approx$ `@Relation`**: Using `.Include()` in EF Core is conceptually similar to how Room uses `@Relation` to fetch related entities.

## The Bigger Picture
EF Core is the industry standard for .NET backends. However, the "magic" of ORMs can lead to severe performance degradation if you don't understand the generated SQL. The goal for these days is to master the **Code-First** workflow: defining your domain model in C# and letting EF Core handle the database schema, while remaining conscious of the performance cost of the "Change Tracker."
