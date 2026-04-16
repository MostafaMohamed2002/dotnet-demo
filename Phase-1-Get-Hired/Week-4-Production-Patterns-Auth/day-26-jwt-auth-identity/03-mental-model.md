# MENTAL MODEL CHECK

## The Analogy: The Digital Concert Ticket
Imagine you are attending a concert.

- **Registration/Login**: You go to the box office and show your ID. They verify you are who you say you are.
- **The JWT**: Instead of making you show your ID at every single security checkpoint inside the stadium, the box office gives you a **Wristband** (the JWT).
- **The Signature**: The wristband has a special holographic seal that is impossible to forge.
- **Claims**: The wristband has a color. Red = VIP (Admin), Blue = General Admission (Patient).
- **Verification**: At the entrance to the VIP lounge, the guard doesn't call the box office to ask who you are. He just looks at the wristband, sees the Red color, and verifies the holographic seal is authentic. If the seal is real, he lets you in.
- **Statelessness**: The guard doesn't need a "list of people currently in the building." The proof of access is carried by the guest.

## Where the Analogy Breaks Down
In the real world, once a wristband is given, you can't easily "take it back" if the person is banned from the concert. This is the **JWT Revocation Problem**. Because the server doesn't keep a session list, it can't "kill" a token. The only way to handle this is by making tokens very short-lived or using a "Blacklist" in Redis.
