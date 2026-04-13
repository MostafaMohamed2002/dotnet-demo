# Day 5: .NET Runtime & Ecosystem – Orient

## Why This Day Exists

For the first four days, you've been writing C# code—properties, patterns, async tasks, LINQ queries—and it all "just worked." But **where** does it work? **How** does it execute? **Why** does it matter?

This day pulls back the curtain on the **.NET runtime ecosystem**: the Common Language Runtime (CLR) that interprets your code, the garbage collector that frees memory automatically, the `.csproj` file that defines your project, and the `dotnet` CLI that orchestrates everything.

**Why now?** Before you write real backend applications (tomorrow: HTTP clients, database queries, dependency injection), you need to understand:
- What the CLR is and how JIT compilation works
- How garbage collection prevents memory leaks
- The difference between .NET 8 and .NET Framework 4.x
- What your `.csproj` file actually does
- What `dotnet restore`, `dotnet build`, `dotnet run` do under the hood

**This knowledge prevents:** mysterious performance issues, incorrect framework choices, broken deployments, and cargo-cult "why does adding this package break everything?" moments.

---

## From Kotlin to .NET: Runtime Model Shift

### Kotlin (on JVM)
- **Runtime**: JVM (Java Virtual Machine)
- **Compilation**: Kotlin → bytecode → JIT to native code at runtime
- **Memory**: Garbage collector (mark-and-sweep, generational)
- **Package manager**: Gradle, Maven (dependency resolution)
- **Dev loop**: `./gradlew build`, `./gradlew run`

### C# (on CLR)
- **Runtime**: CLR (Common Language Runtime)
- **Compilation**: C# → IL (Intermediate Language) → JIT to native code at runtime
- **Memory**: Garbage collector (generational, with Large Object Heap)
- **Package manager**: NuGet (dependency resolution)
- **Dev loop**: `dotnet build`, `dotnet run`

**The conceptual shift:** Both are managed runtimes with JIT compilation and GC. The naming is different (JVM vs CLR, Java bytecode vs IL, Maven vs NuGet), but the **execution model is nearly identical**. You're already thinking in managed runtime terms.

---

## What You'll Learn Today

### 1. **CLR & JIT Compilation** (02-core-concepts.md)
   - What the CLR is and what it does
   - IL (Intermediate Language) and how it differs from bytecode
   - JIT (Just-in-Time) compilation: when code turns from IL to native
   - RyuJIT optimizer and tiered compilation
   - How this affects startup time and runtime performance

### 2. **Garbage Collection** (02-core-concepts.md)
   - Generational GC: Gen0, Gen1, Gen2
   - Ephemeral heap (fast) vs Gen2 (slow)
   - Large Object Heap (LOH) and when objects land there
   - GC pauses and why you care about collection pressure
   - How `using` statements and `IDisposable` interact with GC

### 3. **.NET 8 vs .NET Framework 4.x** (03-runtime-landscape.md)
   - Which one to use (modern = .NET 8; legacy = Framework 4.x)
   - Key differences: cross-platform, side-by-side, feature-parity
   - Breaking changes and migration path
   - .NET EOL timeline and support windows

### 4. **Project Files & NuGet** (04-csproj-nuget.md)
   - SDK-style `.csproj` (modern, simple)
   - Old-style `.csproj` (Framework 4.x, verbose)
   - What `<TargetFramework>net8.0</TargetFramework>` means
   - NuGet package resolution and lock files
   - Transitive dependencies and version conflicts

### 5. **dotnet CLI** (05-dotnet-cli.md)
   - `dotnet new` (create project from template)
   - `dotnet restore` (download dependencies)
   - `dotnet build` (compile IL)
   - `dotnet run` (execute)
   - `dotnet add package` (add NuGet dependency)
   - `dotnet publish` (package for deployment)

### 6. **Hands-On Lab** (06-hands-on-task.md)
   - Create a console app with `dotnet new`
   - Inspect the `.csproj` file
   - Add a NuGet package
   - Understand transitive dependencies
   - Run and verify

---

## Checklist: What You Should Know by Day 5 EOD

- [ ] Explain the CLR and why it exists (vs native C++, vs JVM)
- [ ] Describe JIT compilation: IL → native code, when it happens
- [ ] Explain generational GC: Gen0 vs Gen1 vs Gen2
- [ ] Understand .NET 8 vs Framework 4.x and when to use each
- [ ] Read a `.csproj` file and understand `<TargetFramework>`, `<ItemGroup>`, `<PackageReference>`
- [ ] Know what NuGet lock files do
- [ ] Run `dotnet new`, `dotnet build`, `dotnet run`, `dotnet add package` confidently
- [ ] Understand why `dotnet restore` is needed before building

---

## Tomorrow: You'll Use This

Tomorrow (Day 6), you'll write your first real console applications:
1. An **HTTP client** using `System.Net.Http.HttpClient` and `System.Text.Json`
2. A **LINQ query app** that highlights deferred execution

Both of these depend on:
- NuGet packages (you'll add them with `dotnet add package`)
- The .NET 8 ecosystem (you'll understand what versions are compatible)
- The CLI (you'll build and run your apps)

**Today is the scaffolding. Tomorrow is the building.**

---

## How to Use This Day

1. **Read 02-core-concepts.md** – Deep dive on CLR, JIT, GC (technical but essential)
2. **Read 03-runtime-landscape.md** – Understand the .NET landscape (decision-making)
3. **Read 04-csproj-nuget.md** – Project files and package management
4. **Read 05-dotnet-cli.md** – CLI reference and commands
5. **Do 06-hands-on-task.md** – Light lab to apply the knowledge
6. **Skim 07-summary.md** – Recap and bridge to Day 6

**Time estimate**: 2–3 hours including the lab.

**Come back to this day** if you hit mysterious build errors, version conflicts, or questions like "Why isn't my package found?" or "What does `restore` actually do?"
