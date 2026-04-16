# REAL-WORLD CONTEXT

## Production Implementation
In a real-world Clinic API, we would never store passwords in plain text. We use `PasswordHasher<T>`, which applies a salted PBKDF2 hash. This ensures that even if the database is leaked, the passwords remain secure.

## Case Study: Role Segregation
In our booking system, the roles are strictly partitioned:
- **Admin**: Can create/edit `Doctor` profiles. They ensure the clinic has the right staff.
- **Doctor**: Can view their own `Appointment` list but cannot change their own specialization (that's an Admin task).
- **Patient**: Can create `Appointments` but cannot see other patients' data.

## The Junior Gotcha: "The Over-Privileged Token"
A common mistake is adding too much information into the JWT payload (e.g., adding the patient's full medical history to the token claims). 
**Why this fails:** 
1. **Token Size**: JWTs are sent in every HTTP header. Large tokens increase latency and can hit header size limits in Nginx/IIS.
2. **Security**: The payload is only Base64 encoded, **not encrypted**. Anyone with the token can read the claims. Never put sensitive PII (Personally Identifiable Information) in a JWT. Use the `UserId` claim to fetch data from the DB.
