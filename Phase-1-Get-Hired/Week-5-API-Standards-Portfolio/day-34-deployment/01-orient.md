# ORIENT

## Why this exists
A backend that only runs on `localhost` is a prototype; a backend on a public URL is a product. Deployment is the process of moving your application from a controlled development environment to a production environment. This stage introduces "Environment Parity" challenges—ensuring that the code that worked on your machine behaves identically on a remote server.

## Android/Kotlin Parallels
In Android, "deployment" usually means uploading an APK or AAB to the Google Play Store. While the process is different, the core concept of **Build Variants** (debug vs. release) is the same. Just as you wouldn't ship a production app with `Log.d` everywhere or a hardcoded API key, you cannot ship a .NET API with a local SQL Server connection string or a development-only JWT secret.

## The Big Picture
Deployment is the final bridge. We are moving from "it works on my machine" to "it works for the user." We will focus on **Twelve-Factor App** principles, specifically regarding configuration: separating code from config using Environment Variables.
