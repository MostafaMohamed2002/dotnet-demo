# MENTAL MODEL CHECK

## The Analogy: The Personal Assistant (Change Tracker)
Imagine you have a personal assistant (the `DbContext`).

- **Standard Query**: You ask the assistant to get a file from the archive. The assistant gets the file and **keeps a copy of it on their desk**, constantly watching to see if you scribble any notes on it. If you do, the assistant remembers exactly what you changed and prepares a "summary of edits" to send back to the archive later (`SaveChangesAsync`).
- **`AsNoTracking()`**: You tell the assistant: "Just bring me the file, read it, and then throw the copy away." The assistant doesn't keep anything on the desk. They are much faster because they aren't spending energy monitoring your edits.

## Where the Analogy Breaks Down
In the real world, the "desk" (Change Tracker) isn't just about memory; it's about **Identity Map**. If you request the same Doctor object twice in the same session without `AsNoTracking()`, EF Core will give you the *exact same object reference* from the desk instead of hitting the database again. `AsNoTracking()` will always hit the database (or cache) and create a new object instance.
