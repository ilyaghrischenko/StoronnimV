---
title: Frontend Architecture
type: architecture
status: current
tags:
  - architecture
  - react
  - frontend
created: 2026-07-20
updated: 2026-07-22
verified_at: 2026-07-22T11:24:18Z
verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
code_refs:
  - path: frontend/storonnimv.client/src/main.tsx
    symbol: provider composition
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/App.tsx
    symbol: application shell
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route table
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx
    symbol: shared HTTP and UI state
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/AGENTS.md
    symbol: frontend architecture rules
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/styles/shared/FormStyles.scss
    symbol: responsive form primitives
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/elements/admin/BasicAdmins.tsx
    symbol: semantic responsive administrator table
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
source_refs: []
summary: React routing, context-based state, API access, UI composition, styling, and responsive implementation boundaries.
---
# Frontend Architecture

The frontend is a React 18 and TypeScript 5.6 single-page application built by Vite 6. main.tsx installs the global and document-metadata providers. App.tsx owns the browser router, shared frame, and modal host. Page.tsx defines the public, authentication, SuperAdmin, developers, error, and fallback routes.

Feature state is split across Home, News, Schedule, Group, Music, Video, and Admin contexts. GlobalContext is the shared boundary for the validated VITE_API_URL value, credentialed Axios requests, antiforgery-token acquisition for unsafe methods, global loading and modal state, authentication detection, and validation errors. Components obtain network behavior from this context rather than constructing independent clients.

Route-level composition lives under components/pages; reusable and feature UI lives under components/elements; response shapes live under models. Administrative forms use controlled state and FormData when files are present. SCSS is the canonical styling source and produces the tracked CSS artifacts. The responsive implementation now covers the shared shell, public content and media pages, shared modals and states, login, administrator forms, the semantic Basic Admin table/card presentation, and embedded CRUD controls. The remaining M3 work is the cross-device accessibility audit.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/architecture/_index|Architecture]].
