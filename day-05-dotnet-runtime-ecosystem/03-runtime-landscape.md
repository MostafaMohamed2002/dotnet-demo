# Day 5: .NET Runtime & Ecosystem – Runtime Landscape

## The Fork in the Road: .NET 8 vs .NET Framework 4.x

In 2016, Microsoft created a fork:

- **Old path**: .NET Framework 4.x (Windows-only, feature-frozen)
- **New path**: .NET Core 1.0 → 2.0 → ... → .NET 5 → ... → .NET 8 (cross-platform, actively developed)

In 2020, ".NET Core" was renamed to just ".NET". Today:
- **".NET"** = .NET 5, 6, 7, 8 (modern, recommended)
- **".NET Framework"** = 4.5, 4.7, 4.8.1 (legacy, Windows-only)

**You will use .NET 8.** Framework 4.x is for legacy enterprise codebases.

---

## Quick Comparison Table

| Aspect | .NET 8 | .NET Framework 4.x |
|--------|--------|-------------------|
| **Cross-platform** | ✅ Windows, Linux, macOS | ❌ Windows only |
| **License** | MIT (open source) | Proprietary |
| **Dev model** | Side-by-side (multiple versions) | Windows integral (one per OS) |
| **New features** | Added every 12 months | Frozen (no new features) |
| **Performance** | Optimized, fast startup | Slower, heavy |
| **Modern async** | Task, async/await | TPL (older async model) |
| **LINQ** | Modern (IAsyncEnumerable) | Older (IEnumerable only) |
| **Records** | ✅ Native support | ❌ Not available |
| **Nullable refs** | ✅ Native support | ❌ Not available |
| **NuGet** | Native integration | Add-on (package manager) |
| **Example version** | .NET 8.0.1 (latest) | 4.8.1 (frozen) |

---

## Why .NET Framework Is Dead (But Not Gone)

### The History

**2002**: .NET Framework 1.0 released.
**2009**: .NET Framework 4.0 released (TPL, PLINQ).
**2015**: .NET Framework 4.7.2 released (last major update).
**2016**: .NET Core 1.0 released (cross-platform rewrite).
**2020**: Microsoft ends active support for Framework 4.x.
**2024**: .NET 8 is stable and production-ready.

### Why It's Dead

1. **Windows-only**: Mobile, Linux, cloud (AWS EC2, Google Cloud) need cross-platform.
2. **Huge bloat**: Framework 4.x has 30 years of backwards compatibility (slow).
3. **No new features**: Can't add async patterns, records, nullable refs, etc.
4. **Hard to port**: Windows-only registry, GDI+, COM interop assumptions everywhere.

### Why It's Not Gone

Many enterprises still run **Windows Server 2019** with **Framework 4.8** for legacy applications. Migration costs are high. **You won't maintain these,** but they exist.

---

## Key Differences in Practice

### 1. Project Files (.csproj)

**Modern .NET 8 (.csproj):**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

**Old Framework 4.x (.csproj):**
```xml
<Project ToolsVersion="15.0" 
         DefaultTargets="Build" 
         xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
  </PropertyGroup>
  <!-- pages of boilerplate -->
</Project>
```

**Modern is 50x simpler.** The `Sdk="Microsoft.NET.Sdk"` import handles defaults.

### 2. Async Model

**Modern .NET (Task, async/await):**
```csharp
public async Task<Appointment> GetAppointmentAsync(int id)
{
    var response = await httpClient.GetAsync($"/api/appointments/{id}");
    return await response.Content.ReadAsAsync<Appointment>();
}
```

**Old Framework (TPL, harder):**
```csharp
public Task<Appointment> GetAppointment(int id)
{
    return httpClient.GetAsync($"/api/appointments/{id}")
        .ContinueWith(r => r.Result.Content.ReadAsAsync<Appointment>())
        .Unwrap();
}
```

Modern is **cleaner and safer** (keyword-based, not callback chains).

### 3. NuGet Integration

**Modern .NET**: NuGet is built-in. Install packages with:
```bash
dotnet add package Newtonsoft.Json
```

**Old Framework**: NuGet was add-on. Had to download, use package console, etc.

---

## Runtime Versioning & Support

### .NET 8 Release Cycle

```
.NET 8.0.0 (Nov 2023) ─────────────────────────→ Nov 2025 (EOL)
├─ LTS (Long-Term Support)
├─ Supported for 3 years
└─ Recommended for production
```

Each .NET release is either:
- **LTS** (Long-Term Support): 3 years of support (every 2 releases)
  - .NET 6 (LTS)
  - .NET 8 (LTS) ← Use this
