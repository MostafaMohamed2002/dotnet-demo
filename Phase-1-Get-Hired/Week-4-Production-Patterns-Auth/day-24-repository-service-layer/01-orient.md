# ORIENT

## Why This Topic Exists
If you put your EF Core logic directly in your Controllers, you've created a "Fat Controller." Fat controllers are a nightmare to maintain: they mix HTTP concerns (routing, status codes) with database concerns (SQL queries, joins) and business concerns (validation, rules).

The **Repository Pattern** and **Service Layer** are architectural boundaries. They ensure that your API is composed of "Thin" layers that each do one thing:
1. **Controllers**: Handle the HTTP request, validate the input, and return a response.
2. **Services**: Orchestrate business rules (e.g., "Can a patient book this slot?").
3. **Repositories**: Abstract the database. The service doesn't know if the data comes from SQL Server, MongoDB, or an API.

## Android/Kotlin Parallels
This is exactly the **Clean Architecture** pattern you see in professional Android apps:
- **UI Layer (Controller)** $\rightarrow$ **Domain Layer (Service)** $\rightarrow$ **Data Layer (Repository)**.
In Android, the `ViewModel` acts as the service layer, coordinating between the UI and the `Repository`. In .NET, we separate the Service and Repository into two distinct classes to allow for better scaling and testability.

## The Bigger Picture
By separating these layers, you make your application **testable**. You can write a unit test for `AppointmentService` by providing a "Mock" repository that doesn't actually touch the database. If your business logic is trapped inside a controller, you can't test it without running a full web server.
