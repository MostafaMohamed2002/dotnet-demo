# Day 5: .NET Runtime & Ecosystem – Summary

## What You Learned

### 1. The CLR & JIT Compilation
- **CLR**: Runtime that loads, verifies, and executes managed IL
- **IL**: Portable bytecode format (language-independent)
- **JIT**: Compiles IL to native code at runtime
  - First call is slow (compilation overhead)
  - Subsequent calls use cached native code
- **RyuJIT**: Modern optimizer (inlining, dead code elimination, constant folding)
- **Tiered Compilation**: Fast JIT (Tier 0) + aggressive JIT (Tier 1) for faster startup

### 2. Garbage Collection
- **Generational heap**: Gen0 (fast, young), Gen1 (intermediate), Gen2 (slow, old), LOH (>85KB)
- **Gen0 collection**: Frequent, fast (microseconds)
- **Gen2 collection**: Rare, expensive (milliseconds)
- **GC pauses**: Stop-the-world collections cause latency spikes
- **High allocation**: Increases GC pressure; write allocation-free code when possible
- **IDisposable**: For unmanaged resource cleanup (files, connections); use `using` statements

### 3. .NET 8 vs .NET Framework 4.x
- **.NET 8**: Modern, cross-platform, actively developed, LTS support until Nov 2025
- **.NET Framework 4.x**: Legacy, Windows-only, feature-frozen, EOL in sight
- **Use .NET 8 for new projects.** Framework 4.x is maintenance-only for legacy enterprise.
- **Breaking changes in migration**: Windows APIs removed, async models updated, config format changed

