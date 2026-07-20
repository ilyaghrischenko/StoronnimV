---
title: Backend Architecture
type: architecture
status: current
tags:
  - architecture
  - aspnet-core
  - backend
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/StoronnimV.Server/StoronnimV.Server.sln
    symbol: solution structure
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Program.cs
    symbol: HTTP pipeline
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs
    symbol: dependency injection modules
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/AGENTS.md
    symbol: backend architecture rules
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Responsibilities and dependency boundaries of the API, Application, Domain, Infrastructure, and Tests projects.
---
# Backend Architecture

The backend targets .NET 9 and is split into five projects. StoronnimV.Api owns controllers, middleware, runtime configuration, OpenAPI, health checks, CORS, authentication, authorization, response compression, rate limiting, and the Hangfire dashboard environment gate. StoronnimV.Application owns controller services, business orchestration, DTOs, validators, mapping profiles, identity services, media policy, and the recurring schedule job. StoronnimV.Domain owns entities, enums, projections, and repository contracts. StoronnimV.Infrastructure owns EF Core, PostgreSQL repositories and migrations, and the Azure Blob adapter. StoronnimV.Tests references the API project and contains HTTP, authentication, CRUD, media, and background-job tests.

Controllers stay thin and delegate to controller services. Expected application failures are mapped by centralized middleware to the shared problem JSON shape. Read-only repository paths use projections; mutations pass cancellation tokens through services and repositories. Program.cs is the composition root and orders routing, CORS, authentication, antiforgery middleware, and authorization before mapping controllers.

The application intentionally uses explicit migrations rather than applying migrations during startup. The Hangfire dashboard is available only outside Production, while the recurring job is registered independently through IRecurringJobManager.

See [[wiki/contracts/http-api|HTTP API]], [[wiki/contracts/authentication-and-media|Authentication and Media Contracts]], and [[wiki/database/postgresql-and-storage|PostgreSQL and Storage]]. Return to [[wiki/architecture/_index|Architecture]].
