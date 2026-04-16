# ORIENT — LINQ, IEnumerable, IQueryable, and Interface Composition

## Why This Exists

LINQ (Language-Integrated Query) unifies data querying: whether you're filtering a `List<T>` in memory or querying a SQL Server database, you use the same syntax. But LINQ has two very different execution models: **IEnumerable** (in-memory, eager evaluation with LINQ-to-Objects) and **IQueryable** (queryable, deferred translation to SQL or other backends). Getting this wrong costs you thousands—a missing `.Where()` before `.Select()` can cause a full-table scan in production.

Interfaces in modern C# (8+) support composition and default implementations. You'll design repositories, services, and data access patterns around interfaces. Understanding interface design means building flexible, testable backends that scale from a single microservice to distributed systems.

## Connection to Kotlin

You already know:
- **Kotlin `List<T>.filter()`, `List<T>.map()`** → C# **LINQ `IEnumerable<T>.Where()`, `.Select()`**
- **Kotlin type hierarchy with `interface`** → C# **interface composition** (now with default methods in C# 8+)
- **Kotlin eager evaluation** (`.map()` returns immediately) → C# **IEnumerable (eager on enumeration) vs IQueryable (deferred translation)**

The mental shift: In Kotlin, collection operations are optimized by the compiler. In C#, you must understand *which* LINQ provider you're using to know *when* code executes. IQueryable translates to SQL; IEnumerable runs in memory. Choose wrong, and a simple query becomes a distributed denial of service on your database.

## Where This Fits

Every backend query—filtering appointments by status, paginating patient lists, aggregating doctor schedules—uses LINQ. Every service class depends on injected repositories, and those repositories are built on IEnumerable/IQueryable interfaces. Understanding LINQ and interfaces deeply means writing queries that are fast, readable, and testable. It's the difference between a backend that handles 100 concurrent users and one that handles 10,000.

---

**Next:** Core concepts break down LINQ execution, IEnumerable vs IQueryable, and interface design patterns with clinic domain examples.
