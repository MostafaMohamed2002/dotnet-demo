# ORIENT

## Why This Topic Exists
The most dangerous part of using an ORM like EF Core is that it makes the database feel "invisible." Because you are dealing with C# objects and collections, it is easy to write code that looks efficient in C# but is catastrophic in SQL. The **N+1 Problem** is the most common performance killer in enterprise applications—it can turn a snappy API into a crawling one as soon as the database grows beyond a few dozen rows.

## Android/Kotlin Parallels
In Android, you might have encountered this when fetching a list of users from a local Room database and then performing a separate query for each user to get their profile picture or a list of their posts.
- **Room**: If you returned a `List<User>` and then iterated over it to fetch related data in a loop, you were performing N+1 queries.
- **EF Core**: The behavior is identical, but because EF Core manages complex object graphs and "lazy loading" (if enabled), this problem often happens silently without you even writing a manual loop.

## The Bigger Picture
Modern backend engineering isn't just about "making it work"; it's about **observability**. You cannot optimize what you cannot see. Today, we move from "trusting the ORM" to "verifying the SQL." Learning to read EF Core logs and understanding the boundary between `IQueryable` (database-side) and `IEnumerable` (memory-side) is what separates a junior developer from a senior engineer.
