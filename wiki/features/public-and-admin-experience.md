---
title: Public and Admin Experience
type: feature
status: current
tags:
  - feature
  - public-site
  - admin
created: 2026-07-20
updated: 2026-07-22
verified_at: 2026-07-22T11:24:18Z
verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
code_refs:
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route definitions
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx
    symbol: server-confirmed role gate
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/contexts/AdminContext.tsx
    symbol: administrator workflows
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/01_REQUIREMENTS.md
    symbol: approved product scope
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/09_STATE.md
    symbol: verified implementation state
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/elements/admin/AuthForm.tsx
    symbol: responsive administrator sign-in
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: frontend/storonnimv.client/src/components/elements/admin/BasicAdmins.tsx
    symbol: responsive Basic Admin management table
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/evidence/MOB-05.md
    symbol: responsive administrator acceptance evidence
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
source_refs: []
summary: User-visible routes and the content-management capabilities available to Basic Admin and SuperAdmin roles.
---
# Public and Admin Experience

Visitors can open Home, Schedule, News, Music, Group, Video, Developers, and Error routes. Home combines the nearest schedule item, recent news, and the promotion video. News, Schedule, and Video provide pagination and detail or playback interactions. Group presents the band description, members, and member social links. Music presents streaming destinations and embedded playback. The Developers route intentionally remains a blank static page.

Basic Admin users sign in through /admin. Once the server confirms the session, controls embedded in the public feature pages provide create, edit, delete, photo, and video actions for the supported content. SuperAdmin users can additionally open /admin/basic-admins to list, create, rename, reset passwords for, and delete Basic Admin accounts. Server policy, rather than client storage, decides access.

The committed implementation evidence at the verified commit reports completed responsive verticals for Home, News, Schedule, Group, Music, Footer socials, Video, authentication, media lifecycle, and Basic Admin management. Login and administrator forms are viewport-bounded, validation is announced, inline actions are named touch targets, and the semantic Basic Admin table uses a card presentation at compact widths while retaining its table form on larger viewports. Mock cross-browser coverage and a real disposable API, PostgreSQL, and Blob run verified Basic and SuperAdmin role boundaries plus content and media CRUD. The remaining M3 task is the cross-device accessibility audit.

See [[wiki/architecture/frontend-architecture|Frontend Architecture]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/features/_index|Features]].
