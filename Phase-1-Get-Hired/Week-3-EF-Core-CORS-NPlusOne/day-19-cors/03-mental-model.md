# MENTAL MODEL CHECK

## The Analogy: The VIP Guest List
Imagine your API is an exclusive club.

- **The Browser**: The bouncer at the door.
- **The Origin**: The ID card the guest (the frontend) is showing.
- **The Preflight (`OPTIONS`)**: The bouncer doesn't let the guest in immediately. Instead, he calls the club manager (the server) and asks: "Hey, there's someone here from 'Localhost:3000'. Are they on the guest list?"
- **The Policy**: The manager's guest list. If `localhost:3000` is on the list, the manager says "Yes," and the bouncer lets them in.
- **The Middleware Order**: If the bouncer (CORS middleware) is standing *behind* the security check (Authorization middleware), the guest is rejected for not having a membership card before the bouncer even checks the guest list. The bouncer must be at the very front of the line.

## Where the Analogy Breaks Down
In a real API, the "Guest List" (CORS policy) doesn't actually stop a hacker from calling your API. A hacker can use `curl` or Postman to bypass the browser entirely. CORS is a **browser-side security feature** to protect users, not a **server-side security feature** to protect your data. For data protection, you use JWT and Authorization.
