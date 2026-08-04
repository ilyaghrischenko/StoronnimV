# План завершения StoronnimV

**Дата подготовки:** 12 июля 2026 года
**Состояние:** планирование завершено; `BASE-01`, `BASE-02`, `DATA-01`, `BASE-03`, `BASE-04`, `DATA-02`, `QA-01`, `API-01`, `DATA-03`, `API-02`, `FEAT-01`, `FEAT-02`, `API-03`, `DATA-04`, `FEAT-03`, `FEAT-04`, `FEAT-05`, `API-04`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `FEAT-09`, `FEAT-10`, `MOB-01`, `MOB-02`, `MOB-03`, `MOB-04`, `MOB-05` и `MOB-06` выполнены; `M1`, `M2` и `M3` завершены, `M4` активен; следующая задача — `QA-02`.

## Назначение

Каталог содержит утверждённый план доведения существующего сайта музыкальной группы до проверяемого завершения и evidence выполненных задач.

## Документы и порядок чтения

1. [01_REQUIREMENTS.md](01_REQUIREMENTS.md) — утверждённый объём и ограничения.
2. [02_DECISIONS.md](02_DECISIONS.md) — журнал решений владельца и технических допущений.
3. [03_IMPLEMENTATION_PLAN.md](03_IMPLEMENTATION_PLAN.md) — этапы, зависимости и критический путь.
4. [04_BACKLOG.md](04_BACKLOG.md) — implementation-ready задачи.
5. [05_MILESTONES.md](05_MILESTONES.md) — демонстрируемые этапы выпуска.
6. [06_VALIDATION_PLAN.md](06_VALIDATION_PLAN.md) — автоматическая и ручная проверка.
7. [07_TRACEABILITY.md](07_TRACEABILITY.md) — связь анализа, требований и backlog.
8. [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md) — неблокирующие внешние решения.
9. [09_STATE.md](09_STATE.md) — компактное состояние для следующих сеансов Codex.
10. [10_RUNTIME_CONTRACT.md](10_RUNTIME_CONTRACT.md) — канонические runtime requirements, configuration names и безопасная последовательность подготовки локального окружения.
11. [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md) — явная безопасная команда EF migrations и schema inspection.
12. [12_DATA_COPY_WORKFLOW.md](12_DATA_COPY_WORKFLOW.md) — безопасный local/production workflow backup, restore, Blob copy и reconciliation.
13. [13_SUPERADMIN_BOOTSTRAP.md](13_SUPERADMIN_BOOTSTRAP.md) — безопасное ручное создание первого SuperAdmin, login proof и rotation/recovery boundaries.
14. [evidence/BASE-02.md](evidence/BASE-02.md) — фактические команды и результаты clean backend restore/build.
15. [evidence/DATA-01.md](evidence/DATA-01.md) — фактические команды и результаты migration workflow на пустой PostgreSQL.
16. [evidence/BASE-03.md](evidence/BASE-03.md) — фактические команды и результаты local API startup, health и Development OpenAPI.
17. [evidence/BASE-04.md](evidence/BASE-04.md) — фактические команды и результаты frontend environment API URL, production bundle и browser network inspection.
18. [evidence/DATA-02.md](evidence/DATA-02.md) — фактические команды и результаты local PostgreSQL/Azurite fixture backup, restore и media copy.
19. [evidence/QA-01.md](evidence/QA-01.md) — browser/API evidence первого Home/News public vertical и различимых loading/empty/error states.
20. [evidence/API-01.md](evidence/API-01.md) — TDD и integration evidence middleware order, JWT cookie/header principal и anonymous/Basic/SuperAdmin decisions.
21. [evidence/DATA-03.md](evidence/DATA-03.md) — controlled local bootstrap, guarded rerun и real API SuperAdmin login evidence.
22. [evidence/API-02.md](evidence/API-02.md) — cookie/CORS/CSRF topology, integration tests и real browser login/logout evidence.
23. [evidence/FEAT-01.md](evidence/FEAT-01.md) — Basic Admin login errors, refresh session detection и logout через real browser/API/PostgreSQL flow.
24. [evidence/FEAT-02.md](evidence/FEAT-02.md) — server-confirmed route role, forged client-role rejection, loading/forbidden states и stable refresh evidence.
25. [evidence/API-03.md](evidence/API-03.md) — JSON/multipart endpoint matrix, typed ISO dates, unified problem JSON и public DTO alignment evidence.
26. [evidence/DATA-04.md](evidence/DATA-04.md) — upload policy, DB/Blob compensation, real PostgreSQL/Azurite lifecycle и fault-injection evidence.
27. [evidence/FEAT-03.md](evidence/FEAT-03.md) — независимые Home loading/empty/error/retry states, nullable contracts и semantic section navigation.
28. [evidence/FEAT-04.md](evidence/FEAT-04.md) — News pagination/detail, полный CRUD, точные dates и согласованный photo/video lifecycle.
29. [evidence/FEAT-05.md](evidence/FEAT-05.md) — Schedule list/detail/status/map, точные dates, полный CRUD и согласованный photo lifecycle.
30. [evidence/API-04.md](evidence/API-04.md) — awaited/idempotent Schedule status job, DI recurring-job registration и Production dashboard route gate.
31. [evidence/FEAT-06.md](evidence/FEAT-06.md) — Group singleton constraint, полный member/social CRUD, media lifecycle и browser mutation readback.
32. [evidence/FEAT-07.md](evidence/FEAT-07.md) — Music/group-social CRUD, safe external URL validation, photo lifecycle и browser link evidence.
33. [evidence/FEAT-08.md](evidence/FEAT-08.md) — Video category pagination/CRUD/playback, safe promotion replacement и original local category media.
34. [evidence/FEAT-09.md](evidence/FEAT-09.md) — SuperAdmin Basic Admin CRUD, live UI state, password contract, role/type boundaries и unique login invariant.
35. [evidence/FEAT-10.md](evidence/FEAT-10.md) — пустая Developers page, synchronous route title, direct-link rewrite и browser evidence.
36. [evidence/MOB-01.md](evidence/MOB-01.md) — responsive shared foundation, единый SCSS workflow и before/after viewport evidence.
37. [evidence/MOB-02.md](evidence/MOB-02.md) — responsive Home/News/Schedule, Swiper, cards, pagination, map и cross-browser viewport evidence.
38. [evidence/MOB-03.md](evidence/MOB-03.md) — responsive Music/Group/Video, Swiper, embeds, media playback и cross-browser viewport evidence.
39. [evidence/MOB-04.md](evidence/MOB-04.md) — accessible responsive modals, semantic request states, retry, media policy и cross-browser viewport evidence.
40. [evidence/MOB-05.md](evidence/MOB-05.md) — responsive login/admin forms, semantic table/cards, touch controls и real role/CRUD evidence.
41. [evidence/MOB-06.md](evidence/MOB-06.md) — cross-device semantics, keyboard/focus, reduced motion, axe/overflow и branded Chrome/Safari/Firefox/Edge evidence.

