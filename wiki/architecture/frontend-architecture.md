---
title: Frontend Architecture
type: architecture
status: current
tags:
  - architecture
  - react
  - frontend
created: 2026-07-20
updated: 2026-08-04
verified_at: 2026-08-04T11:24:03Z
verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
code_refs:
  - path: frontend/storonnimv.client/src/main.tsx
    symbol: provider composition
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/App.tsx
    symbol: application shell
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route table
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx
    symbol: shared HTTP and UI state
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/AGENTS.md
    symbol: frontend architecture rules
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/shared/FrameLayout.tsx
    symbol: semantic public and admin frame
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/shared/Header.tsx
    symbol: responsive navigation and drawer focus contract
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/hooks/usePrefersReducedMotion.ts
    symbol: live reduced-motion preference
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/styles/shared/FormStyles.scss
    symbol: responsive accessible form primitives
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/admin/BasicAdmins.tsx
    symbol: semantic responsive administrator table
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/evidence/MOB-06.md
    symbol: cross-device accessibility acceptance evidence
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
source_refs: []
summary: React routing, context-based state, API access, UI composition, styling, and responsive implementation boundaries.
---
# Frontend Architecture

The frontend is a React 18 and TypeScript 5.6 single-page application built by Vite 6. main.tsx installs the global and document-metadata providers. App.tsx owns the browser router, semantic shared frame, and modal host. Page.tsx defines the public, authentication, SuperAdmin, developers, error, and fallback routes.

Feature state is split across Home, News, Schedule, Group, Music, Video, and Admin contexts. GlobalContext is the shared boundary for the validated VITE_API_URL value, credentialed Axios requests, antiforgery-token acquisition for unsafe methods, global loading and modal state, authentication detection, and validation errors. Components obtain network behavior from this context rather than constructing independent clients.

Route-level composition lives under components/pages; reusable and feature UI lives under components/elements; response shapes live under models. Administrative forms use controlled state and FormData when files are present. SCSS is the canonical styling source and produces the tracked CSS artifacts.

The public frame renders semantic header, main, and footer landmarks in DOM order, with a skip link to main-content; admin routes omit empty header and footer landmarks. Compact navigation uses a bounded drawer with initial focus, Tab containment, Escape and breakpoint cleanup, focus restoration, and body-scroll restoration. Shared loading, empty, error, list, heading, media, and validation components expose stable accessibility semantics. A live reduced-motion hook controls Swiper autoplay/transitions and Group animation, while CSS disables remaining motion sources without hiding long content. MOB-06 completed the responsive and accessibility work for M3 across the branded browser matrix.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/architecture/_index|Architecture]].
