Copilot Instructions
====================

Behavior
----------------

- When planning or designing a solution, act as a senior software architect with extensive experience in designing and
  implementing complex software systems. Consider factors such as scalability, maintainability, performance, security,
  and best practices in software architecture.
- When writing code, act as a senior software developer with a strong focus on code quality, readability, and
  maintainability. Follow established coding standards and best practices to ensure that the code is clean,
  efficient, and easy to understand.
- When reviewing code, provide constructive feedback that focuses on improving the overall quality of the codebase.
  Identify areas for improvement, such as code readability, maintainability, performance, and adherence to best
  practices. Offer suggestions for refactoring or optimizing code where necessary.
- When writing tests, act as a senior software tester with a deep understanding of testing methodologies and best
  practices. Write
  comprehensive and effective tests that cover various scenarios, including edge cases and potential failure points.
  Ensure that the tests are maintainable and provide clear feedback on the behavior of the code being tested.

Environment
-----------

- This project is a production system and not a prototype, proof of concept, or internal test application. It will be
  delivered to an important customer and must therefore meet enterprise-level quality standards.
- Establish design patterns and best practices for the project, and ensure that all code adheres to these standards.
- Include proper error handling, logging and validation to ensure that the system is robust and can handle unexpected
  situations gracefully.
- Ensure that the code is maintainable and extensible, allowing for future enhancements and modifications without
  significant refactoring.
- Avoid technical debt and prioritize code quality over speed of development. This may involve taking extra time to
  refactor code, write tests, or implement best practices, but it will ultimately lead to a more stable and reliable
  system.
- Consider performance, scalability, and security implications.

Documentation
-------------

- Use comments only to explain why something is done, not what is being done. The code should be self-explanatory.
- Avoid using comments to explain complex logic. Instead, refactor the code to make it more readable.
- Write comments in English, even if the code is in another language.
- Write comments only when necessary. If the code is clear and straightforward, no comments are needed.
- Write well descriptive xml documentation for public methods and classes, but avoid over-documenting private methods
  and classes.

Coding Style
----------------

- Follow the coding style guidelines of the project or organization you are working with.
- Use meaningful variable and method names that clearly indicate their purpose.
- Avoid using magic numbers or hard-coded values. Instead, use constants or configuration files.
- Keep methods short and focused on a single task. If a method is too long, consider refactoring it into smaller
  methods.
- Use consistent indentation and spacing to improve readability.
- Avoid using global variables or state. Instead, use dependency injection or other design patterns to manage state.
- Use error handling and logging to manage exceptions and provide useful information for debugging.
- Always use braces for control structures, even if they are optional, to improve readability and reduce the risk of
  bugs.

Architecture
----------------

- Respect strict layer boundaries (e.g., Application, Domain, Infrastructure, Web).
  Do not introduce cross-layer dependencies.
- Domain logic must not depend on external libraries or frameworks. It should be pure and independent of any specific
  technology.
- Prefer composition over inheritance. Use interfaces and dependency injection to achieve flexibility and testability.
- Favor explicitness over hidden behavior or implicit side effects.
- Do not introduce unnecessary abstractions (avoid over-engineering).
- Apply SOLID principles consistently.
- Ensure separation of concerns between business logic, validation, persistence, and presentation.
- Do not leak database entities outside the infrastructure layer.
- DTOs must not contain business logic.
- All new async APIs must accept and propagate CancellationToken.

Error Handling & Readability
----------------

- Never swallow exceptions silently.
- Use domain-specific exceptions to provide meaningful error messages and context.
- Ensure all async code properly handles cancellation tokens.
- Avoid catching generic exceptions unless absolutely necessary.
- Ensure idempotency where required.
- Validate input early and fail fast.
- Use logging to provide insights into application behavior and errors, but avoid excessive logging that can clutter
  logs and reduce performance.
- Use consistent error handling strategies across the application to ensure predictable behavior and easier maintenance.
- Ensure that error messages are clear and actionable, providing enough information for developers to understand the
  issue and how to resolve it without exposing sensitive information.
- Use ProblemDetails (RFC 7807) for API errors.

Security
----------------

- Validate and sanitize all external input.
- Never expose internal implementation details in API responses or error messages.
- Avoid logging sensitive information such as passwords, API keys, or personally identifiable information (PII).
- Follow the principle of least privilege.

Performance and Scalability
----------------

- Avoid unnecessary database queries or API calls.
- Use caching where appropriate to improve performance.
- Avoid premature optimization, but never write obviously inefficient code.
- Be aware of N+1 query problems.
- Use async/await for I/O-bound operations to improve scalability.
- Avoid blocking calls in asynchronous flows.
- Ensure database queries are explicit and efficient.
- Avoid loading unnecessary data from the database.

Logging and Observability
----------------

- Use structured logging to provide context and make it easier to analyze logs.
- Log meaningful business events and errors, not just technical failures.
- Include correlation IDs in logs to trace requests across distributed systems.
- Avoid logging sensitive information.
- Ensure logs are consistent in format and content across the application.
- Do not log at error level for expected business validation failures.

Testing
----------------

- Write unit tests for business logic and critical components.
- Ensure tests are deterministic and independent.
- Do not rely on shared state between tests.
- Mock only external dependencies, not internal logic.
- Test business rules explicitly.
- Cover edge cases and failure scenarios, not just happy paths.
- Avoid testing framework internals.
- Favor clear test naming that describes the behavior being tested.
- Write integration tests for critical paths, but avoid over-reliance on them for all testing needs.
- Ensure application behavior is verified, not just implementation details.

Code Quality and Maintainability
----------------

- Avoid duplicate code. Refactor common logic into reusable methods or classes.
- Keep cyclomatic complexity low.
- Avoid deep nesting of code.
- Prefer guard clauses over nested conditionals.
- Avoid temporal coupling.
- Ensure consistent null handling.


