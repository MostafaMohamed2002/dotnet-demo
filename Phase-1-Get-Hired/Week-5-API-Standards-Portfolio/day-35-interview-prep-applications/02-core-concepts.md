# CORE CONCEPT BREAKDOWN

## 1. The "Must-Know" Technical Drills
When answering these, use the **S-T-A-R** method (Situation, Task, Action, Result) and reference your Clinic project.

### Dependency Injection (DI) & Lifetimes
- **What**: DI is a design pattern that removes the hard dependency between a class and its dependencies.
- **Lifetimes**:
    - `Transient`: New instance every time it's requested. (Lightweight, stateless).
    - `Scoped`: One instance per HTTP request. (Perfect for `ApplicationDbContext`).
    - `Singleton`: One instance for the entire lifetime of the app. (Configuration, Caching).
- **Why**: Testability (Mocking) and Loose Coupling.

### IEnumerable vs. IQueryable
- `IEnumerable<T>`: The data is already in memory. Filtering happens via **LINQ-to-Objects** (client-side).
- `IQueryable<T>`: The query is a "blueprint." Filtering happens via **LINQ-to-SQL** (server-side). The SQL is only executed when you iterate (e.g., `.ToList()`).
- **Clinic Example**: Use `IQueryable` for the paginated Doctor list to avoid pulling 10,000 doctors into RAM just to show 10.

### Middleware Pipeline
- **Concept**: A sequence of components that handle requests and responses.
- **The Order**: `Exception Handling` $\rightarrow$ `HSTS/HTTPS` $\rightarrow$ `Routing` $\rightarrow$ `CORS` $\rightarrow$ `Authentication` $\rightarrow$ `Authorization` $\rightarrow$ `Endpoints`.
- **Crucial**: Authentication *must* come before Authorization.

### The N+1 Problem
- **What**: Occurs when you fetch a list of entities (1 query) and then loop through them to fetch a related entity for each one (N queries).
- **Fix**: **Eager Loading**. Use `.Include(x => x.Patient)` in EF Core to join the tables in a single SQL query.

### JWT End-to-End Flow
1. User sends credentials $\rightarrow$ `/login`.
2. Server validates $\rightarrow$ Creates claims $\rightarrow$ Signs with Secret Key $\rightarrow$ Returns JWT.
3. Client stores JWT $\rightarrow$ Sends in `Authorization: Bearer <token>` header.
4. Server intercepts via `JwtBearer` middleware $\rightarrow$ Verifies signature $\rightarrow$ Populates `User.Identity`.

## 2. SOLID in the Clinic Project
- **S (Single Responsibility)**: `AuthController` only handles identity; `AppointmentService` handles booking logic.
- **O (Open/Closed)**: Using Interfaces for repositories allows adding a `MongoDbRepository` without changing the `Service` logic.
- **L (Liskov Substitution)**: If you have a `BaseUser` class, `Doctor` and `Patient` should be interchangeable wherever a `BaseUser` is expected.
- **I (Interface Segregation)**: Creating `IDoctorActions` and `IPatientActions` instead of one giant `IUserService`.
- **D (Dependency Inversion)**: Controllers depend on `IAppointmentService` (abstraction), not `AppointmentService` (concrete implementation).
