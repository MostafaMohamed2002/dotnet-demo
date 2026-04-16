# Summary — Day 1: C# Properties and Nullability

## What You Learned

- **Auto-properties** (`{ get; set; }`) generate backing fields automatically; **init-only properties** (`{ get; init; }`) enforce immutability after construction
- **Nullable reference types** (`string?`) are a compile-time feature (opt-in via `#nullable enable`) that forces you to handle null safely; runtime, `string` and `string?` are identical
- **Nullable value types** (`int?`, `DateTime?`) wrap primitive types in a `Nullable<T>` struct with a `HasValue` flag; this is different from reference null-safety
- **Value types** (struct, int, bool) are stack-allocated and copied by value; **reference types** (class, record, string) are heap-allocated and copied by reference
- **Record types** (`record`) are reference types optimized for immutability and value-based equality; use `with` expressions to create modified copies
- **Properties can include logic** (custom getters/setters) without runtime overhead—the JIT inlines them
- **Domain modeling starts with properties:** immutability, nullability, and initialization rules encode business constraints

## What This Unlocks

Understanding C# properties and nullability is the foundation for building type-safe domain models. These models are the spine of your backend API—they define what data is valid, what can change, and what's required. Once you master this, Entity Framework Core mapping, validation, and business logic will feel natural.

## Revisit Before Next Session

Flag any shaky concept:
- How is `init` different from `set`? (Revisit if unclear.)
- Why is nullable reference types opt-in, and what's the difference between `string` and `string?`? (Revisit if unclear.)
- Value types vs. reference types—when does this matter? (Revisit if unclear.)

If all three are solid, you're ready for **Day 2: C# Syntax — Types, Operators, and Control Flow**.
