# Summary — Day 13–14: SQL Server & Database Design Thinking

## What You Learned
- The philosophy of **Normalization (1NF, 2NF, 3NF)** to prevent data redundancy.
- How to translate a real-world domain (Clinic) into a relational schema with PKs and FKs.
- Fluent T-SQL for schema definition (`CREATE`, `ALTER`) and data retrieval (`JOIN`, `GROUP BY`).
- The performance impact of **Clustered vs Non-Clustered Indexes**.
- The role of **Stored Procedures** in legacy enterprise systems.
- The critical importance of `NVARCHAR` for internationalization (Arabic support).

## What This Unlocks
You now have a persistent data layer. You no longer rely on "mock" lists. You can design a system that can scale to millions of records while maintaining data integrity and query performance.

## Revisit Before Next Session
- Practice writing `JOIN` queries with multiple tables.
- Ensure you can explain *why* a specific column needs an index.
- Review the difference between `INNER JOIN` and `LEFT JOIN`.
