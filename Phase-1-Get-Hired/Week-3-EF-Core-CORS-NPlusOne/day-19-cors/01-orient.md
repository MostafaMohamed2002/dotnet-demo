# ORIENT

## Why This Topic Exists
You can build a perfect API that works flawlessly in Postman, but the moment you connect it to a React or Angular frontend, everything breaks with a cryptic error: `Access-to- CORS-request has been blocked by CORS policy`. 

**CORS (Cross-Origin Resource Sharing)** is not a bug in your API; it is a security feature implemented by the **web browser**, not the server. It exists to prevent a malicious website from making requests to your API using a user's saved session cookies without their knowledge.

## Android/Kotlin Parallels
In native Android apps, you don't deal with CORS. When you use `Retrofit` or `Ktor` to call an API, the request comes from an app, not a browser. Native apps are trusted by the OS and can make requests to any domain. 

CORS only exists in the world of **Web Browsers**. If you are building a backend that will be consumed by a web-based frontend, CORS is the first "wall" you will hit.

## The Bigger Picture
Understanding CORS is about understanding the "Handshake" between the browser and the server. If you misconfigure the CORS middleware in `Program.cs`, your frontend team will be blocked, and you'll spend hours debugging a problem that isn't in your controllers, but in the ASP.NET Core middleware pipeline.