## Подтверждено владельцем

- Сохраняются публичные страницы Home, Schedule, News, Music, Group и Video.
- `/developers` сохраняется как пустая статическая страница.
- Полный content admin и SuperAdmin входят в релиз и работают на mobile, tablet и desktop.
- Публичная мобильная версия полноценная; разрешён отдельный упрощённый layout без обязательного сохранения SVG-рамки.
- Визуальный baseline — текущий runtime `style.css` и фактический desktop UI.
- Для `M1`–`M4` используется утверждённый локальный PostgreSQL/Azurite test corpus; реальный production content импортируется только перед `M5`.
- `GroupPage` — singleton.
- Первый SuperAdmin вручную вносится в подготовленную БД.
- Migrations применяются явной командой, не автоматически при startup.
- Hangfire dashboard отключён в production.
- Analytics в текущий релиз не входит.
- Для uploads обязательны лимиты и проверки файлов.
- Для трёх Video category tiles владелец утвердил генерацию original photorealistic black/gold assets без текста, брендов и watermark.
- Закоммиченные исторические logs должны быть удалены; новая стратегия логирования пока не определяется.
- Сначала восстанавливается локальное окружение; production deployment остаётся поздним этапом.

## Остаётся открытым

Открытые пункты не блокируют первый milestone: production hosting/topology и доступы, точные upload-лимиты, выбор источника реального production content перед `M5`, будущая стратегия логирования и явное подтверждение принимающего лица. Они перечислены в [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md) с крайним этапом решения.
