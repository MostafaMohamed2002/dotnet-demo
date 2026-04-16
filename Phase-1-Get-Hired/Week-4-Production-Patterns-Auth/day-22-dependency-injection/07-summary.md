# Summary — Day 22-23: Dependency Injection

## What You Learned

- **The DI Container**: Understanding how the framework builds the object graph and removes the need for the `new` keyword.
- **Constructor Injection**: The professional standard for providing dependencies to classes.
- **Service Lifetimes**:
    - `Transient`: New instance every time.
    - `Scoped`: One instance per HTTP request (Perfect for DB work).
    - `Singleton`: One instance for the app lifetime.
- **Interface-Based Design**: Decoupling "What" (Interface) from "How" (Implementation).
- **Captive Dependencies**: Recognizing the danger of injecting a short-lived service into a long-lived one.

## What This Unlocks

You have moved from "Writing Code" to "Designing Systems." By decoupling your components, your application is now testable, maintainable, and scalable. You can now implement complex patterns like the Repository and Service layers without creating a tangled web of dependencies.

## Revisit Before Next Session

- Ensure you can explain the la difference between `AddScoped` and `AddSingleton`.
- Practice the "GUID Test" to visually confirm how lifetimes work in your local environment.
