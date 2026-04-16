# MENTAL MODEL CHECK

## The Analogy: The Airport Control Tower
Imagine an airport.

- **FluentValidation**: The security checkpoint. If your ticket is expired or your bags are overweight, you are stopped *before* you even get to the gate. The "Airport" (the API) doesn't waste resources processing a passenger who doesn't meet the requirements.
- **Global Exception Handling**: The "Emergency Landing" protocol. If a plane (a request) has a mechanical failure mid-flight, the control tower doesn't just let it crash into the terminal. They guide it to a safe landing strip and notify the ground crew (the logs) immediately.
- **Structured Logging**: The Black Box. It doesn't just record a stream of noise; it records specific data points (Altitude: 30k, Speed: 500kts) in a structured format that investigators can analyze later.
- **Correlation ID**: The Flight Number. If "Flight 123" has an issue, investigators don't look for "any plane that was in the air"; they look for every single log entry associated with "Flight 123" across radar, radio, and maintenance logs.

## Where the Analogy Breaks Down
In an airport, a security failure is a human event. In an API, a validation failure is a "soft" failure—the system is working exactly as intended. However, a "Global Exception" is a "hard" failure—something happened that the developer never anticipated.
