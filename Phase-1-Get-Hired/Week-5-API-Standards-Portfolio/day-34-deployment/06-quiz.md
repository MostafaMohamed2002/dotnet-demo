# QUIZ
Answer all 5 questions. Paste your answers into the chat when ready.

**1. Conceptual**: Why is it dangerous to store a production connection string in `appsettings.json` and commit it to a public GitHub repository?

**2. Code-based**: If you have a variable in `appsettings.json` under `ConnectionStrings:DefaultConnection`, what is the exact name of the Environment Variable you should create in Railway or Azure to override it?

**3. Conceptual**: Your app works perfectly on your machine but returns a `500 Error` on Railway. You check the logs and see "A network-related or instance-specific error occurred while establishing a connection to SQL Server." What is the most likely cause?

**4. Scenario-based**: You want to allow your QA team to use Swagger in the "Staging" environment but keep it disabled in "Production". How would you modify the `if (app.Environment.IsDevelopment())` block in `Program.cs` to achieve this?

**5. Conceptual**: What is the purpose of "SSL Termination" provided by cloud platforms, and why does it mean your .NET code usually doesn't need to manage SSL certificates itself?
