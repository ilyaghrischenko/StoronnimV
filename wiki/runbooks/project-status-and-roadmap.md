---
title: Project Status and Roadmap
type: runbook
status: needs-review
tags:
  - runbook
  - roadmap
  - needs-review
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: docs/implementation/04_BACKLOG.md
    symbol: task backlog
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/05_MILESTONES.md
    symbol: release milestones
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/08_OPEN_ITEMS.md
    symbol: open external decisions
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: docs/implementation/09_STATE.md
    symbol: current committed state
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Verified milestone position, next implementation tasks, and deployment decisions that remain external or unresolved.
---
# Project Status and Roadmap

At the verified commit, milestones M1 and M2 are complete and M3 is active. MOB-01 and MOB-02 established the responsive shell plus Home, News, and Schedule. The next planned task is MOB-03 for responsive Music, Group, and Video. MOB-04 through MOB-06 cover modals and shared states, the full administrator experience, and cross-device accessibility. M4 adds repeatable backend, frontend, end-to-end, and quality gates. M5 covers production topology, CI, real-content rehearsal, migration and rollback, cleanup, documentation, and deployment. M6 is the final production smoke and acceptance stage.

This page is needs-review because roadmap state can advance independently of stable architecture and because final production topology is not selected in committed evidence. Hosting providers, DNS/TLS, exact frontend/API origins, secret storage, production PostgreSQL and Blob resources, real-content source, backup and rollback ownership, and final acceptance remain external decisions. The local PostgreSQL/Azurite fixture is verification evidence, not production content.

When implementation commits advance the backlog, update this page from committed 04_BACKLOG.md, 05_MILESTONES.md, 08_OPEN_ITEMS.md, and 09_STATE.md. Do not infer deployed status from local code or historical evidence.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]] and [[wiki/runbooks/local-development|Local Development]]. Return to [[wiki/runbooks/_index|Runbooks]].
