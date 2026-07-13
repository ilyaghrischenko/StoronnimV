# Traceability

| Источник | Вывод или требование | Решение | Задачи backlog | Критерий проверки |
|---|---|---|---|---|
| Analysis 02, 05 | Backend build/run не доказан; env docs расходятся | Сначала reproducible local foundation | `BASE-01`, `BASE-02`, `BASE-03` | Clean restore/build/run и health green |
| Analysis 01, 04 | Frontend API URL hardcoded localhost | Environment-driven API URL | `BASE-04`, `QA-01` | Bundle без localhost; browser получает данные |
| DEC-017 | Локальные milestones используют PostgreSQL/Azurite test corpus | Deterministic fixture, backup/inventory/non-production restore | `DATA-02`, `QA-01` | Counts, Blob checksums и sampled local media сверены; restore доказан |
| DEC-006, DEC-017 | Реальный content source перенесён в deployment readiness | Выбрать production source и выполнить отдельный real-data rehearsal | `OPS-03`, `QA-06` | Production counts/media и restore доказаны до выпуска |
| DEC-009 | Migrations выполняются отдельно | Explicit migration workflow | `DATA-01`, `OPS-03` | Empty DB update и production-like rehearsal green |
| Analysis 05, 09 | Authentication middleware/policy разрыв | Server-validated auth | `API-01`, `API-02`, `FEAT-01`, `FEAT-02` | Auth/role/CSRF matrix проходит |
| DEC-008 | Первый SuperAdmin создаётся вручную | Secure bootstrap runbook | `DATA-03`, `DOC-01` | Clean DB account создаётся без secret in Git |
| Analysis 06 | JSON/FormData/date/status/error mismatches | Единые контракты | `API-03`, `FEAT-04`, `FEAT-05`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `FEAT-09` | Endpoint contract tests и readback green |
| Analysis 09, DEC-013 | DB/Blob non-atomic; upload validation отсутствует | Validation и compensation pattern | `DATA-04`, `FEAT-04`, `FEAT-05`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `QA-04` | Invalid input rejected; fault injection invariant preserved |
| Public Home requirement | Home schedule/news/video и states | Завершить vertical | `FEAT-03`, `MOB-02`, `QA-04` | Real, loading, empty и error scenarios green |
| Public News requirement | News pagination/detail | Read + full admin CRUD | `FEAT-04`, `MOB-02`, `QA-04` | Pagination/detail/mutations/readback green |
| Public Schedule requirement | Schedule pagination/detail/map/status | Read + CRUD + job | `FEAT-05`, `API-04`, `MOB-02` | UI/API/date/status/map/job green |
| DEC-007 | GroupPage singleton; members/socials required | Enforce singleton и complete CRUD | `FEAT-06`, `MOB-03`, `QA-02` | Второй GroupPage rejected; full readback green |
| Public Music/Footer requirement | Platforms, Spotify и group socials | Complete links CRUD | `FEAT-07`, `MOB-03`, `QA-04` | Links/embed/footer and mutations green |
| Public Video requirement | Three categories, pagination, promotion | Complete media vertical | `FEAT-08`, `MOB-03`, `QA-04` | Categories/playback/promotion failure path green |
| DEC-002 | Full SuperAdmin on all devices | Complete account management | `FEAT-09`, `MOB-05`, `QA-04` | CRUD Basic Admin; SuperAdmin protected |
| DEC-001 | Developers remains empty static page | Stable empty route | `FEAT-10`, `MOB-06` | Empty direct route renders without error |
| Analysis 07, DEC-004, DEC-005 | Desktop-only geometry; runtime CSS baseline | Responsive foundation, simplified mobile | `MOB-01`, `MOB-02`, `MOB-03`, `MOB-04`, `MOB-05`, `MOB-06` | 320/375/768/1024/1440 matrix green |
| DEC-011 | Browser/accessibility/test baseline accepted | Automated and manual quality gates | `QA-02`, `QA-03`, `QA-04`, `QA-05` | Build/lint/tests/E2E/audits green |
| DEC-003 | Local first; deployment later | Separate deployment milestone | `OPS-01`, `OPS-02`, `OPS-03`, `OPS-04`, `QA-06` | Topology decided; rehearsal/deploy/smoke green |
| DEC-010 | Dashboard disabled in production | Environment gate | `API-04`, `QA-05`, `QA-06` | Production route inaccessible |
| DEC-014 | Historical tracked logs removed | Scoped cleanup | `CLEAN-01`, `DOC-01` | No tracked logs/generated noise; clean build |
| Completion criteria | Нет P0/P1 и owner can complete scenarios | Final release audit | `QA-07` | Traceability verified and owner acceptance recorded |

## Coverage audit

- Каждая обязательная страница связана с feature, mobile и validation задачей.
- Auth, SuperAdmin, API contracts, media consistency, Hangfire и deployment risks имеют отдельные задачи.
- Каждая backlog task имеет источник в собственной строке `04_BACKLOG.md`; grouped ranges выше не заменяют task-level source.
- Исключённые analytics, contact forms, multilingual UI и новая dashboard в backlog отсутствуют.
