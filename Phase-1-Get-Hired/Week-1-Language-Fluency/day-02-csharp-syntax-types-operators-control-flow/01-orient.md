# ORIENT — Advanced C# Syntax for Backend Development

## Why This Exists

C# has features that aren't in Kotlin but solve real backend problems: pattern matching for complex validation, tuples for returning multiple values without allocating objects, enums for domain-driven design, and operators you can overload for domain-specific logic. There's also `var` and type inference—which *looks* like Kotlin but behaves differently—and string interpolation that goes way beyond `$""`.

Exception handling in C#, while similar to Java/Kotlin, has nuances around checked vs. unchecked exceptions (C# doesn't distinguish) and finally blocks. And sometimes you need to work with `dynamic` types or use reflection in middleware—dangerous, but you need to recognize it.

## Connection to Kotlin

You know:
- **Kotlin sealed classes** → C# **pattern matching** (similar intent, different syntax)
- **Kotlin `data class`** → C# **tuples** (returning multiple values)
- **Kotlin `when` expressions** → C# **switch expressions** (more powerful in modern C#)
- **Kotlin's `by lazy`** → C# **properties with backing fields** (lazy initialization)
- **Kotlin exceptions** → C# **exceptions** (similar, but no checked exceptions)

The mental shift: C# is less functional than Kotlin. Pattern matching is powerful, but it's not as deeply integrated into the type system. However, C# 11+ has added incredible pattern matching capabilities that rival functional languages.

## Where This Fits

Every API endpoint you write will use string interpolation, exception handling, and pattern matching. Tuples appear in method returns. Enums model domain constants (appointment status, user roles, etc.). Understanding these features means writing safer, more concise backend code with fewer bugs.

---

**Next:** Core concepts cover type inference, pattern matching, tuples, enums, and exception handling with real examples.
