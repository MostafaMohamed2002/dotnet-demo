# Day 5: .NET Runtime & Ecosystem – dotnet CLI

## The dotnet Command

The `dotnet` CLI is your primary tool for building, running, and managing .NET projects.

```bash
$ dotnet --version
8.0.1
```

All major workflows go through `dotnet`. No Visual Studio required (though Rider and VS Code integrate it).

---

## Essential Commands

### dotnet new

Create a new project from a template.

```bash
# Create a console app
$ dotnet new console --name MyApp

# Creates:
# MyApp/
# ├─ Program.cs (entry point)
# ├─ MyApp.csproj (project file)
# └─ obj/ (build artifacts, temporary)
```

**Common templates:**

```bash
dotnet new console        # Console app (what we're building)
dotnet new classlib       # Class library (DLL for others to use)
dotnet new web            # ASP.NET Core web app
dotnet new mvc            # ASP.NET Core MVC app
dotnet new webapi         # ASP.NET Core API
dotnet new xunit          # Unit test project
```

**Interactive template selection:**

```bash
$ dotnet new --search "api"
# Lists templates matching "api"
```

### dotnet restore

Download NuGet dependencies.

```bash
$ dotnet restore
Determining projects to restore...
Restoring MyApp/MyApp.csproj...
Installed packages for MyApp/MyApp.csproj
```

**When you need it:**
- After cloning a repo (dependencies not on disk)
- After adding a package with `dotnet add package`
- After editing `.csproj` manually
- On CI/CD pipelines before building

**Technically optional before build**: `dotnet build` auto-restores, but explicit `restore` is clearer.

### dotnet build

Compile your project to IL.

```bash
$ dotnet build
Microsoft (R) Build Engine version 17.x.x for .NET
Determining projects to restore...
Restoring MyApp/MyApp.csproj...
  Restored MyApp/MyApp.csproj in 1.23s
Building...
MyApp -> /home/user/MyApp/bin/Debug/net8.0/MyApp.dll

Build succeeded.
```

**Outputs:**
- `.dll` (intermediate, in `bin/Debug/net8.0/`)
- `.exe` (on Windows, in `bin/Debug/net8.0/`)

**Configuration:**
```bash
dotnet build --configuration Release   # Optimized build (slower compile, faster run)
dotnet build --configuration Debug     # Fast compile, slower run (default)
```

**Debug vs Release:**
- **Debug**: Includes symbols for debugging; JIT compilation less aggressive
- **Release**: Removes debug info; RyuJIT optimizes aggressively; ~2-3x faster

### dotnet run

Build (if needed) and execute your program.

```bash
$ dotnet run
MyApp

Hello, World!
```

**With arguments:**
```bash
$ dotnet run -- arg1 arg2
# "--" separates dotnet arguments from program arguments
# Program receives: ["arg1", "arg2"]
```

**In code:**
```csharp
var args = args;  // Program.cs receives command-line arguments
foreach (var arg in args)
    Console.WriteLine(arg);
```

### dotnet add package

Install a NuGet package.

```bash
$ dotnet add package Newtonsoft.Json
  Determining projects to restore...
  Writing /home/user/MyApp/MyApp.csproj
  Restoring MyApp/MyApp.csproj...
  Writing MSBuild props files...
  Restoring packages...
Successfully added reference to package 'Newtonsoft.Json' version '13.0.3'
```

**Edits `.csproj`:**
```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

**With specific version:**
```bash
dotnet add package Newtonsoft.Json --version 13.0.1
dotnet add package Newtonsoft.Json --prerelease  # Beta, alpha, rc
```

### dotnet remove package

Remove a dependency.

```bash
$ dotnet remove package Newtonsoft.Json
Removing reference 'Newtonsoft.Json' from project '/home/user/MyApp/MyApp.csproj'
Successfully removed package
```

### dotnet list package

Show all your dependencies.

```bash
$ dotnet list package
Project 'MyApp' has the following package references
   [net8.0]:
   Top-level Package      Requested   Resolved
   > Newtonsoft.Json      13.0.3      13.0.3
   > Serilog              2.10.0      2.10.0
```

**Show transitive dependencies:**
```bash
dotnet list package --include-transitive
```

### dotnet publish

Package for deployment.

```bash
$ dotnet publish -c Release -o ./publish
Microsoft (R) Build Engine version 17.x.x
Building 'MyApp' for Release...
MyApp -> /home/user/MyApp/bin/Release/net8.0/MyApp.dll
MyApp -> /home/user/MyApp/publish
```

**Creates a `publish` folder with:**
- `.dll` (compiled code)
- `.exe` (on Windows)
- Dependencies
- All files needed to run on another machine

**For deployment:**
```bash
dotnet publish -c Release --self-contained
# Includes the .NET runtime; target machine doesn't need .NET installed
```

---

## Workflow: Create, Build, Run

### End-to-End Example

```bash
# 1. Create project
$ dotnet new console --name CliApp
Created new C# project in CliApp/

# 2. Enter directory
$ cd CliApp

# 3. Add a dependency
$ dotnet add package Newtonsoft.Json

# 4. Build
$ dotnet build
Build succeeded.

# 5. Run
$ dotnet run
Hello, World!

# 6. Publish for distribution
$ dotnet publish -c Release -o ./bin/Release/publish
```

---

## Advanced Commands

### dotnet clean

Remove build artifacts (obj/, bin/).

```bash
$ dotnet clean
Cleaning CliApp/
```

**Use case**: Fix weird build issues by starting fresh.

### dotnet test

Run tests in xUnit, NUnit, or MSTest projects.

```bash
$ dotnet test
Build started...
MyTests (net8.0) - 0 errors, 0 warnings
  MyTests.Tests.UnitTest1 PASSED
  MyTests.Tests.UnitTest2 FAILED

