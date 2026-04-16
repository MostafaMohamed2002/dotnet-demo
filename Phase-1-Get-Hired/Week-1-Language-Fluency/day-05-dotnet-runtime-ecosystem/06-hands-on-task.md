# Day 5: .NET Runtime & Ecosystem – Hands-On Task

## Lab: Hands-On .NET Ecosystem

You're going to create a console app from scratch, inspect its files, add a NuGet package, and understand the full build pipeline.

**Time**: 30–45 minutes

**Success criteria**:
- [ ] Create a new console app with `dotnet new`
- [ ] Examine and understand the `.csproj` file
- [ ] Add a NuGet package and see changes
- [ ] Inspect generated files (`obj/`, `bin/`)
- [ ] Build and run successfully
- [ ] Verify transitive dependencies

---

## Part 1: Create a New Project

### Step 1: Create the app

```bash
$ dotnet new console --name AppointmentUtil

# Output:
# The template "Console Application" was created successfully.
# Processing post-creation actions...
# Restore succeeded.
```

### Step 2: Examine the directory structure

```bash
$ cd AppointmentUtil
$ ls -la

AppointmentUtil/
├─ Program.cs           (entry point)
├─ AppointmentUtil.csproj  (project blueprint)
├─ obj/
│  ├─ AppointmentUtil.csproj.nuget.cache
│  ├─ AppointmentUtil.csproj.nuget.g.props
│  └─ ... (other generated files)
└─ (no bin/ yet – haven't built)
```

**What these are:**
- `Program.cs`: Your code entry point
- `AppointmentUtil.csproj`: Project configuration (THIS IS KEY)
- `obj/`: Build artifacts (ignore for now)

---

## Part 2: Examine the .csproj File

### Step 3: Open `AppointmentUtil.csproj`

```bash
$ cat AppointmentUtil.csproj
```

**Output (typical):**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

### Step 4: Understand each element