- **STS** (Short-Term Support): 18 months of support
  - .NET 7 (STS, EOL in May 2024)
  - .NET 9 (STS, future)

**Rule**: Use LTS releases in production. Use STS for exploration.

### Support Window Example

```
.NET 8 (Nov 2023)
├─ Mainstream support: Nov 2023 – Nov 2024 (critical bug fixes)
├─ Servicing: Nov 2024 – Nov 2025 (security patches only)
└─ EOL: Nov 2025 (no more support)

.NET 9 (future, STS)
├─ Mainstream: release – 18 months (critical fixes)
└─ EOL: 18 months (no more support)
```

**For this course**: Target .NET 8 (LTS, stable until 2026+).

---

## When You Encounter Framework 4.x

You'll hit legacy codebases. Here's the decision tree:

```
You inherit a .NET Framework 4.x codebase
    ↓
Is it actively maintained and critical?
    ├─ Yes → Keep it running on Framework 4.x (minimal changes)
    ├─ No → Plan migration to .NET 8+ (will be many-month effort)
    └─ Uncertain → Ask PM/team lead

Migrating to .NET 8?
    ├─ Step 1: Run Microsoft's .NET Upgrade Assistant tool
    ├─ Step 2: Replace NuGet packages with .NET 8 equivalents
    ├─ Step 3: Fix deprecated APIs (Windows-specific calls)
    ├─ Step 4: Update async patterns (Task instead of TPL)
    └─ Step 5: Test extensively
```

**Timeline**: 2–8 weeks depending on codebase size and complexity.

---

## Breaking Changes in .NET 8

When migrating from Framework, you'll hit breaking changes. Common ones:

### 1. Windows APIs Not Available

```csharp
// This works in Framework 4.x (Windows-only)
using System.Windows.Forms;
var button = new Button();  // ❌ Not available in .NET 8

// Solution: Use cross-platform UI library
using System.Threading.Tasks;
// Plain async/await, no UI layer
```

### 2. Web APIs Changed (ASP.NET Core)

```csharp
// Framework 4.x (System.Net.Http)
var client = new HttpClient();

// Modern .NET (same API, but dependency injection model changed)
// See Day 6 for HttpClientFactory pattern
```

### 3. Configuration Model Changed

```csharp
// Framework 4.x (app.config XML)
<configuration>
  <connectionStrings>
    <add name="db" connectionString="..." />
  </connectionStrings>
</configuration>

// Modern .NET (appsettings.json)
{
  "ConnectionStrings": {
    "db": "..."
  }
}
```

**Key takeaway**: API surface is similar, but assumptions about Windows, registry, XML config are gone. Porting requires careful testing.

---

## Choosing Your Target Framework

### Decision Tree

```
What are you building?
├─ New backend service (most common)
│   └─ Use: .NET 8
├─ Library (NuGet package)
│   └─ Use: net6.0 (oldest LTS) for broad compatibility
├─ Desktop app (WPF, WinForms)
│   └─ Use: .NET 8 with WPF project template
├─ Migrating Framework 4.x codebase
│   └─ Use: .NET 8 (target, after migration effort)
└─ Maintaining Framework 4.x (legacy requirement)
    └─ Use: .NET Framework 4.8.1 (last version, then plan migration)
```

**For this course**: Always .NET 8.

### Multi-targeting (If You Must)

Some libraries support multiple frameworks:

```xml
<TargetFrameworks>net6.0;net8.0;net48</TargetFrameworks>
```

This builds three versions: .NET 6, .NET 8, and .NET Framework 4.8. **Don't do this unless you maintain a library used by legacy code.** It's complex.

---

## Side-by-Side Installation

One computer can have multiple .NET versions installed:

```bash
$ dotnet --list-sdks
6.0.401
8.0.1  ← We use this
```

Each project specifies which version via `<TargetFramework>`:

```xml
<TargetFramework>net8.0</TargetFramework>
```

**This is huge.** In Framework 4.x, one OS = one version. Modern .NET lets projects coexist.

---

## Summary: Framework Choice

| Scenario | Use | Reasoning |
|----------|-----|-----------|
| New backend app | .NET 8 | Modern, cross-platform, LTS |
| New library | net6.0 | Broad compatibility, easy maintenance |
| Migrating legacy | .NET 8 | Effort upfront, but future-proof |
| Maintaining legacy | .NET Framework 4.8 | Minimize changes, plan migration |
| Learning / course work | .NET 8 | Modern patterns, best practices |

**Bottom line**: Use .NET 8. Framework 4.x is legacy. You'll touch it eventually in an old codebase, but don't build new things with it.
