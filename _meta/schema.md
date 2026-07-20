---
title: Project Wiki Schema
type: meta
status: current
tags:
  - schema
---
# Project Wiki Schema

Schema version 1.

Humans own code, `Plans.md`, `plans/**`, and `inbox/**`. Plugin owns `wiki/**`, `_meta/manifest.json`, `_meta/log.md`, and `_meta/runs/**`. Plugin owns only its delimited block in `AGENTS.md`.

Allowed page statuses: `draft`, `current`, `needs-review`, `stale`, `deprecated`, `superseded`.

Allowed page types: `architecture`, `database`, `domain`, `contract`, `feature`, `adr`, `runbook`, `index`.
