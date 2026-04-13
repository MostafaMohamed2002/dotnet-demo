# QUIZ — Day 1: C# Properties and Nullability

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): Init-Only Properties

What is the difference between `{ get; set; }` and `{ get; init; }`, and why would you use `init` in a domain model?

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

---

## Q4 (Scenario-Based): Designing a Property

You are designing a `Patient` entity for the clinic API. The patient must have an `Email` (required), a `PhoneNumber` (optional), and a `CreatedAt` timestamp (set once, never changed by outside code, but can be set in the constructor).

Write the three property declarations using the appropriate syntax (`{ get; set; }`, `{ get; init; }`, nullable types). Do not write backing fields.

---

## Q5 (Conceptual): Nullable Value Types

Why does `int?` (nullable int) need different handling than `int`? What does `HasValue` represent, and how is it different from checking `null` on a reference type like `string?`?

---

**When you've answered all five, paste them into the chat and I'll grade them.**
