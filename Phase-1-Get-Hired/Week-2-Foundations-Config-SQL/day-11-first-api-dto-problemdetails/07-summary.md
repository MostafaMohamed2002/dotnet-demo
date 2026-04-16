# Summary — Day 11: First Real API, DTOs & ProblemDetails

## What You Learned
- The **Entity vs DTO** separation: protecting the internal data model from the external API contract.
- The **Triple-DTO** strategy: separate models for Creating, Updating, and Responding.
- **RFC 7807 (ProblemDetails)**: How to return standardized, machine-readable error responses.
- **Model Binding**: The exact usage of `[FromBody]`, `[FromRoute]`, and `[FromQuery]`.
- The danger of **Over-posting** and how DTOs act as a security filter.

## What This Unlocks
You can now design APIs that are secure, professional, and easy for frontend developers to consume. You've moved from "writing code" to "designing contracts."

## Revisit Before Next Session
- Practice the mapping logic (Entity $\rightarrow$ DTO).
- Experiment with `Problem()` and see how the JSON output differs from a simple `BadRequest()`.
