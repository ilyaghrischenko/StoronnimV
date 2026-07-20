---
title: PostgreSQL and Storage
type: database
status: current
tags:
  - database
  - postgresql
  - ef-core
  - azure-blob
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimVContext.cs
    symbol: StoronnimVContext
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs
    symbol: current EF Core model
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/20260715012000_EnforceGroupPageSingleton.cs
    symbol: GroupPage singleton migration
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/20260717233000_EnforceAdminLoginUniqueness.cs
    symbol: Admin login uniqueness migration
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/11_MIGRATION_WORKFLOW.md
    symbol: migration runbook
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Persistence topology, entity sets, integrity constraints, migrations, and the boundary between metadata and media bytes.
---
# PostgreSQL and Storage

PostgreSQL stores nine application sets: Admins, GroupPages, GroupSocials, Members, Socials, NewsItems, Schedules, MusicPlatforms, and Videos. It also serves as Hangfire storage. Member-to-Social is a one-to-many relationship with cascade delete. News can reference an optional Video. Media-bearing entities store blob identifiers and URLs rather than file bytes.

Two explicit database invariants are important. GroupPages is constrained to at most one row by a unique expression index, and the migration refuses to create the index when duplicates already exist. Admin.Login is unique, with a migration guard that fails before creating the index if duplicate logins exist. These constraints complement application-service checks and protect concurrent writes.

EF Core migrations are applied through the documented explicit workflow; the API does not migrate or seed the database at startup. DB_CLOUD is shared by EF Core, Hangfire, and the PostgreSQL health check. Azure Blob Storage uses BLOB_STORAGE and separate photo/video containers. Database and Blob operations are not one transaction, so media services implement ordered updates and compensating cleanup.

See [[wiki/domain/content-model|Content Model]], [[wiki/contracts/authentication-and-media|Authentication and Media Contracts]], and [[wiki/runbooks/local-development|Local Development]]. Return to [[wiki/database/_index|Database]].
