---
title: Project Status and Roadmap
type: runbook
status: needs-review
tags:
  - runbook
  - roadmap
  - needs-review
created: 2026-07-20
updated: 2026-08-04
verified_at: 2026-08-04T11:24:03Z
verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
code_refs:
  - path: docs/implementation/04_BACKLOG.md
    symbol: task backlog
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/05_MILESTONES.md
    symbol: release milestones
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/08_OPEN_ITEMS.md
    symbol: open external decisions
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/09_STATE.md
    symbol: current committed state
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
  - path: docs/implementation/evidence/MOB-06.md
    symbol: MOB-06 acceptance evidence
    kind: file
    verified_commit: 5630ad2a32259f894f3765c001d2d3f524485907
source_refs: []
summary: Verified milestone position, next implementation tasks, and deployment decisions that remain external or unresolved.
---
# Project Status and Roadmap

At the verified commit, milestones M1, M2, and M3 are complete and M4 is active. MOB-01 through MOB-06 cover the responsive shell, public content and media pages, shared modals and states, login, the full administrator experience, semantic compact table presentation, touch-usable controls, cross-device keyboard/focus behavior, accessibility semantics, and reduced motion. The next planned task is QA-02, the backend regression suite. QA-03 and QA-04 then add frontend and end-to-end regression coverage before the M4 quality gates. M5 covers production topology, CI, real-content rehearsal, migration and rollback, cleanup, documentation, and deployment. M6 is the final production smoke and acceptance stage.

This page is needs-review because roadmap state can advance independently of stable architecture and because final production topology is not selected in committed evidence. Hosting providers, DNS/TLS, exact frontend/API origins, secret storage, production PostgreSQL and Blob resources, real-content source, backup and rollback ownership, and final acceptance remain external decisions. Disposable localhost browser evidence is verification data, not a production deployment or content source.

When implementation commits advance the backlog, update this page from committed 04_BACKLOG.md, 05_MILESTONES.md, 08_OPEN_ITEMS.md, and 09_STATE.md. Do not infer deployed status from local code or historical evidence.

See [[wiki/features/public-and-admin-experience|Public and Admin Experience]] and [[wiki/runbooks/local-development|Local Development]]. Return to [[wiki/runbooks/_index|Runbooks]].
