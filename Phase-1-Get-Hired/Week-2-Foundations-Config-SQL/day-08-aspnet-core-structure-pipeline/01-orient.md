# ORIENT

## What is this and why does it exist?
ASP.NET Core is the cross-platform, high-performance framework for building modern, cloud-enabled, Internet-connected apps. At its heart, it is a **request-response engine**. It exists to abstract the complexities of HTTP, providing a structured way to handle routing, security, and data processing before a request ever reaches your business logic.

## Android/Kotlin Parallels
If you've used **Ktor** or **Spring Boot** (or even handled Intent routing in Android), the concepts are similar:
- **Middleware $\approx$ Interceptors**: In Android, you might use an OkHttp Interceptor to add a header to every request. In ASP.NET Core, Middleware does this for *incoming* requests.
- **Dependency Injection**: Hilt/Dagger in Android is conceptually identical to the built-in .NET DI container. You register a service once and "inject" it into your Controller's constructor.
- **Routing**: Think of the ASP.NET Route as the `AndroidManifest.xml` for your API endpoints—it defines which "Activity" (Controller Action) handles which "Intent" (URL path).

## The Bigger Picture
This is the "skeleton" of your backend. Before you can save a patient to a database or authenticate a doctor, you must understand how a raw TCP stream from a client becomes a `HttpRequest` object, passes through a series of filters, and eventually triggers a specific method in your code.
