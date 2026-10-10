# GitDashboard

GitDashboard is a web application for analysing local Git repositories and public GitHub repositories. A React frontend displays repository statistics returned by an ASP.NET Core API.

The project is primarily a **practical learning and portfolio project**, focused on developing deeper experience with C#, .NET, ASP.NET Core, backend architecture, automated testing, and Test-Driven Development (TDD).

The application is developed incrementally, with tests used to drive implementation and design decisions.

Try the [hosted application](https://gitdashboard-f4mr.onrender.com/). The app is containerised with Docker and served on Render.

## Project Goals

The main goal is to use a real, evolving application to develop practical backend engineering skills.

Areas being explored include:

- C# and modern .NET
- ASP.NET Core Minimal APIs
- Dependency Injection
- Interfaces and abstractions
- Service-oriented application design
- Asynchronous programming and cancellation
- HTTP clients and external APIs
- Configuration
- Exception handling and API error responses
- Unit and integration testing with xUnit
- Test-Driven Development
- Designing code for testability
- Working with Git repositories
- Integrating with third-party APIs
- Deployment and hosting

The project is intentionally designed so that architectural decisions emerge from real requirements rather than attempting to design the entire system in advance.

## Current Status

The application currently supports analysing **local Git repositories** and **public GitHub repositories**. The React frontend is served alongside the API.

The API exposes:

```text
GET /api/analyse
```

### Local repository

```text
/api/analyse?path=/path/to/repository
```

### GitHub repository

```text
/api/analyse?owner=username&name=repository
```

The application retrieves all commits from the selected repository source (using pagination for GitHub) and calculates repository statistics.

Currently calculated statistics include:

- Total commits
- Commits by author
- Commits by day of the week
- Commits by date

## Architecture

Repository access is separated from repository analysis through a repository-source abstraction.

```text
HTTP Request
     │
     ▼
RepositoryAnalysisService
     │
     ▼
IRepositorySourceResolver
     │
     ▼
RepositorySourceResolver
     │
     ├── LocalRepositorySource
     │
     └── GitHubRepositorySource
     │
     ▼
List<Commit>
     │
     ▼
StatisticsService
     │
     ▼
RepositoryStats
     │
     ▼
HTTP Response
```

This separation allows additional repository providers to be introduced without coupling the analysis logic to a particular source.

Repository types are represented explicitly, currently including:

- `LocalRepositoryReference`
- `GitHubRepositoryReference`

The `RepositorySourceResolver` selects the appropriate `IRepositorySource` based on the repository reference.

## GitHub Integration

GitHub repositories are accessed through GitHub's REST API using an injected `HttpClient`.

The GitHub integration includes:

- Typed `HttpClient` configuration
- GitHub API request headers
- JSON response deserialization
- Commit mapping into the application's domain model
- Cancellation support
- Pagination through all commit pages
- Optional token authentication, configured with the `GitHub:Token` setting (for example, the `GitHub__Token` environment variable)
- Handling of missing repositories
- Handling of GitHub API failures and rate limits
- Appropriate API error responses

GitHub API failures are distinguished from invalid repositories. General upstream API failures return `502 Bad Gateway`; detected primary rate limits return `429 Too Many Requests` and include a `Retry-After` header when GitHub provides a reset time. Without a token, requests use GitHub's unauthenticated rate limit.

## Testing

Testing is a central part of the development process.

The project uses **xUnit** and contains both unit and integration tests.

Tests currently cover areas including:

- Git command execution and parsing
- Repository statistics
- Repository source selection
- Local repository access
- GitHub repository access
- HTTP response handling
- Cancellation behaviour
- API endpoint behaviour
- Dependency Injection configuration
- HTTP client configuration
- Integration with a real Git repository

External HTTP calls are isolated from unit tests using fake HTTP handlers, allowing success and failure scenarios to be tested deterministically without depending on the availability of GitHub.

The test suite includes unit and integration tests and runs in the GitHub Actions build workflow.

## Development Approach

The project follows a Test-Driven Development workflow:

```text
Define behaviour
      ↓
Write a failing test
      ↓
Implement the smallest change
      ↓
Make the test pass
      ↓
Run the full test suite
      ↓
Refactor where appropriate
      ↓
Repeat
```

This approach is being used not only to verify correctness, but also to help drive the architecture.

For example, support for GitHub repositories led to the introduction of:

- Repository reference types
- `IRepositorySource`
- Multiple repository sources
- `IRepositorySourceResolver`
- Typed HTTP clients
- Dedicated API exception handling

Each of these was introduced as the application's requirements evolved.

## AI-Assisted Development

This project is being developed with **AI-assisted technical mentoring**.

AI is being used as a development partner to:

- Explain .NET and C# concepts
- Explore alternative designs
- Help formulate tests
- Identify appropriate abstractions
- Review implementation decisions
- Guide incremental TDD cycles
- Challenge assumptions and explain trade-offs

The intention is not to generate the application wholesale, but to use AI as a learning aid while implementing and understanding the code incrementally.

A key objective of the project is therefore:

> **To understand the design and reasoning behind the code, rather than simply producing working code.**

The commit history reflects the incremental nature of the project and the progression of its architecture.

## Planned Development

The project is still actively evolving.

### Repository analysis

Future analysis features may include:

- Commit activity over time
- Contributor activity
- Commit frequency and trends
- Additional author statistics
- Repository activity summaries

### GitHub integration

Planned improvements include:

- More comprehensive handling of secondary and other GitHub rate limits
- Additional GitHub repository metadata
- More comprehensive GitHub API error handling

### API design

As the application grows, the API and application boundaries will be reviewed.

In particular, repository-reference parsing currently happens at the HTTP endpoint. This may eventually move into a dedicated application-layer component as the number of supported repository types grows.

### Testing

Testing will continue to evolve alongside the application, with an emphasis on:

- Focused unit tests
- Integration tests for application behaviour
- Testing external service boundaries without relying on live services
- Failure and cancellation scenarios
- Testing observable behaviour rather than implementation details
- Maintaining a fast and reliable test suite

## Learning Objectives

By the end of the project, I want to be able to demonstrate practical experience with:

- Building a .NET backend from the ground up
- Designing services and abstractions
- Applying dependency injection effectively
- Writing maintainable asynchronous C#
- Building APIs with ASP.NET Core
- Integrating external services
- Writing meaningful unit and integration tests
- Applying TDD to feature development
- Making architectural decisions based on evolving requirements
- Containerising and deploying a .NET application

The GitDashboard application is the vehicle for that learning; the broader objective is to develop a stronger understanding of **professional backend development and test-driven engineering practices**.
