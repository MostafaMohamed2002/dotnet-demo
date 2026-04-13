# MENTAL MODEL — Properties as Gateways

## The Analogy

Think of a C# property as a **security checkpoint at the clinic entrance**:

- **Auto-property** (`{ get; set; }`): Open doors. People walk straight through. No questions asked.
- **Property with logic** (`get { return _field.Trim(); }`): Receptionist at the desk. Every visitor's name gets cleaned up before they're admitted.
- **Init-only property** (`{ get; init; }`): Sign-in sheet. You write your name once during registration, then security won't let you change it.

The backing field (`_field`) is the **storage room behind the checkpoint**. The property is the **public entrance**. You control what gets stored, how it's retrieved, and who can modify it.

---

## Where the Analogy Breaks Down

This breaks down when you think about **performance**:

- In real life, a receptionist filtering every visitor is a bottleneck.
- In C#, properties are **inlined by the JIT compiler** at runtime. The "receptionist" disappears; it's just direct field access.

So a property with logic has *zero* runtime cost—but it *does* cost compile time (the JIT must generate code). For property-heavy code (EF Core models with hundreds of properties), this is negligible.

**Another breakdown:** The mental model doesn't capture **nullable reference types**. A property being "nullable" is a *compile-time intent*, not a runtime behavior. `string?` and `string` are identical at runtime—NRT is purely for the compiler's type checker. This catches mistakes early (good), but it's invisible at runtime (confusing).

---

## Key Insight

Properties aren't "just getters/setters"—they're **syntax for controlled access to state**. Kotlin's `val`/`var` are simpler (no explicit properties), but C# gives you fine-grained control: you can make a field read-only, enforce initialization rules, add side effects, or validate on write. This control is essential when you're building a backend API with strict domain constraints.
