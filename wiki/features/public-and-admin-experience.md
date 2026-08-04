---
title: Public and Admin Experience
type: feature
status: current
tags:
  - feature
  - public-site
  - admin
created: 2026-07-20
updated: 2026-08-04
verified_at: 2026-08-04T11:24:03Z
verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
code_refs:
  - path: frontend/storonnimv.client/src/components/pages/shared/Page.tsx
    symbol: route definitions
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx
    symbol: server-confirmed role gate
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/contexts/AdminContext.tsx
    symbol: administrator workflows
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/01_REQUIREMENTS.md
    symbol: approved product scope
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/09_STATE.md
    symbol: verified implementation state
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/shared/FrameLayout.tsx
    symbol: semantic public and admin landmarks
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/shared/Header.tsx
    symbol: keyboard-complete compact navigation
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/admin/ValidationErrors.tsx
    symbol: field-linked administrator validation
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: frontend/storonnimv.client/src/components/elements/admin/BasicAdmins.tsx
    symbol: responsive Basic Admin management table
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/evidence/MOB-06.md
    symbol: cross-device accessibility acceptance evidence
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
source_refs: []
summary: User-visible routes and the content-management capabilities available to Basic Admin and SuperAdmin roles.
---
# Public and Admin Experience

Visitors can open Home, Schedule, News, Music, Group, Video, Developers, and Error routes. Home combines the nearest schedule item, recent news, and the promotion video. News, Schedule, and Video provide pagination and detail or playback interactions. Group presents the band description, members, and member social links. Music presents streaming destinations and embedded playback. The Developers route intentionally remains a blank static page.

Basic Admin users sign in through /admin. Once the server confirms the session, controls embedded in the public feature pages provide create, edit, delete, photo, and video actions for the supported content. SuperAdmin users can additionally open /admin/basic-admins to list, create, rename, reset passwords for, and delete Basic Admin accounts. Server policy, rather than client storage, decides access.

The committed frontend presents one logical route heading, semantic landmarks and lists, named controls and media, visible keyboard focus, and at least 44-by-44 CSS-pixel interactive targets. The compact drawer traps focus while open and closes by Escape, navigation, overlay, close control, or transition to desktop; it restores scrolling and a visible focus target. Loading, empty, error, retry, and validation states are announced without duplicate live regions, and administrator field errors are linked to deterministic control ids.

Reduced-motion preference disables automatic carousel and Group movement, removes CSS motion, and keeps long content fully available; runtime preference changes stop and restart the Swipers. Broken Home images use a local fallback or disappear after retry, while failed promotion video becomes an alert with retry. Committed MOB-06 evidence records app-owned axe violations, overflow, and hit failures at zero across Chrome, Safari, Firefox, and Edge matrices. M3 is complete; QA-02 begins M4.

See [[wiki/architecture/frontend-architecture|Frontend Architecture]], [[wiki/contracts/http-api|HTTP API]], and [[wiki/runbooks/project-status-and-roadmap|Project Status and Roadmap]]. Return to [[wiki/features/_index|Features]].
