---
title: Content Model
type: domain
status: current
tags:
  - domain
  - content
  - administration
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/News.cs
    symbol: News
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/Schedule.cs
    symbol: Schedule
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/GroupPage.cs
    symbol: GroupPage
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/Member.cs
    symbol: Member
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/Admin.cs
    symbol: Admin
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Domain/Entities/Video.cs
    symbol: Video
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Core content entities, administrator roles, relationships, and lifecycle classifications used by the site.
---
# Content Model

The content model mirrors the public site. News represents dated editorial items with optional photo and optional attached video. Schedule represents events with date, location, status, and optional photo. GroupPage is the singleton band description and can contain Members; each Member can have Social links. GroupSocial stores band-level external links. MusicPlatform stores streaming destinations and imagery. Video stores playable media classified by VideoType, including the promotion video used by Home.

BaseEntity supplies the shared identifier and timestamps used across content records. Domain enums classify administrator type, news priority, schedule status, social type, and video type. Projection types define read-optimized shapes for list, detail, home, and pagination queries without exposing EF entities directly.

Admin records represent Basic Admin and SuperAdmin roles. Basic Admin accounts manage content. SuperAdmin-only endpoints manage Basic Admin accounts and do not provide general role escalation. Database uniqueness protects administrator logins, and the documented bootstrap process creates the first SuperAdmin outside normal content CRUD.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]], [[wiki/database/postgresql-and-storage|PostgreSQL and Storage]], and [[wiki/contracts/http-api|HTTP API]]. Return to [[wiki/domain/_index|Domain]].
