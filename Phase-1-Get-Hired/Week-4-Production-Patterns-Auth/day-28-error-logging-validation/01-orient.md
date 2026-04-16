# ORIENT

## Why This Topic Exists
In a "Happy Path" development cycle, you assume the user provides valid data and the server never fails. In production, the "Unhappy Path" is where 80% of your time is spent. 

If a production server crashes and returns a raw stack trace to the client, you've committed two sins:
1. **Security Breach**: You've leaked your internal folder structure, library versions, and variable names to potential attackers.
2. **Observability Gap**: If you don't have structured logs, you're essentially "blind" when a bug happens in production. You can't tell if a `500 Internal Server Error` was caused by a database timeout or a null reference in the service layer.

## Android/Kotlin Parallels
In Android, you likely used `try-catch` blocks or `Result` wrappers around your API calls and Room queries. You might have used **Timber** or **Logcat** for logging.
- **Structured Logging $\approx$ Logcat with Tags**: Instead of just printing a string, you provide tags. In .NET, we take this further by providing "Properties" that log aggregators (like ELK or Azure Application Insights) can index.
- **Global Error Handling $\approx$ `Thread.UncaughtExceptionHandler`**: Instead of putting a `try-catch` in every single ViewModel, you have a top-level handler that catches everything that slips through.

## The Bigger Picture
Professional APIs follow the "Fail Fast, Fail Gracefully" principle. We use **FluentValidation** to stop bad data at the door, **Global Middleware** to catch unexpected crashes, and **Correlation IDs** to stitch together the fragmented story of a single request across thousands of log lines.