Tests:
  Passed: 1, Failed: 1
```

### dotnet watch

Auto-rebuild and re-run on file changes.

```bash
$ dotnet watch run
watch: project file change detected

MyApp

# Edit Program.cs, save
# Automatically rebuilds and re-runs
```

**Huge productivity win** during development.

### dotnet workload install

Install platform-specific SDKs (iOS, Android, MAUI).

```bash
dotnet workload install ios     # For iOS development
dotnet workload install android # For Android development
```

**Not needed for backend/console work.**

---

## Common Gotchas

### Error: "Project file does not exist"

```bash
$ dotnet build
error: No project found in current directory
```

**Fix**: Navigate to the directory containing `.csproj`.

```bash
$ cd MyApp
$ dotnet build
```

### Error: "Dependency conflict"

```bash
error NU1107: Version conflict. LibA requires Newtonsoft.Json >= 13.0, 
                                  LibB requires Newtonsoft.Json 12.0
```

**Fix**: Use `dotnet list package --vulnerable` to inspect, then remove/update one library.

### Build succeeds but run fails

```bash
$ dotnet build
Build succeeded.

$ dotnet run
Unhandled exception: System.IO.FileNotFoundException
```

This usually means a **runtime dependency** (database connection, file path) is missing. Not a build issue.

---

## The Build Output Directory

After `dotnet build`, your compiled code is in:

```
MyApp/
├─ bin/
│  ├─ Debug/
│  │  └─ net8.0/
│  │     ├─ MyApp.dll        (your compiled code)
│  │     ├─ MyApp.exe        (Windows executable)
│  │     ├─ MyApp.pdb        (debug symbols)
│  │     └─ *.deps.json      (dependency manifest)
│  └─ Release/
│     └─ net8.0/
│        └─ (optimized binaries)
├─ obj/
│  └─ (build intermediate files, can delete)
└─ MyApp.csproj
```

**Never manually edit files in `bin/` or `obj/`.** They're generated. Run `dotnet clean` if needed.

---

## dotnet CLI vs Rider / Visual Studio

| Task | dotnet CLI | Rider / VS |
|------|-----------|-----------|
| Create project | `dotnet new console` | File → New Project |
| Add package | `dotnet add package Foo` | NuGet Package Manager UI |
| Build | `dotnet build` | Build menu or Ctrl+Shift+B |
| Run | `dotnet run` | Run menu or Shift+F10 |
| Debug | `dotnet run` (basic) | Debug menu (breakpoints, watch) |
| Test | `dotnet test` | Test Explorer UI |

**Rider/VS are conveniences.** The CLI is the **source of truth.** Every IDE action translates to a CLI command.

---

## dotnet Global Tools

Install CLI tools globally (system-wide).

```bash
# Install a global tool
$ dotnet tool install --global dotnet-format

# Run it
$ dotnet format

# Uninstall
$ dotnet tool uninstall --global dotnet-format
```

**Useful global tools:**
- `dotnet-format` – Auto-format code
- `dotnet-ef` – Entity Framework Core CLI
- `dotnet-api-explorer` – Explore APIs

**Not critical for now,** but good to know.

---

## Proxy & Package Sources

If behind a corporate proxy:

```bash
# Add custom NuGet source
$ dotnet nuget add source https://company-repo.com/nuget/v3/ -n company-nuget

# Remove NuGet source
$ dotnet nuget remove source nuget.org

# List sources
$ dotnet nuget list source
```

Configuration stored in `~/.nuget/NuGet/NuGet.Config`.

---

## Summary: dotnet CLI Commands

| Command | Purpose |
|---------|---------|
| `dotnet new` | Create project from template |
| `dotnet restore` | Download dependencies |
| `dotnet build` | Compile to IL |
| `dotnet run` | Build + execute |
| `dotnet add package` | Install NuGet package |
| `dotnet remove package` | Uninstall package |
| `dotnet list package` | Show dependencies |
| `dotnet clean` | Remove build artifacts |
| `dotnet test` | Run unit tests |
| `dotnet publish` | Package for deployment |
| `dotnet watch run` | Auto-rebuild on changes |

**Most common workflow:**
```bash
dotnet new console -n MyApp
cd MyApp
dotnet add package Newtonsoft.Json
dotnet build
dotnet run
```

---

## Kotlin/Gradle Comparison

| Task | Gradle (Kotlin) | dotnet CLI |
|------|-----------------|-----------|
| Create project | `gradle init` | `dotnet new console` |
| Add dependency | Edit `build.gradle.kts` + `gradle assemble` | `dotnet add package` |
| Build | `./gradlew build` | `dotnet build` |
| Run | `./gradlew run` | `dotnet run` |
| Test | `./gradlew test` | `dotnet test` |
| Package | `./gradlew build` → JAR in `build/` | `dotnet publish` → DLL in `publish/` |
| List dependencies | `gradle dependencies` | `dotnet list package` |

**dotnet CLI is more straightforward** because Gradle's task-based system adds complexity.

---

## Key Takeaway

The `dotnet` CLI is your primary tool. Learn it well:

1. **Create** with `dotnet new`
2. **Add dependencies** with `dotnet add package`
3. **Build** with `dotnet build`
4. **Run** with `dotnet run`
5. **Deploy** with `dotnet publish`

Everything else (Rider, VS) is UI sugar on top. The CLI is platform-independent and works everywhere.