### 4. Project Files & NuGet
- **`.csproj` file**: Defines target framework, packages, compiler options
  - Modern SDK-style: `<Sdk="Microsoft.NET.Sdk">`
  - Old Framework style: verbose, thousands of lines (don't write this)
- **`<TargetFramework>`**: `net8.0`, `net6.0`, `net48` (choose `net8.0`)
- **`<PackageReference>`**: NuGet dependencies (added with `dotnet add package`)
- **Transitive dependencies**: Resolved automatically by NuGet
- **Lock files** (`packages.lock.json`): Pin exact transitive versions for reproducible builds

### 5. dotnet CLI
| Command | Purpose |
|---------|---------|
| `dotnet new console` | Create console app from template |
| `dotnet restore` | Download dependencies |
| `dotnet build` | Compile to IL |
| `dotnet run` | Build + execute |
| `dotnet add package` | Install NuGet package |
| `dotnet remove package` | Uninstall package |
| `dotnet list package` | Show dependencies |
| `dotnet publish` | Package for deployment |
| `dotnet clean` | Remove build artifacts |
| `dotnet test` | Run unit tests |

**Most common workflow:**
```bash
dotnet new console -n MyApp
cd MyApp
dotnet add package Newtonsoft.Json
dotnet build
dotnet run
```

### 6. Build Artifact Directories
- **`bin/Debug/net8.0/`**: Compiled output (DLLs, EXEs, symbols)
  - `.dll`: Your compiled code
  - `.pdb`: Debug symbols (breakpoints, variable inspection)
  - `*.deps.json`: Dependency manifest
- **`obj/Debug/net8.0/`**: Build internals (don't edit manually)
  - Generated metadata, compile caches, property files
  - Safe to delete; run `dotnet clean`

---

## Key Mental Models

### 1. Two-Stage Compilation
```
C# source → C# compiler → IL (portable) → JIT → Native code (fast)
                            ↑                    ↑
                         Compile-time        Runtime
```

**Why this matters**: Same IL runs on Windows, Linux, macOS. Type safety checked at load time. JIT optimizes for actual runtime behavior.

### 2. Generational Garbage Collection
```
Young objects (Gen0) ─→ Frequent, fast collection ✅
                 ↓
Surviving objects (Gen1) ─→ Less frequent ✅
                 ↓
Long-lived objects (Gen2) ─→ Rare, expensive ⚠️
```

**Why this matters**: Allocate aggressively in short-lived contexts (request handling). Avoid allocations in loops or long-lived caches.

### 3. .NET Landscape
```
Use .NET 8 (modern)
     ↓
     └─ New projects ✅
     └─ Migration from Framework 4.x (2–8 weeks effort) 🔄

Legacy .NET Framework 4.x (EOL)
     └─ Maintenance only ❌
```

### 4. Build Pipeline
```
.csproj (blueprint) ─→ dotnet restore (download) ─→ dotnet build (compile IL)
                              ↓
                     bin/, obj/ created
                              ↓
                        dotnet run (JIT + execute)
```

---

## Checklist: What You Should Know

- [ ] Explain the CLR and why it exists
- [ ] Describe IL and how it's portable
- [ ] Understand JIT compilation (when it happens, caching, overhead)
- [ ] Explain generational GC (Gen0 vs Gen1 vs Gen2)
- [ ] Know .NET 8 vs Framework 4.x and when to use each
- [ ] Read and understand a `.csproj` file
- [ ] Know what NuGet packages and transitive dependencies are
- [ ] Run `dotnet new`, `dotnet add package`, `dotnet build`, `dotnet run` confidently
- [ ] Understand `bin/` (outputs) vs `obj/` (internals)
- [ ] Know what `dotnet restore`, `dotnet build`, `dotnet publish` do
- [ ] Understand lock files and why they matter for team reproducibility

---

## Kotlin → C# Bridges

| Concept | Kotlin / JVM | C# / CLR |
|---------|------------|----------|
| **Bytecode** | JVM bytecode | IL (Intermediate Language) |
| **JIT** | C2 compiler (aggressive) | RyuJIT + tiered compilation |
| **Memory** | Garbage collected (GC) | Garbage collected (GC) |
| **Package manager** | Maven / Gradle | NuGet |
| **Build tool** | ./gradlew | dotnet CLI |
| **Project file** | build.gradle.kts | `.csproj` |
| **Add dependency** | Edit build.gradle + build | `dotnet add package` |
| **Startup** | Slower (full JIT first) | Faster (tiered compilation) |

**Kotlin knowledge transfers directly.** Both are managed runtimes with JIT and GC. Naming is different; execution model is nearly identical.

---

## What This Unlocks (Day 6)

With Day 5 knowledge, you're ready to:
1. **Create new projects** from templates (`dotnet new`)
2. **Add dependencies** from NuGet (`dotnet add package`)
3. **Build and run** production-like applications (`dotnet build`, `dotnet run`)
4. **Understand error messages** better (IL vs runtime, JIT compilation issues, GC pauses)
5. **Deploy applications** (`dotnet publish`)

Tomorrow (Day 6), you'll build two real console apps:
- **HTTP Client** fetching from an API (uses `System.Net.Http`)
- **LINQ Query Engine** with deferred execution (uses `System.Linq`)

Both depend on the CLI, NuGet, and build pipeline you learned today.

---

## Where to Go for Deep Dives

If you want to understand specific topics better:

### CLR & JIT
- **Official RyuJIT docs**: https://github.com/dotnet/coreclr/blob/master/Documentation/botr/ryujit-overview.md
- **JIT compilation performance**: https://github.com/dotnet/docs/blob/main/docs/core/runtime-config/jit.md

### Garbage Collection
- **GC overview**: https://github.com/dotnet/docs/blob/main/docs/standard/garbage-collection/index.md
- **Large Object Heap**: https://github.com/dotnet/docs/blob/main/docs/standard/garbage-collection/large-object-heap.md
- **GC performance**: https://github.com/dotnet/docs/blob/main/docs/standard/garbage-collection/performance.md

### .NET Ecosystem
- **.NET 8 release notes**: https://github.com/dotnet/core/releases/tag/v8.0.0
- **.NET 8 breaking changes**: https://github.com/dotnet/docs/blob/main/docs/core/compatibility/8.0.md
- **SDK-style csproj guide**: https://github.com/dotnet/docs/blob/main/docs/core/project-sdk/overview.md

### NuGet & Package Management
- **NuGet docs**: https://learn.microsoft.com/en-us/nuget/
- **NuGet lock files**: https://github.com/NuGet/Home/wiki/Enable-repeatable-package-restore-using-lock-file

### dotnet CLI
- **dotnet CLI reference**: https://github.com/dotnet/docs/blob/main/docs/core/tools/index.md

---

## Common Questions (FAQ)

**Q: Is .NET 8 stable enough for production?**
A: Yes. .NET 8 is LTS (Long-Term Support) until Nov 2026. Stable, tested, used by Microsoft internally.

**Q: Do I need Visual Studio? Can I use VS Code?**
A: Visual Studio / Rider are conveniences. All work happens via `dotnet` CLI. VS Code + OmniSharp works fine.

**Q: Why are GC pauses a problem?**
A: In real-time systems (trading, robotics, games), 100ms pause = missed deadline. In typical web apps (CRUD APIs), it's less critical but still impacts tail latency.

**Q: Can I use a different GC algorithm?**
A: Yes, but rare. Default generational GC is tuned for most workloads. Custom config: `DOTNET_TieredCompilation=0`, etc. (advanced).

**Q: Why is my Release build slower at compile time but faster at runtime?**
A: Release enables aggressive JIT optimization. Compiler spends more time optimizing; runtime is faster.

**Q: How do I choose between .NET 6 and .NET 8?**
A: Both are LTS. .NET 8 is newer with more features and optimizations. Use .NET 8 unless you need compatibility with older platforms.

---

## Recap: Three Ideas to Carry Forward

1. **Managed runtime is fast**: JIT compilation and tiered compilation mean your code runs at near-native speed while staying safe.

2. **Garbage collection frees you from memory management**: But allocations matter. Write allocation-free code in hot paths.

3. **.NET CLI is your main tool**: IDE is sugar. Learn `dotnet new`, `add package`, `build`, `run`, `publish` well. They're portable across Windows, Mac, Linux.

---

## Next: Day 6 – First Real Code

Tomorrow, you'll use all of this to build:
1. An **HTTP Client** app (fetch data from an API, parse JSON)
2. A **LINQ Query** app (in-memory queries, deferred execution, eager materialization)

These are real patterns you'll use every day in backend development. Day 5 gave you the **scaffolding**. Day 6 is the **building**.

**You're ready. See you tomorrow.**