| Element | Value | Meaning |
|---------|-------|---------|
| `<Sdk>` | `Microsoft.NET.Sdk` | Use standard .NET console app SDK (brings in defaults) |
| `<OutputType>` | `Exe` | Create executable (not a library) |
| `<TargetFramework>` | `net8.0` | Compile for .NET 8.0 |
| `<ImplicitUsings>` | `enable` | Auto-include common `using` statements (convenience) |
| `<Nullable>` | `enable` | Enable nullable reference types (modern C#) |

**Observations:**
- This is **tiny** (5 XML lines)
- No manual references to .NET libraries
- No package list yet

---

## Part 3: Add a NuGet Package

### Step 5: Add the Newtonsoft.Json package

```bash
$ dotnet add package Newtonsoft.Json

# Output:
#   Determining projects to restore...
#   Writing /home/user/AppointmentUtil/AppointmentUtil.csproj
#   Restoring packages for /home/user/AppointmentUtil/AppointmentUtil.csproj...
#   Restoring packages for /home/user/AppointmentUtil/AppointmentUtil.csproj...
#   ...
#   Successfully added reference to package 'Newtonsoft.Json' version '13.0.3'
```

### Step 6: Inspect the updated `.csproj`

```bash
$ cat AppointmentUtil.csproj
```

**Output:**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

</Project>
```

**What changed:**
- Added `<ItemGroup>` with a `<PackageReference>` to Newtonsoft.Json 13.0.3
- That's it. The package is declared, NuGet downloaded it, and it's ready to use.

### Step 7: Check transitive dependencies

```bash
$ dotnet list package

Project 'AppointmentUtil' has the following package references
   [net8.0]:
   Top-level Package      Requested   Resolved
   > Newtonsoft.Json      13.0.3      13.0.3
```

**Show transitive dependencies:**

```bash
$ dotnet list package --include-transitive

Project 'AppointmentUtil' has the following package references
   [net8.0]:
   Top-level Package      Requested   Resolved
   > Newtonsoft.Json      13.0.3      13.0.3

   Transitive Package     Resolved
```

In this case, Newtonsoft.Json 13.0.3 has **no transitive dependencies** (modern, self-contained).

---

## Part 4: Inspect Generated Directories

### Step 8: Build the project

```bash
$ dotnet build

# Output:
# Microsoft (R) Build Engine version 17.x.x for .NET
# Determining projects to restore...
# Restoring AppointmentUtil/AppointmentUtil.csproj...
#   Installed 1 package in 1.23s
# Building...
# AppointmentUtil -> /home/user/AppointmentUtil/bin/Debug/net8.0/AppointmentUtil.dll
# 
# Build succeeded.
```

### Step 9: Explore the `bin/` directory

```bash
$ ls -la bin/Debug/net8.0/

AppointmentUtil.dll           (compiled IL – your code)
AppointmentUtil.exe           (Windows executable – runner)
AppointmentUtil.pdb           (debug symbols – used by debugger)
AppointmentUtil.deps.json     (dependency manifest)
AppointmentUtil.runtimeconfig.json  (runtime settings)
Newtonsoft.Json.dll           (the package you added)
```

**Key files:**
- `AppointmentUtil.dll`: Your compiled code (IL bytecode)
- `Newtonsoft.Json.dll`: The NuGet package (compiled)
- `.pdb`: Symbol file (maps compiled code back to source for debugging)

### Step 10: Explore the `obj/` directory

```bash
$ ls -la obj/Debug/net8.0/

AppointmentUtil.AssemblyInfo.cs     (auto-generated assembly metadata)
AppointmentUtil.AssemblyInfoInputs.cache
AppointmentUtil.csproj.CoreCompileInputs.cache (timestamps of compiled files)
AppointmentUtil.csproj.FileListAbsolute.txt    (list of all build outputs)
AppointmentUtil.csproj.nuget.g.props (transitive dependency list – generated)
AppointmentUtil.csproj.nuget.g.targets
```

**What this is:**
- **Never edit manually.** These are generated by the build system.
- Tracks what was built and when.
- Run `dotnet clean` to delete all of `obj/` and `bin/` if needed.

---

## Part 5: Write Code Using the Package

### Step 11: Update `Program.cs`

```csharp
using Newtonsoft.Json;

// Sample clinic appointment data (using Newtonsoft.Json)
var appointmentJson = @"
{
    ""id"": 101,
    ""patientName"": ""Alice Johnson"",
    ""doctorName"": ""Dr. Smith"",
    ""appointmentDate"": ""2026-04-15T10:30:00"",
    ""reason"": ""Checkup""
}";

// Parse JSON using Newtonsoft.Json
var appointment = JsonConvert.DeserializeObject<Appointment>(appointmentJson);

Console.WriteLine($"Appointment ID: {appointment.Id}");
Console.WriteLine($"Patient: {appointment.PatientName}");
Console.WriteLine($"Doctor: {appointment.DoctorName}");
Console.WriteLine($"Date: {appointment.AppointmentDate:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine($"Reason: {appointment.Reason}");

// Define a simple data class
public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; }
}
```

### Step 12: Build and run

```bash
$ dotnet build
Build succeeded.

$ dotnet run

# Output:
# Appointment ID: 101
# Patient: Alice Johnson
# Doctor: Dr. Smith
# Date: 2026-04-15 10:30:00
# Reason: Checkup
```

**What happened:**
1. You used `JsonConvert.DeserializeObject<T>()` from Newtonsoft.Json
2. The compiler found it in the NuGet package
3. Build succeeded because the dependency was available
4. Runtime executed the deserialization correctly

---

## Part 6: Check Build Artifacts

### Step 13: Verify the output DLL contains your code

```bash
$ ls -lh bin/Debug/net8.0/

