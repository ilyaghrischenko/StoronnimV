---
title: Local Development
type: runbook
status: current
tags:
  - runbook
  - development
  - verification
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/AGENTS.md
    symbol: backend workspace commands
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/AGENTS.md
    symbol: frontend workspace commands
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/.env.example
    symbol: backend environment template
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/.env.example
    symbol: frontend environment template
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/10_RUNTIME_CONTRACT.md
    symbol: canonical local runtime contract
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/11_MIGRATION_WORKFLOW.md
    symbol: database migration workflow
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Working directories, required configuration, build and test commands, and safe local service boundaries.
---
# Local Development

Use .NET 9 for the backend and the package-lock-compatible Node/npm runtime for the frontend. Run backend commands from the repository root against backend/StoronnimV.Server/StoronnimV.Server.sln. The standard sequence is restore, Release build, and dotnet test with build servers disabled as documented in backend/AGENTS.md. Run frontend commands from frontend/storonnimv.client: npm install when dependencies are absent, then npm run build and npm run lint. No frontend test script is defined at the verified commit.

The backend requires DB_CLOUD, TOKEN_ISSUER, TOKEN_AUDIENCE, TOKEN_KEY, TOKEN_LIFETIME, and CLIENT_URL at startup. BLOB_STORAGE is required for media operations. Structured CookieOptions, RateLimiterOptions, and MediaUpload settings come from appsettings or environment overrides. The frontend requires VITE_API_URL and validates it as an absolute HTTP(S) URL without credentials, query, or fragment.

Use isolated local PostgreSQL and Azure Storage/Azurite resources. Never point local verification at production resources. Apply EF migrations only with the explicit migration workflow after confirming the target; startup does not apply them. The API exposes /health and Development OpenAPI/Swagger. Frontend credentialed requests require exact-origin CORS and compatible cookie settings.

See [[wiki/contracts/authentication-and-media|Authentication and Media Contracts]], [[wiki/database/postgresql-and-storage|PostgreSQL and Storage]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/runbooks/_index|Runbooks]].
