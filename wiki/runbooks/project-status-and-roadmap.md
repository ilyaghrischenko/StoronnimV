---
title: Project Status and Roadmap
type: runbook
status: needs-review
tags:
  - runbook
  - roadmap
  - needs-review
created: 2026-07-20
updated: 2026-07-22
verified_at: 2026-07-22T11:24:18Z
verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
code_refs:
  - path: docs/implementation/04_BACKLOG.md
    symbol: task backlog
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/05_MILESTONES.md
    symbol: release milestones
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/08_OPEN_ITEMS.md
    symbol: open external decisions
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/09_STATE.md
    symbol: current committed state
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
  - path: docs/implementation/evidence/MOB-05.md
    symbol: MOB-05 acceptance evidence
    kind: file
    verified_commit: 4925fad0abfacb43cb72ef819dfe3615475d49dc
source_refs: []
summary: Verified milestone position, next implementation tasks, and deployment decisions that remain external or unresolved.
---
# Project Status and Roadmap

At the verified commit, milestones M1 and M2 are complete and M3 is active. MOB-01 through MOB-05 are complete: they cover the responsive shell, public content and media pages, shared modals and states, login, the full administrator experience, semantic compact table presentation, and touch-usable CRUD controls. The next planned task is MOB-06, the cross-device accessibility audit that closes M3. M4 adds repeatable backend, frontend, end-to-end, and quality gates. M5 covers production topology, CI, real-content rehearsal, migration and rollback, cleanup, documentation, and deployment. M6 is the final production smoke and acceptance stage.

This page is needs-review because roadmap state can advance independently of stable architecture and because final production topology is not selected in committed evidence. Hosting providers, DNS/TLS, exact frontend/API origins, secret storage, production PostgreSQL and Blob resources, real-content source, backup and rollback ownership, and final acceptance remain external decisions. The local PostgreSQL/Azurite fixture is verification evidence, not production content.

When implementation commits advance the backlog, update this page from committed 04_BACKLOG.md, 05_MILESTONES.md, 08_OPEN_ITEMS.md, and 09_STATE.md. Do not infer deployed status from local code or historical evidence.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]] and [[wiki/runbooks/local-development|Local Development]]. Return to [[wiki/runbooks/_index|Runbooks]].