-rw-r--r-- AppointmentUtil.dll           (5 KB – your code)
-rw-r--r-- Newtonsoft.Json.dll           (650 KB – the package)
```

**Observation:**
- Your app is 5 KB (just the Appointment class + Program code)
- Newtonsoft.Json is 650 KB (full JSON parsing library)
- Both are bundled together when you run

### Step 14: Publish for distribution

```bash
$ dotnet publish -c Release -o ./publish

# Output:
# Microsoft (R) Build Engine...
# Restoring AppointmentUtil...
# AppointmentUtil -> /home/user/AppointmentUtil/bin/Release/net8.0/AppointmentUtil.dll
# AppointmentUtil -> /home/user/AppointmentUtil/publish
```

```bash
$ ls -lh publish/

AppointmentUtil.dll           (optimized)
AppointmentUtil.exe           (runner)
Newtonsoft.Json.dll           (package)
AppointmentUtil.runtimeconfig.json
```

**This is what you'd deploy to production.** Everything needed to run is in `publish/`.

---

## Self-Review Checklist

Before calling this lab done, verify all of these:

- [ ] `dotnet new console` created a valid project
- [ ] `.csproj` file is minimal and readable (5-7 lines of properties)
- [ ] `dotnet add package Newtonsoft.Json` updated `.csproj` with `<PackageReference>`
- [ ] `dotnet list package` shows Newtonsoft.Json with resolved version
- [ ] `dotnet build` compiled successfully and created `bin/Debug/net8.0/` with `.dll` files
- [ ] `obj/` directory exists with generated build metadata (not edited by hand)
- [ ] Code in `Program.cs` uses `JsonConvert` from the package
- [ ] `dotnet run` executed successfully and output the appointment data
- [ ] `dotnet publish -c Release` created a `publish/` folder with all files needed for deployment
- [ ] You understand the difference between `bin/` (outputs) and `obj/` (build internals)

---

## Reflection Questions

1. **Why does `.csproj` not list transitive dependencies?**
   - Answer: NuGet resolves them automatically. Only top-level dependencies are listed.

2. **What happens when you run `dotnet build` without a prior `dotnet restore`?**
   - Answer: Build auto-restores. But explicit restore is clearer for CI/CD.

3. **Why is `obj/` safe to delete but `bin/` shouldn't be manually edited?**
   - Answer: Both are generated. `obj/` is intermediate; `bin/` contains outputs you might copy. `dotnet clean` removes both safely.

4. **Where does NuGet download packages to?**
   - Answer: Global cache at `~/.nuget/packages/` (Windows: `%USERPROFILE%\.nuget\packages\`). Linked into projects, not copied.

5. **Why is the Release build (~15 MB) larger than the Debug output (~5 MB) on disk but faster at runtime?**
   - Answer: Release includes optimized native images (ahead-of-time) compiled by `dotnet publish` with R2R (ready-to-run).

---

## What This Lab Teaches

✅ **Hands-on understanding of:**
- Project creation and structure
- `.csproj` blueprint (minimal but powerful)
- NuGet package management and versioning
- Build artifact directories (`bin/`, `obj/`)
- Compilation pipeline (C# → IL → native)
- Using third-party packages in code
- Publishing for deployment

---

## Common Pitfalls

| Pitfall | Fix |
|---------|-----|
| "Package not found" after adding | Run `dotnet restore` explicitly |
| Build succeeds but `bin/` is missing | Check output path in `.csproj` or rebuild |
| Edited `obj/` files and build breaks | Run `dotnet clean`, don't edit `obj/` manually |
| Trying to use package before `dotnet add` | Always `dotnet add package` first |
| Multiple `dotnet run` calls without changes | No rebuilds occur; code is cached |

---

## Up Next (Day 6)

Tomorrow, you'll use this knowledge to build two real console apps:
1. **HTTP Client**: Using `System.Net.Http` to fetch data from an API
2. **LINQ Queries**: Using deferred execution with in-memory data

Both depend on:
- Creating projects with `dotnet new`
- Adding packages with `dotnet add package`
- Building and running with `dotnet build/run`
- Understanding the assembly outputs

You're ready.
