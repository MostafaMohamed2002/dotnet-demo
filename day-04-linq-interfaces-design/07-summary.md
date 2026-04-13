# Summary — Day 4: LINQ, IQueryable vs IEnumerable, and Interface Design

## What You Learned

- **LINQ is a query abstraction** unifying in-memory and server-side querying. Two implementations: LINQ-to-Objects (IEnumerable, in-memory) and LINQ-to-SQL (IQueryable, server-side via EF Core).
- **Deferred execution:** `.Where()`, `.Select()`, `.OrderBy()` don't execute immediately. They build an expression tree. Execution happens at enumeration (`.ToList()`, `.FirstOrDefault()`, `.Count()`).
- **IQueryable translates to SQL.** EF Core builds an expression tree and converts it to SQL. Filtering happens on the database server, not in memory.
- **IEnumerable is in-memory only.** All operations run in-memory. Using `.ToList()` before `.Where()` on a database query causes a full-table scan—catastrophic for large tables.
- **N+1 problem:** Loading related data one query per entity. Fixed with `.Include()` (eager load) or projections (fetch only what you need).
- **Projection (`.Select()`) reduces data transfer.** Select only needed columns; translate to `SELECT` in SQL.
- **Pagination with `.Skip().Take()`** translates to `OFFSET/FETCH` in SQL.
- **Interface segregation:** Separate `IReadRepository<T>` and `IRepository<T>` so services depend only on what they need.
- **Default interface implementations (C# 8+)** allow interfaces to evolve without breaking implementers, but use sparingly.

## What This Unlocks

Mastering LINQ and interfaces means you can write efficient, composable backends. Queries that naturally scale—a single service method might serve millions of users because the query is optimized in SQL, not executed in memory. Repository patterns with interface composition make your code testable and flexible. You understand the difference between a backend that scales and one that collapses under load.

## Revisit Before Next Session

Flag any shaky concept:
- IQueryable vs IEnumerable: which is which, and when does each apply? (Revisit if unclear.)
- Deferred execution: which operations defer, and which force execution? (Revisit if unclear.)
- N+1 problem: what causes it, and how do you fix it? (Revisit if unclear.)
- Repository pattern: why separate IReadRepository from IRepository? (Revisit if unclear.)

If all four are solid, you've completed **Week 1 of the "Get Hired" phase.** You now understand:
- Day 1: C# properties, nullability, immutability
- Day 2: Pattern matching, enums, exception handling
- Day 3: Async/await, Task patterns, concurrency
- Day 4: LINQ, IQueryable, and interface design

You're ready for **Week 2: Entity Framework Core fundamentals** (Day 5–7), where you'll build on LINQ and async to master data persistence, relationships, and migrations.
