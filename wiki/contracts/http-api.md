---
title: HTTP API
type: contract
status: current
tags:
  - contract
  - http
  - api
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/StoronnimV.Server/StoronnimV.Api/Controllers/HomeController.cs
    symbol: home endpoints
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Controllers/NewsController.cs
    symbol: news endpoints
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs
    symbol: content administration endpoints
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Controllers/SuperAdminController.cs
    symbol: administrator account endpoints
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Models/ApiErrorResponse.cs
    symbol: problem response contract
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/10_RUNTIME_CONTRACT.md
    symbol: runtime HTTP contract
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Controller route groups, public reads, protected mutations, pagination, and response conventions.
---
# HTTP API

Public controller groups are /api/home, /api/news, /api/schedules, /api/group, /api/music, /api/videos, and /api/group-socials. They expose home summaries, list/detail reads, and paginated resources. Page numbers are route parameters and page size is supplied where the controller contract supports it.

Identity starts under /api/account: login is an unsafe request and /csrf-token issues the request token used by cookie-authenticated browser mutations. /api/admin exposes server-confirmed administrator state and role, logout, and content CRUD for news, schedules, videos, group data, members, music platforms, member socials, and group socials. JSON is used for body-bound text edits; multipart form data is used for upload-bearing operations. /api/super-admin/basic-admins is protected by the SuperAdminOnly policy and manages Basic Admin accounts.

Public and administrator rate-limit policies are applied at controller level. Expected validation, authorization, not-found, media, and server failures use the shared application/problem+json response shape. Unknown server errors do not expose internal exception detail.

The TypeScript client models and contexts are consumers of these DTOs. Contract changes should be reviewed across controller request/response types, AutoMapper profiles, frontend models, and every affected context or form.

See [[wiki/contracts/authentication-and-media|Authentication and Media Contracts]] and [[wiki/features/public-and-admin-experience|Public and Admin Experience]]. Return to [[wiki/contracts/_index|Contracts]].
