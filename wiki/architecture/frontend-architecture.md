---
title: Frontend Architecture
type: architecture
status: current
tags:
  - architecture
  - react
  - frontend
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: frontend/storonnimv.client/src/main.tsx
    symbol: provider composition
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/App.tsx
    symbol: application shell
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route table
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx
    symbol: shared HTTP and UI state
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/AGENTS.md
    symbol: frontend architecture rules
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: React routing, context-based state, API access, UI composition, styling, and responsive implementation boundaries.
---
# Frontend Architecture

The frontend is a React 18 and TypeScript 5.6 single-page application built by Vite 6. main.tsx installs the global and document-metadata providers. App.tsx owns the browser router, shared frame, and modal host. Page.tsx defines the public, authentication, SuperAdmin, developers, error, and fallback routes.

Feature state is split across Home, News, Schedule, Group, Music, Video, and Admin contexts. GlobalContext is the shared boundary for the validated VITE_API_URL value, credentialed Axios requests, antiforgery-token acquisition for unsafe methods, global loading and modal state, authentication detection, and validation errors. Components obtain network behavior from this context rather than constructing independent clients.

Route-level composition lives under components/pages; reusable and feature UI lives under components/elements; response shapes live under models. Administrative forms use controlled state and FormData when files are present. Styling is sourced from SCSS and compiled CSS; the current roadmap has established a responsive shared shell plus Home, News, and Schedule, while media-heavy and admin mobile work remains planned at the verified commit.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/architecture/_index|Architecture]].
