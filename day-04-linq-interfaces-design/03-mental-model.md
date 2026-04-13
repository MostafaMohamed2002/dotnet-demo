# MENTAL MODEL — LINQ as a Recipe Language

## The Analogy

Think of LINQ as a **recipe written in advance**:

- **Composing a LINQ query** is like writing a recipe before the chef starts cooking.
- **`.Where()`, `.Select()`, `.OrderBy()`** are the steps in the recipe (deferred—the chef hasn't acted yet).
- **`.ToList()`, `.FirstOrDefault()`** are instructions to "start cooking now."
- **IQueryable (database)** is like handing the recipe to a chef (EF Core) who interprets it and decides the most efficient way to cook.
- **IEnumerable (in-memory)** is like cooking yourself: fetch all ingredients (data), then follow the recipe steps one by one.

```
Recipe (LINQ):
  1. Where specialty == "Cardiology"
  2. Select (FirstName, LastName, Specialty)
  3. OrderBy LastName
  4. Take 10

IQueryable path (chef interprets):
  Chef reads the recipe → decides to use a database index → SQL query sent → results returned
  (Efficient: only 10 rows fetched)

IEnumerable path (you cook):
  Fetch ALL doctors → filter by specialty in memory → project columns in memory → sort in memory → take 10
  (Inefficient: all rows fetched, then processed)
```

---

## Where the Analogy Breaks Down

The analogy doesn't capture:

1. **Expression trees.** A LINQ query is represented as an expression tree (an in-memory data structure), not literally a text recipe. EF Core inspects and translates this tree.

2. **Lazy evaluation.** The analogy suggests a recipe is "written" but not followed. In reality, a deferred LINQ query exists as an object in memory but is only "read" at enumeration.

3. **Composability as mutation.** In the analogy, each step in the recipe is fixed. In LINQ, you can fork the query and apply different steps to different branches without affecting the original.

---

## Key Insight

**IQueryable is a deal with the query provider (EF Core).** You say, "Here's what I want (expression tree)." The provider says, "I'll figure out how to get it efficiently (SQL optimization)." The cost of misunderstanding this is hitting your database with bad queries.

**IEnumerable is a contract with yourself.** You say, "I want to filter this list in memory." No provider optimization; you get what you ask for. This is fast for small lists, catastrophic for large tables if you accidentally use it on a database query.

---

## Practical Rule

> **If the source is an `IQueryable`, build your LINQ chain as an `IQueryable` until the very end. Materialize (`.ToList()`) only when you're ready to use the data.**

Break this rule, and you'll spend Friday evening debugging a query that loads a million rows into memory.
