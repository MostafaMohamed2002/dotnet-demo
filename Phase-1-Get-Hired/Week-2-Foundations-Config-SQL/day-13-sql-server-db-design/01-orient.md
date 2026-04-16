# ORIENT

## What is this and why does it exist?
Up until now, your data lived in memory and vanished every time you restarted the app. To build a real system, you need a **Relational Database Management System (RDBMS)**. SQL Server is the industry standard for .NET backends.

The goal of this session is NOT just to learn a tool, but to learn **Database Design Thinking**. Writing a query is easy; designing a schema that doesn't collapse under its own weight after six months is the hard part. We focus on the "blueprint" before we start "building."

## Android/Kotlin Parallels
If you've used **Room** in Android, you've already dealt with a simplified version of this. Room uses SQLite, which is a "lite" version of the same relational concepts.
- `Entity` in Room $\approx$ `TABLE` in SQL Server.
- `@ForeignKey` in Room $\approx$ `FOREIGN KEY` constraint in SQL.
- `@Query` in Room $\approx$ Writing raw T-SQL in SQL Server.
The big difference is that in a backend, the database is a standalone server that exists independently of your application.

## The Bigger Picture
Database design is the most critical part of any backend. A bug in your C# code can be fixed with a hotfix in minutes. A mistake in your database schema (like forgetting a foreign key or choosing the wrong data type for a primary key) can require hours of downtime and complex data migrations to fix.
