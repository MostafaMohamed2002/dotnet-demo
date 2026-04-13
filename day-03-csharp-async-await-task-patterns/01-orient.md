# ORIENT — Async/Await and the Task Model in C#

## Why This Exists

In Kotlin, you use `suspend` functions and coroutine builders (`async`, `launch`) to write non-blocking, concurrent code. You work with `Deferred<T>` for results and cancellation is baked in. C# took a different path: **async/await and `Task<T>` are language and library features**, not a full coroutine runtime. The semantics are similar (suspension, continuation), but the mental model and mechanics differ significantly. Understanding `Task`, `ValueTask`, and `ConfigureAwait(false)` is essential for backend development—get this wrong and you'll deadlock or waste threads.

## Connection to Kotlin

You already know:
- **Kotlin `suspend fun`** → C# **`async Task<T>` method**
- **Kotlin `Deferred<T>`** → C# **`Task<T>` or `ValueTask<T>`**
- **Kotlin `async { ... }`** → C# **`async Task<T>` with `await` inside**
- **Kotlin coroutine cancellation** → C# **`CancellationToken`** (explicit, not automatic)

The critical shift: Kotlin's coroutine model is unified and scheduler-aware. C#'s async/await is language-level; execution depends on the runtime (ASP.NET Core, console app, WinForms). This means `ConfigureAwait(false)` exists to manage synchronization context—a concept that doesn't exist in Kotlin.

## Where This Fits

Every API endpoint you write will be `async Task<IActionResult>`. Every database query (`ToListAsync`, `FirstOrDefaultAsync`) returns a `Task`. Every background job is coordinated with `Task.WhenAll` or `Task.WhenAny`. Misunderstanding async/await—especially the difference between `Task` and `ValueTask`, or the danger of `async void`—will cost you hours of debugging production deadlocks or unobserved exceptions.

---

**Next:** Core concepts break down the Task model, async/await mechanics, and when to use ValueTask with real production examples.
