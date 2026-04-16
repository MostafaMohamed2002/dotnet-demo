# REAL-WORLD CONTEXT

## Production Implementation
In professional environments, you almost never run `CREATE TABLE` manually in production. Instead, you use **Migrations** (via EF Core or Flyway/DbUp). Migrations are version-controlled SQL scripts that ensure every environment (Dev, QA, Prod) has the exact same schema.

## Egypt Legacy Codebases: Stored Procedures
In many Egyptian corporate environments (Banking, Insurance, Government), you will encounter **Stored Procedures**. 

**What they are**: SQL code saved on the server. Instead of the C# app sending a complex `SELECT` query, it just calls `EXEC GetDoctorSchedules @DoctorId = 10`.

**Pros**: 
- Better performance (pre-compiled by SQL Server).
- Security (you can give a user permission to execute a procedure without giving them access to the raw tables).
**Cons**: 
- Business logic is split between C# and SQL, making it harder to debug and version control.

## The "Gotcha": The NVARCHAR Trap
In SQL Server, always use `NVARCHAR` instead of `VARCHAR` for text.
- `VARCHAR`: Uses 1 byte per character (ASCII).
- `NVARCHAR`: Uses 2 bytes per character (Unicode).
Since you are building a clinic API in Egypt, you will likely handle Arabic names. `VARCHAR` will turn Arabic characters into gibberish (???). **Always use `NVARCHAR` for any field that could contain non-English text.**
