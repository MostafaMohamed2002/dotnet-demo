# Summary — Day 2: C# Types, Pattern Matching, and Exception Handling

## What You Learned

- **Type inference with `var`** is statically typed (not dynamic) and inferred at compile time; use it when the type is obvious from context
- **Pattern matching** (`is`, `switch` expressions) replaces complex if-else chains with readable, exhaustive case handling
- **Property patterns** allow you to destructure and match on object properties: `obj is { Name: "Bob", Age: > 30 }`
- **Tuples** provide lightweight, multi-value returns without allocating objects; use named tuples `(bool Success, string Message)` for clarity
- **Switch expressions** (C# 8+) are expression-based and exhaustiveness-checked; they're preferred over traditional switch statements
- **Enums** model domain constants (status, roles, states) and can have extension methods for behavior
- **Exception handling** in C# has no checked exceptions (unlike Java); catch specific exceptions, log or re-throw, never silently swallow
- **Custom exceptions** encode business logic errors (e.g., `DoctorNotAvailableException`); use them instead of generic exceptions

## What This Unlocks

Understanding pattern matching and enums means you can write validation logic that's simultaneously safe (compiler-verified exhaustiveness), readable (no nested ifs), and performant (no runtime overhead). Tuples eliminate wrapper-class overhead for simple returns. Exception handling becomes intentional: you decide exactly what to do with each error type. These features compound: a service class that combines pattern matching, tuples, and enums is shorter, clearer, and easier to maintain than imperative code.

## Revisit Before Next Session

Flag any shaky concept:
- Pattern matching syntax: do you understand property patterns vs. type patterns? (Revisit if unclear.)
- Switch expressions: why are they exhaustiveness-checked, and when does the compiler warn? (Revisit if unclear.)
- Tuples: when are they appropriate, and when should you use a class/record instead? (Revisit if unclear.)
- Exception handling: what's the difference between catching specific exceptions and re-throwing vs. silently returning? (Revisit if unclear.)

If all four are solid, you're ready for **Day 3: Async/Await and Concurrency in C#** (the bridge to backend services).
