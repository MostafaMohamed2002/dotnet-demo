# MENTAL MODEL CHECK

## The Analogy: The Digital Passport
Think of the **JWT** as a **Passport**.

When you go through customs (an API request), the officer (the server) doesn't call your home country to verify who you are every single time. Instead, they look at the passport. If the passport has the official government seal (the **Signature**) and hasn't expired (the **Expiration Claim**), the officer trusts the information written inside (the **Claims/Roles**).

## Where the Analogy Breaks Down
A physical passport is hard to "revoke" instantly—you'd have to put the person on a blacklist. Similarly, **JWTs are stateless**. Once a token is issued, it is valid until it expires. You cannot "delete" a JWT from the server because the server isn't storing it. To implement an immediate logout or ban, you would need a "Token Blacklist" in Redis, which introduces state back into the system, breaking the pure stateless nature of JWTs.
