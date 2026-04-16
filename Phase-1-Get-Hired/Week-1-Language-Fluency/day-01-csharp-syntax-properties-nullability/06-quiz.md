# QUIZ — Day 1: C# Properties and Nullability

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): Init-Only Properties

What is the difference between `{ get; set; }` and `{ get; init; }`, and why would you use `init` in a domain model?

**ANSWER:**
- `{ get; set; }` allows the property to be read and written at **any time** after the object is created
- `{ get; init; }` allows the property to be set **only during initialization** (constructor or object initializer), then it becomes immutable
- **Why use `init`**: In a domain model, you want to enforce immutability for critical business logic. Once a `Doctor`'s ID or creation timestamp is set, it should never be modified by outside code, protecting the integrity of your domain entities.

---

## Q2 (Code-Based): Nullable Reference Types

```csharp
#nullable enable

public class Doctor
{
    public string Name { get; set; }
    public string? MiddleName { get; set; }
}

var doc = new Doctor { Name = "Alice" };
string middle = doc.MiddleName;  // Line X
```

What error or warning does the compiler produce at Line X? Explain why.

**ANSWER:**
The compiler produces a **warning/error**: `Cannot implicitly convert type 'string?' to 'string'`

**Why:** 
- `doc.MiddleName` is of type `string?` (nullable string) — it could be `null`
- `middle` is declared as type `string` (non-nullable) — it must never be `null`
- You cannot assign a potentially-null value to a non-nullable variable without checking for null first
- **Fix**: Either change `string middle` to `string? middle`, or add null-checking: `string middle = doc.MiddleName ?? "";`

---

## Q3 (Code-Based): Value Types vs. Reference Types

```csharp
public struct Coordinate
{
    public int X { get; set; }
}

var c1 = new Coordinate { X = 10 };
var c2 = c1;
c2.X = 20;

Console.WriteLine(c1.X);  // What is printed, and why?
```

**ANSWER:** Prints **`10`**

**Why:**
- `struct` is a **value type** — when you do `c2 = c1`, the entire struct is **copied** by value
- `c1` and `c2` are now **separate copies** in memory
- Modifying `c2.X = 20` only affects the copy in `c2`, not `c1`
- Therefore `c1.X` remains `10`

---

## Q4 (Scenario-Based): Designing a Property

You are designing a `Patient` entity for the clinic API. The patient must have an `Email` (required), a `PhoneNumber` (optional), and a `CreatedAt` timestamp (set once, never changed by outside code, but can be set in the constructor).

Write the three property declarations using the appropriate syntax (`{ get; set; }`, `{ get; init; }`, nullable types). Do not write backing fields.

**ANSWER:**
```csharp
public string Email { get; set; }
public string? PhoneNumber { get; set; }
public DateTime CreatedAt { get; init; }
```

**Reasoning:**
- `Email`: Required (non-nullable), readable and writable
- `PhoneNumber`: Optional (nullable `string?`), readable and writable
- `CreatedAt`: Set once during initialization, never modified after → `init`-only

---

## Q5 (Conceptual): Nullable Value Types

Why does `int?` (nullable int) need different handling than `int`? What does `HasValue` represent, and how is it different from checking `null` on a reference type like `string?`?

**ANSWER:**
- `int?` is actually `Nullable<int>` — a **struct wrapper** around `int` that adds the ability to represent "no value"
- `HasValue` is a **boolean property** that indicates whether a value is present (not a null check like with reference types)
- **Key difference**: 
  - With `string?`, `null` **is** the actual value
  - With `int?`, `null` is represented as a special state where `HasValue = false`, and the actual int value is stored separately in the backing field
- You check it with: `if (age.HasValue) { ... }` or use null-coalescing: `age ?? 0`

---

**All answers completed!**
