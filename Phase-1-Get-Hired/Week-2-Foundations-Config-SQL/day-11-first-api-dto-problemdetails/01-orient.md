# ORIENT

## What is this and why does it exist?
In the previous sessions, we looked at the "plumbing" (pipeline, config). Now, we build the "furniture"—the actual API endpoints. The goal of a professional API isn't just to return data, but to do so with a **predictable contract**.

This session introduces the "Buffer Layer" (DTOs) and the "Standardized Error" (ProblemDetails). Without these, your API is fragile: changing a database column would break every mobile app that consumes your API, and your error messages would be inconsistent.

## Android/Kotlin Parallels
If you've used **Retrofit**, you've already dealt with DTOs. You don't use the raw JSON response as your domain model throughout the app; you map the API response to a data class. In .NET, we do this on the server side. We map the **Database Entity** $\rightarrow$ **DTO** $\rightarrow$ **JSON**.

## The Bigger Picture
This is where you move from "making it work" to "making it maintainable." By separating the data you *store* (Entity) from the data you *expose* (DTO), you gain total control over your API versioning and security.
