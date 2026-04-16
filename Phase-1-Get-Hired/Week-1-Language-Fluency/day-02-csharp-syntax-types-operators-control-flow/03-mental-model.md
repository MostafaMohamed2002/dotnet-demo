# MENTAL MODEL — Pattern Matching as Decision Trees

## The Analogy

Imagine a **triage nurse at the clinic intake desk**:

- **Type pattern** (`obj is Appointment`): "Is this an appointment object? If yes, process it as an appointment."
- **Property pattern** (`apt is { Status: Completed }`): "Is this appointment marked as completed? Check the status property."
- **Pattern combinations** (`apt is { Status: Completed } or { Status: Cancelled }`): "Is it either completed OR cancelled? Either path leads to the same action."

The **switch expression** is the entire decision tree. The nurse (compiler) walks through each branch in order, finds the matching condition, and executes the corresponding action.

---

## Where the Analogy Breaks Down

The analogy doesn't capture **exhaustiveness checking**. A real nurse might miss a patient condition (bad). C# pattern matching, when all cases are covered, **guarantees** no patient is unhandled. The compiler verifies this.

Also, the analogy suggests decisions are sequential (check one thing, then another). In reality, C# evaluates patterns in parallel-ish: it's all compile-time, so there's no performance cost to "checking multiple branches."

---

## Key Insight

Pattern matching is more powerful than if-else chains because:
1. **Readable:** Each case is concise and aligned
2. **Safe:** Compiler warns if you miss a case (exhaustiveness)
3. **Composable:** Combine patterns (AND, OR, NOT) without nesting

This is especially valuable in backend APIs where you're constantly deciding what to do based on object type, status, or structure. Get this right, and your code is self-documenting.
