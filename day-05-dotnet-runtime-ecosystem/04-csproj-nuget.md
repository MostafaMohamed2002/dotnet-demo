# Day 5: .NET Runtime & Ecosystem – Project Files & NuGet

## The .csproj File: Your Project Blueprint

The `.csproj` (C# Project) file defines:
- What .NET version to target
- What NuGet packages your code depends on
- Compiler options and build settings
- Output paths and frameworks

### Modern SDK-Style .csproj (Recommended)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
</Project>
```

| Element | Meaning |
|---------|---------|
| `<TargetFramework>` | Which .NET version: `net8.0`, `net6.0`, `net48` |
| `<OutputType>` | `Exe` (console app) or `Library` (DLL) |
| `<Nullable>` | Enable nullable reference types (modern best practice) |
| `<PackageReference>` | NuGet package dependency with version |

### Old Framework 4.x .csproj (Don't Write This)

```xml
<Project ToolsVersion="15.0" DefaultTargets="Build" 
         xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{GUID}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <!-- many more lines... -->
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <!-- manual reference list... -->
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

**Ugh.** Modern `.csproj` is 10x simpler because defaults are smart.

---

## Key Properties

### TargetFramework

Specifies which .NET version to target:

```xml
<TargetFramework>net8.0</TargetFramework>
```

| Value | Meaning |
|-------|---------|
| `net8.0` | .NET 8.0 (modern, recommended) |
| `net6.0` | .NET 6.0 (LTS, compatible) |
| `net48` | .NET Framework 4.8 (old, Windows-only) |
| `net6.0-windows` | .NET 6 + Windows APIs (desktop apps) |
| `net8.0-linux` | .NET 8 + Linux APIs (uncommon) |

**Rule**: Use `net8.0` unless you have a specific reason (library compatibility, legacy requirement).

### OutputType

```xml
<OutputType>Exe</OutputType>
```

| Value | Meaning |
|-------|---------|
| `Exe` | Executable console app (.exe or binary) |
| `Library` | Class library (.dll) |
| `WinExe` | Windows GUI app (no console window) |

### Nullable

```xml
<Nullable>enable</Nullable>
```

Enables nullable reference types (safety feature from Day 1). **Always enable for new projects.**

### Other Common Properties

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <LangVersion>latest</LangVersion>       <!-- Use latest C# version features -->
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>  <!-- Compiler warnings = build failure -->
  <Deterministic>true</Deterministic>    <!-- Reproducible builds -->
</PropertyGroup>
```

---

## NuGet: .NET Package Manager

NuGet is how you install third-party libraries. It's like Maven (Java) or Gradle (Kotlin).

### Adding a Package

```bash
dotnet add package Newtonsoft.Json
```

This:
1. Connects to nuget.org
2. Downloads the latest stable version
3. Adds it to your `.csproj`:
   ```xml
   <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
   ```

### Specifying a Version

```bash
# Latest stable
dotnet add package Newtonsoft.Json

# Specific version
dotnet add package Newtonsoft.Json --version 13.0.1

# Pre-release (alpha, beta, rc)
dotnet add package Newtonsoft.Json --prerelease
```

### In the .csproj

```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
</ItemGroup>
```

---

## Transitive Dependencies

When you add a package, it brings its own dependencies. These are **transitive dependencies**.

### Example

```
Your App
  ├─ Newtonsoft.Json 13.0.3
  │   ├─ depends on: System.Reflection.Emit 4.7.0
  │   └─ depends on: System.Reflection.Emit.Lightweight 4.7.0
  └─ HttpClient.Timeout 1.0.0
      └─ depends on: System.Net.Http 4.3.0
```

**Your `.csproj` only lists direct dependencies**, but you get all transitive ones.

### Dependency Resolution

NuGet automatically resolves transitive dependencies. But sometimes conflicts occur:

```
Your App
  ├─ Library A requires: Newtonsoft.Json >= 13.0
  └─ Library B requires: Newtonsoft.Json 12.0
```

NuGet picks the highest version (13.0.3) that satisfies both constraints. If no overlap exists, the build fails.

**Solution**: Use `dotnet list package --vulnerable` to find conflicts and `dotnet remove package` to drop one library if needed.

---

## .csproj Files vs packages.config (Legacy)

### Old Way (Framework 4.x): packages.config

```xml
<!-- packages.config -->
<packages>
  <package id="Newtonsoft.Json" version="13.0.3" allowedVersions="[13.0,14)" />
  <package id="log4net" version="2.0.14" allowedVersions="[2.0,3)" />
</packages>
```

Separate from `.csproj`. Hard to reason about.

### Modern Way: .csproj Only

```xml
<!-- SampleApp.csproj -->
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <PackageReference Include="log4net" Version="2.0.14" />
</ItemGroup>
```

Single source of truth. Much cleaner.

---

## NuGet Lock Files

Problem: Your team member installs a package, gets version 13.0.3 (latest). Next week, NuGet releases 13.1.0. Another team member installs, gets 13.1.0. Code behaves differently. **Non-reproducible builds.**

**Solution**: `.csproj` specifies `13.0.3`, but a **lock file** (`.csproj.nuget.g.props`) pins exact transitive dependency versions:

```
Your `dotnet restore` run downloads:
├─ Newtonsoft.Json 13.0.3
├─ System.Reflection.Emit 4.7.0 (transitive)
└─ ... (all exact versions)

Lock file records all exact versions.

Next person runs `dotnet restore`:
├─ Reads lock file
├─ Downloads exact same versions (13.0.3, 4.7.0, ...)
└─ Build is identical
```

### Enabling Lock Files

```bash
dotnet restore --use-lock-file
```

This creates `packages.lock.json`:

```json
{
  "version": 1,
  "dependencies": {
    "net8.0": {
      "Newtonsoft.Json": {
        "type": "Direct",
        "requested": "[13.0.3, )",
        "resolved": "13.0.3"
      },
      "System.Reflection.Emit": {
        "type": "Transitive",
        "resolved": "4.7.0"
      }
    }
  }
}
```

**Commit `packages.lock.json` to Git.** It ensures reproducible builds across your team.

---

## Common NuGet Packages for Backend Development

| Package | Purpose |
|---------|---------|
| `Microsoft.AspNetCore.App` | Web API framework (included in ASP.NET Core templates) |
| `Entity.Framework.Core` | ORM for database access |
| `Newtonsoft.Json` or `System.Text.Json` | JSON serialization |
| `Serilog` | Structured logging |
| `FluentValidation` | Input validation |
| `AutoMapper` | Object mapping |
| `xUnit` or `NUnit` | Unit testing |
| `Moq` | Mocking library for tests |
| `Polly` | Resilience (retries, timeouts) |

You'll add these as needed. For now (Day 6), you'll use:
- `System.Net.Http` (built-in for HTTP)
- `System.Text.Json` (built-in for JSON)

---

## Troubleshooting NuGet Issues

### "Package Not Found"

```bash
$ dotnet add package NonExistentLib
error NU1101: Unable to find package NonExistentLib
```

**Fix**: Check spelling, search nuget.org manually.

### "Version Conflict"

```
error NU1107: Version conflict detected. Newtonsoft.Json 13.0 vs 12.0
```

**Fix**: 
```bash
dotnet remove package LibraryB
# Then manually choose version
dotnet add package LibraryB --version 2.0  # Newer version without conflict
```

### "Restore Fails After .csproj Edit"

```bash
$ dotnet restore
# Fails with MSBuild errors
```

**Fix**: Clean and restore:
```bash
dotnet clean
dotnet restore
```

---

## Summary: Project Files & NuGet

| Concept | What It Is | Why You Care |
|---------|-----------|-------------|
| `.csproj` | Project blueprint (target framework, packages) | Defines what you're building and dependencies |
| `TargetFramework` | Which .NET version | `net8.0` for modern apps |
| `OutputType` | Exe or Library | Exe = console app, Library = DLL |
| `PackageReference` | NuGet dependency | Installed with `dotnet add package` |
| **Transitive dep** | Dependencies of dependencies | Resolved automatically; can cause conflicts |
| **Lock file** | Pins exact transitive versions | Reproducible builds across team |
| `dotnet restore` | Download dependencies | Must run before `dotnet build` |

---

## Kotlin Comparison: Maven/Gradle vs NuGet

| Aspect | Maven/Gradle (Kotlin) | NuGet (.NET) |
|--------|----------------------|-------------|
| **Config file** | `pom.xml` / `build.gradle` | `.csproj` |
| **Add dependency** | Edit XML / `gradle add` | `dotnet add package` |
| **Transitive resolution** | Maven Central repo | NuGet.org |
| **Lock file** | `gradle.lock` / `pom.lock` (optional) | `packages.lock.json` (optional) |
| **Version syntax** | `[1.0, 2.0)` | `[1.0, 2.0)` (same) |
| **Pre-release** | `1.0-beta` | `1.0-beta` (same) |

**Bottom line**: Nearly identical. Maven, Gradle, and NuGet all resolve dependencies the same way. Lock files achieve reproducibility in all three.

---

## Key Takeaway

Your `.csproj` file is small and readable. NuGet automatically downloads dependencies and resolves versions. Lock files ensure reproducible builds. This is far simpler than Framework 4.x's manual reference lists.

Next: Learn how the `dotnet` CLI actually builds and runs your project.
