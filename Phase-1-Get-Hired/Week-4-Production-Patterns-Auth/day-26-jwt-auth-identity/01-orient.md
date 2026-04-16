# ORIENT

## Why This Topic Exists
Until now, your API has been "Open." Anyone who knows the URL can modify or delete data. In a production system, you must verify **who** the user is (Authentication) and **what** they are allowed to do (Authorization).

For modern APIs, the industry standard is **JWT (JSON Web Tokens)**. Unlike traditional sessions where the server remembers the user in its memory, JWTs are **stateless**. The server signs a token and gives it to the client; the client sends it back with every request. The server simply verifies the digital signature to trust the user.

## Android/Kotlin Parallels
In Android, you've likely implemented a "Login" flow where you receive a token from a server and store it in `EncryptedSharedPreferences` or a `DataStore`. You then added that token to every Retrofit request using an `Interceptor`.
- **The Server-side**: Today, you are building the part that *generates* that token and *verifies* it on the other end.
- **ASP.NET Identity**: Think of this as the "Account Management SDK." It handles the boring and dangerous parts (password hashing, user lookups, role assignments) so you don't have to write a `Passwords` table manually.

## The Bigger Picture
Authentication is the most sensitive part of any system. A single mistake (like storing passwords in plaintext or using a weak signing key) can lead to a total system compromise. We will use **ASP.NET Identity Core** for user management and **JWT Bearer** for the communication layer, ensuring a stateless, scalable, and secure architecture.
