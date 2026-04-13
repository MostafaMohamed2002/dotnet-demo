# Day 6: First Real Code – HTTP Client & LINQ Apps – Quiz

## 5 Questions: CLR, HttpClient, LINQ

Answer these to self-assess understanding. Answers and explanations below.

---

## Question 1: GC Pressure

**You're writing a backend API that processes 10,000 requests/second. Each request handler creates a new `Appointment` object and 5 intermediate arrays via LINQ chaining.**

Which statement is true about garbage collection?

A) Gen0 collections will be frequent but fast  
B) Gen2 collections will happen constantly  
C) The GC will pause every request  
D) You should manually call `GC.Collect()`

**Your answer**: ___

---

## Question 2: HttpClient Best Practice

**Which code pattern is correct for production use?**

```csharp
// Option A
for (int i = 0; i < 1000; i++)
{
    using var client = new HttpClient();
    var response = await client.GetAsync("https://api.example.com/data");
}

// Option B
private static readonly HttpClient _client = new HttpClient();
public async Task FetchDataAsync() 
{
    var response = await _client.GetAsync("https://api.example.com/data");
}

// Option C
public HttpClient GetHttpClient() => new HttpClient();
```

**Your answer**: ___

---

## Question 3: LINQ Deferred Execution

**What is printed?**

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

var query = numbers
    .Where(n => 
    {
        Console.WriteLine($"Checking {n}");
        return n > 2;
    });

Console.WriteLine("Query defined");

var result = query.ToList();
```

A) `Query defined` is printed first, then `Checking 1` through `Checking 5`  
B) `Checking 1` through `Checking 5` is printed, then `Query defined`  
C) Only `Query defined` is printed (query never runs)  
D) Each number is checked twice

**Your answer**: ___

---

## Question 4: IEnumerable vs IQueryable

**Which is true?**

A) `IEnumerable<T>` always executes in-memory; `IQueryable<T>` may execute in database  
B) Both execute in-memory  
C) `IEnumerable<T>` is faster than `IQueryable<T>`  
D) `IQueryable<T>` only works with Entity Framework

**Your answer**: ___

---

## Question 5: CancellationToken

**What happens when you run this code and the timeout expires?**

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

try
{
    var data = await FetchLargeDataAsync(cts.Token);
    Console.WriteLine("Success");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.GetType().Name}");
}

async Task<string> FetchLargeDataAsync(CancellationToken cancellationToken)
{
    await Task.Delay(5000, cancellationToken);  // 5 second delay
    return "data";
}
```

A) Prints `Success` (timeout is ignored)  
B) Prints `Error: HttpRequestException`  
C) Prints `Error: OperationCanceledException`  
D) Prints nothing (deadlock)

**Your answer**: ___

---

## Answers

### Question 1: GC Pressure

**Correct Answer: A**

**Explanation**:
- With 10,000 requests/second, each creating intermediate objects, Gen0 fills quickly
- Gen0 collection is **frequent but fast** (microseconds) because Gen0 is small
- Gen2 collections are rare (only if Gen0 → Gen1 → Gen2 promotion happens repeatedly)
- Modern CLR doesn't pause every request; Gen0 collections are brief
- Never call `GC.Collect()` manually (it's expensive and unpredictable)

**Key takeaway**: High allocation rate = frequent Gen0 collections. This is expected and acceptable if objects are short-lived.

---

### Question 2: HttpClient Best Practice

**Correct Answer: B**

**Explanation**:

**Option A (❌ Wrong)**:
- Creates new HttpClient 1,000 times
- Each new instance opens a TCP socket
- Socket exhaustion after ~1,000 requests
- Causes `SocketException` in production

**Option B (✅ Correct)**:
- Single static HttpClient instance reused
- Shares connection pool across requests
- Efficient socket reuse
- This is the recommended pattern

**Option C (❌ Wrong)**:
- Creates new client each call (like Option A)
- Also wrong

**Key takeaway**: `HttpClient` is designed to be reused. Use a single static instance or `IHttpClientFactory` in dependency injection.

---

### Question 3: LINQ Deferred Execution

**Correct Answer: A**

**Explanation**:
```
Query defined       ← Prints immediately (query not executed)

Query defined
Checking 1          ← Where runs when ToList() is called
Checking 2
Checking 3
Checking 4
Checking 5
```

The `.Where()` method is lazy. It doesn't execute when defined, only when the result is enumerated (`.ToList()` causes enumeration).

**Key takeaway**: LINQ queries on `IEnumerable<T>` are deferred. Enumeration triggers execution.

---

### Question 4: IEnumerable vs IQueryable

**Correct Answer: A**

**Explanation**:

| Aspect | IEnumerable | IQueryable |
|--------|------------|-----------|
| Execution | In-memory LINQ to Objects | Typically translated to SQL |
| Provider | LINQ to Objects | Database provider (EF Core, etc.) |
| Speed | Fast (for small datasets) | Fast (filters at DB, returns less data) |

**Example**:
```csharp
var list = new List<Appointment> { ... };

// IEnumerable: LINQ runs in C#, filters in memory
var scheduled = list.Where(a => a.Status == Scheduled);

// IQueryable: LINQ translated to SQL, filters at DB
var dbScheduled = dbContext.Appointments
    .Where(a => a.Status == Scheduled);  // Executes: SELECT * FROM Appointments WHERE Status = 0
```

Both are "LINQ", but execution location differs.

---

### Question 5: CancellationToken

**Correct Answer: C**

**Explanation**:
1. `CancellationTokenSource` created with 1-second timeout
2. `FetchLargeDataAsync` tries to delay 5 seconds with the cancellation token
3. After 1 second, the token signals cancellation
4. `Task.Delay(cancellationToken)` is cancelled and throws `OperationCanceledException`
5. Catch block catches it and prints the exception type name

**Output**: `Error: OperationCanceledException`

**Key takeaway**: Pass `CancellationToken` through async methods. When the token is cancelled, `async` operations throw `OperationCanceledException`.

---

## Scoring

- **5/5**: Expert. Ready for Week 2 (EF Core, ASP.NET)
- **4/5**: Strong. Minor gaps; re-read one section
- **3/5**: Good. Review the weaker topics before Week 2
- **2/5 or below**: Review Days 1–6 before continuing

---

## Review Topics by Question

| Question | Topic | Review |
|----------|-------|--------|
| 1 | GC pressure, Gen0 collections | Day 5: 02-core-concepts.md, "Garbage Collection" |
| 2 | HttpClient best practices | Day 6: 02-core-concepts.md, "System.Net.Http" |
| 3 | LINQ deferred execution | Day 6: 02-core-concepts.md, "LINQ & Deferred Execution" |
| 4 | IEnumerable vs IQueryable | Day 6: 02-core-concepts.md or Day 4: 02-core-concepts.md |
| 5 | CancellationToken semantics | Day 6: 02-core-concepts.md, "CancellationToken" |

---

## Next: Day 6 Summary

Move to 07-summary.md for recap and bridge to Week 2.
