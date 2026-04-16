# Summary — Day 15-17: Entity Framework Core

## What You Learned

- **DbContext & DbSet**: How to set up the gateway between C# and SQL Server.
- **Code-First Migrations**: The workflow of Entity $\rightarrow$ Migration $\rightarrow$ Database.
- **Navigation Properties**: Modeling One-to-Many and Many-to-Many relationships.
- **Eager Loading**: Using `.Include()` and `.ThenInclude()` to fetch related data in a single query.
- **Change Tracking**: Understanding how EF Core monitors objects and how to disable it with `.AsNoTracking()` for performance.
- **Async Data Access**: Using `SaveChangesAsync()` and `ToListAsync()` to keep the API responsive.

## What This Unlocks

You can now build a fully functional data layer that is maintainable and type-safe. You are no longer tethered to writing raw SQL strings in your C# code, though you now have the knowledge to verify that the ORM is generating efficient queries.

## Revisit Before Next Session

- Ensure you understand the "Identity Map" concept (how EF Core handles duplicate objects in one session).
- Practice the difference between `.Include()` and projection using `.Select()`.
