---
title: System Overview
type: architecture
status: current
tags:
  - architecture
  - monorepo
  - overview
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: frontend/storonnimv.client/src/main.tsx
    symbol: frontend entry point
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: Page routes
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Program.cs
    symbol: backend composition root
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs
    symbol: service registration
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: High-level map of the React client, ASP.NET Core API, PostgreSQL database, and Azure Blob storage.
---
# System Overview

StoronnimV is a monorepository for the public and administrative website of the Ukrainian rock band Storonnim V. The browser application is a React single-page application. It calls an ASP.NET Core controller API with credentialed requests. PostgreSQL stores content and administrator metadata, while Azure Blob Storage stores uploaded photos and videos. Hangfire runs the recurring schedule-status update against PostgreSQL.

The main request path is: route-level React page → feature context or form → shared Axios request wrapper → API controller → controller service → entity, home, or identity service → domain repository contract → EF Core or Blob implementation. Read responses use domain projections and AutoMapper DTO mappings. Mutations are protected by server-side authentication and, for cookie-authenticated unsafe requests, antiforgery validation.

The backend project dependency direction is Api → Application + Infrastructure, Application → Domain, and Infrastructure → Domain. The frontend is organized around pages, feature elements, feature contexts, models, and shared state.

Continue with [[wiki/architecture/backend-architecture|Backend Architecture]], [[wiki/architecture/frontend-architecture|Frontend Architecture]], and [[wiki/domain/content-model|Content Model]]. Return to [[wiki/architecture/_index|Architecture]].
