Read this file before every task.

# Project: StoronnimV.Server

## Git
- **GitHub Owner:** ilyaghrischenko.
- **GitHub Repo:** StoronnimV.
- **Main branch name:** main.

## Architecture & Patterns
- Clean Architecture solution split into `StoronnimV.Api`, `StoronnimV.Application`, `StoronnimV.Domain`, `StoronnimV.Infrastructure`, and `StoronnimV.Tests` projects.
- Dependency direction is `Api -> Application + Infrastructure`, `Application -> Domain`, and `Infrastructure -> Domain`; `Domain` has no external package references.
- Controller requests flow through controller services, entity/home/identity services, domain repository contracts, and infrastructure repository implementations.
- Read models use EF Core projections and AutoMapper response mappings; request validation uses FluentValidation validators.
- `StoronnimV.Api/Program.cs` is the composition root, with registrations grouped in `WebApplicationBuilderExtensions`.

## Stack
- **Runtime:** .NET 9, ASP.NET Core 9 controller-based Web API.
- **Data:** Entity Framework Core 9, Npgsql/PostgreSQL, EF Core migrations.
- **Background processing:** Hangfire 1.8 with Hangfire.PostgreSql.
- **Storage and media:** Azure Blob Storage SDK, SixLabors.ImageSharp.
- **Auth and security:** ASP.NET Core JWT Bearer authentication, ASP.NET Core Identity password hashing, CORS, fixed-window rate limiting.
- **Validation and mapping:** FluentValidation, AutoMapper.
- **API and observability:** OpenAPI, Swagger/Swashbuckle, Serilog, ASP.NET Core health checks, response compression.
- **Configuration:** ASP.NET Core options/configuration and DotNetEnv local `.env` fallback.
- **Testing:** xUnit, Microsoft.NET.Test.Sdk, coverlet collector; the test project currently contains no test source files or project references.
- **Tooling and deployment:** repository-local `dotnet-ef` 9.0.7 manifest and a multi-stage .NET 9 Linux Dockerfile.

## Static Code Analyzer

(none detected)

## Critical Coding Rules (MUST FOLLOW)
- Preserve the current project dependency direction: keep domain entities/contracts/projections in `Domain`, orchestration/DTOs/validators/mappings in `Application`, EF/Blob implementations in `Infrastructure`, and HTTP/composition code in `Api`.
- Use nullable reference types consistently and mark optional values with nullable annotations instead of suppressing nullability.
- Use constructor injection for controllers, services, repositories, and middleware; register new implementations in `WebApplicationBuilderExtensions` with the lifetime used by the matching dependency category.
- Name asynchronous methods with the `Async` suffix, accept `CancellationToken ct`, and pass that token through service, repository, EF Core, and Blob calls.
- Keep controllers thin: bind the HTTP request, delegate to the matching controller service, and return the resulting `ActionResult`.
- Put entity lookup, pagination, and mutation rules in application services; use the existing application exceptions for expected failures so `ExceptionMiddleware` remains the centralized HTTP exception mapper.
- Put database access behind domain repository contracts; use `AsNoTracking` plus projections for read-only queries and call `SaveChangesAsync(ct)` inside repository mutations.
- Map domain projections to response DTOs in controller services through AutoMapper profiles rather than mapping response shapes in controllers.
- Implement request validation as `AbstractValidator<TRequest>` classes under `StoronnimV.Application/Validation` and register them through `AddFluentValidation`.
- Read API startup requirements such as `DB_CLOUD`, JWT settings, and `CLIENT_URL` through `EnvironmentExtensions.GetEnvironmentVariableOrThrowException`; bind structured non-secret settings through ASP.NET Core options/configuration.

## Workspace Commands

Run these commands from the repository root.

- Restore: `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --disable-build-servers`
- Build solution: `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --disable-build-servers`
- Build startup project: `dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-restore --configuration Release --disable-build-servers`
- Run API after configuring the documented local PostgreSQL, Blob, JWT, cookie, and CORS environment: `dotnet run --project backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-launch-profile`
- Test: `dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln --configuration Release --disable-build-servers`
- Lint/analyze: (none detected)

## Project Learnings

**Accumulated corrections. This section is for the agent to maintain, not just the human.**

When the user corrects your approach, append a one-line rule here before ending the session. Write it concretely ("Always use X for Y"), never abstractly ("be careful with Y"). If an existing line already covers the correction, tighten it instead of adding a new one. Remove lines when the underlying issue goes away (model upgrades, refactors, process changes).

- Когда владелец явно откладывает недоступные или удалённые remote data, закрывать локальный milestone на утверждённом test fixture, а восстановление real data переносить в deployment gate.
