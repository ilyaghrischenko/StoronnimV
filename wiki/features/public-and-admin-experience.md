---
title: Public and Admin Experience
type: feature
status: current
tags:
  - feature
  - public-site
  - admin
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route definitions
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx
    symbol: server-confirmed role gate
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/contexts/AdminContext.tsx
    symbol: administrator workflows
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/01_REQUIREMENTS.md
    symbol: approved product scope
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/09_STATE.md
    symbol: verified implementation state
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: User-visible routes and the content-management capabilities available to Basic Admin and SuperAdmin roles.
---
# Public and Admin Experience

Visitors can open Home, Schedule, News, Music, Group, Video, Developers, and Error routes. Home combines the nearest schedule item, recent news, and the promotion video. News, Schedule, and Video provide pagination and detail or playback interactions. Group presents the band description, members, and member social links. Music presents streaming destinations and embedded playback. The Developers route intentionally remains a blank static page.

Basic Admin users sign in through /admin. Once the server confirms the session, controls embedded in the public feature pages provide create, edit, delete, photo, and video actions for the supported content. SuperAdmin users can additionally open /admin/basic-admins to list, create, rename, reset passwords for, and delete Basic Admin accounts. Server policy, rather than client storage, decides access.

The committed implementation evidence at the verified commit reports completed desktop verticals for Home, News, Schedule, Group, Music, Footer socials, Video, authentication, media lifecycle, and Basic Admin management. The responsive shared shell, Home, News, and Schedule are complete; responsive Music, Group, Video, modals, and admin flows remain in the active roadmap.

See [[wiki/architecture/frontend-architecture|Frontend Architecture]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/features/_index|Features]].
