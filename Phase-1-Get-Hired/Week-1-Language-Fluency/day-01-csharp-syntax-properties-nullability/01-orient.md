# ORIENT — C# Properties and Nullability

## Why This Exists

In Kotlin, you write `val name: String` and the language automatically generates getters (and setters if `var`). You barely think about it. C# takes a different path: **properties are first-class syntax**, not compiler magic. You write them explicitly, but you get fine-grained control over `get` and `set` behavior that Kotlin doesn't offer natively.

Nullability in Kotlin is part of the type system from day one: `String` is non-null, `String?` is nullable. C# 8+ added **nullable reference types** (NRT) as an *opt-in* feature, which means older .NET code and your codebase might or might not have it enabled. This is a source of confusion for Kotlin developers expecting null-safety to be automatic.

## Connection to Kotlin

You already know:
- **Kotlin `val`/`var`** → C# **`{ get; }` and `{ get; set; }` properties**
- **Kotlin null-safety (`String?`)** → C# **nullable reference types (`string?`), but opt-in**
- **Kotlin `data class`** → C# **`record` type** (added in C# 9, structurally similar)

The mental shift: in Kotlin, properties are syntax sugar. In C#, they're first-class citizens with explicit backing fields and accessor logic. This gives you more control but requires more ceremony.

## Where This Fits

Every class in a backend API has properties: `Doctor.Name`, `Appointment.ScheduledTime`, `Patient.Email`. Before you write your first domain model or Entity Framework mapping, you need to understand C#'s property system deeply—especially nullable reference types, which will catch bugs at compile time instead of at 2 AM in production.

---

**Next:** Core concepts break down properties, value types, and null handling with real code.
