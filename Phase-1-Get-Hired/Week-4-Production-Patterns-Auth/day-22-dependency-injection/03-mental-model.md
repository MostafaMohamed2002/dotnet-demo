# MENTAL MODEL CHECK

## The Analogy: The Catering Company
Imagine you are organizing a corporate event.

- **The DI Container**: The catering manager. You don't go out and cook the food yourself; you tell the manager what you need on the menu.
- **Constructor Injection**: Your request to the manager: "I need a Chef, a Server, and a Sommelier." You don't care *how* the manager finds them, you just expect them to be there when you start the party.
- **Service Lifetimes**:
    - **Transient**: A "Paper Plate." You get a new one every time you need to put something on it.
    - **Scoped**: The "Tablecloth." One is laid out for the entire duration of the dinner party (the HTTP request). Everyone at that table shares the same cloth.
    - **Singleton**: The "Kitchen Stove." There is only one stove in the entire building. Every party, every guest, and every staff member uses the same stove for the entire night.
- **Captive Dependency**: Imagine the "Tablecloth" (Scoped) being accidentally glued to the "Stove" (Singleton). Now, the tablecloth from the first party of the night is still stuck to the stove when the second party arrives. That's a "captive" dependency—something that should have been temporary is now permanent.

## Where the Analogy Breaks Down
In C#, the DI container doesn't just "provide" the object; it manages the **disposal**. When an HTTP request ends, the container automatically calls `.Dispose()` on all `Scoped` objects. The "Catering Manager" doesn't just provide the staff; he also makes sure they go home at the end of the event.
