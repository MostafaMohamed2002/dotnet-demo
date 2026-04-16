# QUIZ
Since this is a verbal drill session, this quiz is designed as a **Mock Interview**. 
**Instructions**: Read the question, set a timer for 60 seconds, and record yourself answering. Listen back and grade yourself.

**1. The Architecture Question**: "Walk me through your Clinic API. How does a request get from the client to the database, and what happens along the way?" (Looking for: Middleware, Controllers, Services, DbContext).

**2. The Efficiency Question**: "I see you used EF Core. How did you handle the N+1 problem when fetching appointments with patient details?" (Looking for: Eager Loading, `.Include()`).

**3. The Security Question**: "Why did you choose JWT over session-based cookies for your authentication system?" (Looking for: Statelessness, Scalability, Mobile-friendliness).

**4. The "Gotcha" Question**: "What happens if you inject a Scoped service into a Singleton service? Is this allowed? Why or why not?" (Looking for: Captive Dependency, Memory leaks/stale data).

**5. The SOLID Question**: "Give me a concrete example of where you applied the Dependency Inversion Principle in your project." (Looking for: Interfaces, `IServiceCollection` registration).
