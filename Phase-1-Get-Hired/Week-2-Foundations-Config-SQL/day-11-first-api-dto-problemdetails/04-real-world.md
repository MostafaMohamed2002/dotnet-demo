# REAL-WORLD CONTEXT

## Production Implementation
In large-scale projects, manually mapping `Entity` $\rightarrow$ `DTO` (e.g., `new ProductDto { Name = entity.Name }`) becomes tedious. 

**The Industry Standard**: Use **AutoMapper** or **Mapperly**. These libraries allow you to define a mapping rule once: `CreateMap<Product, ProductResponseDto>()`, and then simply call `_mapper.Map<ProductResponseDto>(productEntity)`.

## The "Gotcha": Over-posting Attack
A common security vulnerability occurs when you use the **Entity** as the input parameter for a POST/PUT request.

**The Attack:**
If your `Product` entity has a property `IsFeatured` (which only admins should set), but you use `public IActionResult Update(Product product)`, a malicious user can send `{ "name": "Cheap Phone", "isFeatured": true }` in the JSON. Because of **Model Binding**, .NET will happily set `IsFeatured` to true in your database.

**The Fix**: Always use a specific `UpdateProductDto` that **only** contains the fields the user is allowed to change.

## Clinic API Case Study: The "Patient" Resource
In our clinic API, a `Patient` entity contains `MedicalHistory` and `InternalId`. 
- When a doctor requests a patient, we return `PatientResponseDto` (includes history).
- When a receptionist requests a patient, we return `PatientBasicDto` (only name and phone).
- Both are mapped from the same `Patient` entity.
