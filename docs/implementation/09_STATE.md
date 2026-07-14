# Состояние проекта для будущих сеансов

## Текущая цель

Выполнить утверждённый план завершения StoronnimV. `M1` завершён; в `M2` завершены `API-01`, `DATA-03`, `API-02`, `FEAT-01`, `FEAT-02`, `API-03`, `DATA-04`, `FEAT-03`, `FEAT-04` и `FEAT-05`. Runtime/build/migrations/startup/environment URL/local corpus доказаны; authentication и HTTP contracts проверены; media lifecycle использует подтверждённую policy/compensation; Home имеет независимые nullable-safe states/navigation; News и Schedule verticals подтверждены API/browser readback.

## Утверждённый объём

- Public: Home, Schedule, News, Music, Group, Video, Footer/socials, Error и пустая static Developers page.
- Admin: полный content/media CRUD и SuperAdmin management Basic Admin accounts.
- Devices: mobile, tablet и desktop для public/admin.
- Data: для `M1`–`M4` используется локальный PostgreSQL/Azurite test corpus; источник реального production content решается перед `M5`.
- Operations: Hangfire status job; production dashboard disabled; explicit migrations; последующий deployment.

## Исключено

Analytics, contact/booking forms, commerce/tickets, search, multilingual UI, новая admin dashboard, automatic startup migrations, public Hangfire dashboard и новая logging strategy.

## Активный milestone

`M2 — Функционально завершённая desktop-версия`.

## Следующая задача

`API-04 — Исправить Schedule job и production dashboard gate`. Её зависимость `FEAT-05` завершена. `API-04` не начиналась.

## Ключевые ограничения

- Читать актуальный root/user `AGENTS.md` перед каждой задачей.
- Выполнять только одну backlog task или явно согласованный связанный набор.
- Не менять production DB/Blob до backup и authorization.
- Не хранить secrets/credentials в Git, logs или документации.
- Сохранять React/.NET/PostgreSQL/Azure/Hangfire architecture.
- Desktop visual baseline — runtime `style.css`; mobile frame можно упрощать.
- Migrations только отдельной командой; SuperAdmin вручную.

## Команды проверки

Канонический runtime contract: [10_RUNTIME_CONTRACT.md](10_RUNTIME_CONTRACT.md). Evidence: [evidence/BASE-02.md](evidence/BASE-02.md), [evidence/DATA-01.md](evidence/DATA-01.md), [evidence/BASE-03.md](evidence/BASE-03.md), [evidence/BASE-04.md](evidence/BASE-04.md), [evidence/DATA-02.md](evidence/DATA-02.md), [evidence/QA-01.md](evidence/QA-01.md), [evidence/API-01.md](evidence/API-01.md), [evidence/DATA-03.md](evidence/DATA-03.md), [evidence/API-02.md](evidence/API-02.md), [evidence/FEAT-01.md](evidence/FEAT-01.md), [evidence/FEAT-02.md](evidence/FEAT-02.md), [evidence/API-03.md](evidence/API-03.md), [evidence/DATA-04.md](evidence/DATA-04.md), [evidence/FEAT-03.md](evidence/FEAT-03.md), [evidence/FEAT-04.md](evidence/FEAT-04.md), [evidence/FEAT-05.md](evidence/FEAT-05.md).

`BASE-02` проверена 13 июля 2026 года на macOS 26.5 arm64 с .NET SDK 9.0.203. Финальная проверка использовала новые изолированные `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH` и artifacts path вне репозитория:

```bash
dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers
dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers
dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers
dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --no-build --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers
```

Restore завершился с 0 errors и 2 warnings. Solution Release build завершился с 0 errors и 8 warnings; startup API Release build — с 0 errors и 2 warnings. `dotnet test` завершился с exit code 0, но test assembly не содержит доступных тестов. Metadata scan не нашёл machine-specific references. Windows-specific `HintPath` удалён; существующий `Microsoft.Extensions.Configuration` 9.0.0 `PackageReference` сохранён. Версии packages и `net9.0` не менялись. API startup, PostgreSQL, Blob, `/health`, OpenAPI и runtime behavior не проверялись. Migration command выполняется только после проверки target connection и backup согласно `DATA-01`/`OPS-03`.

`DATA-01` проверена 13 июля 2026 года на одноразовом локальном PostgreSQL 17 container. Local `dotnet-ef` 9.0.7 восстановлен из `.config/dotnet-tools.json`; Infrastructure design-time factory читает только `DB_CLOUD` и не запускает API/Hangfire. Подтверждённо пустая БД получила все 24 migrations и 9 application tables; повторная canonical command не применила migrations; `__EFMigrationsHistory` содержит все 24 записи; pending model changes отсутствуют. Финальные solution restore/build/test завершились exit 0; test assembly по-прежнему не содержит тестов. Container удалён. Полные команды и результаты: [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md) и [evidence/DATA-01.md](evidence/DATA-01.md).

`BASE-03` проверена 13 июля 2026 года с API в Development и одноразовой PostgreSQL 17 на случайном localhost port. Process environment теперь имеет приоритет над `.env`, поэтому канонический `dotnet run` безопасно использовал явно заданный local target. `/health`, `/openapi/v1.json` и `/swagger/index.html` вернули `200`; API и PostgreSQL health entries имели статус `Healthy`; отсутствующий `DB_CLOUD` дал явную `EnvVariableNotFoundException`. Clean restore, solution/API Release build и test command завершились exit 0; test assembly не содержит тестов. Container удалён. Полные команды и результаты: [evidence/BASE-03.md](evidence/BASE-03.md).

`BASE-04` проверена 13 июля 2026 года. Vite теперь требует и валидирует absolute HTTP(S) `VITE_API_URL`, отклоняет credentials/query/fragment и удаляет trailing slash перед встраиванием. `npm run build` завершился exit 0; production bundle содержит configured environment URL и не содержит hardcoded `localhost:44315`. Встроенный browser через Vite выполнил `GET /api/group-socials` к одноразовому local mock API, заданному process environment. Full ESLint сохранил documented baseline 6 errors/20 warnings и остаётся вне scope до `QA-03`. Полные команды и результаты: [evidence/BASE-04.md](evidence/BASE-04.md).

`DATA-02` проверена 13 июля 2026 года на двух одноразовых PostgreSQL 17 и двух Azurite Blob targets, доступных только через localhost. В source применены все 24 migrations и deterministic fixture: по одной записи `GroupPage`, `GroupSocial`, `Member`, `MusicPlatform`, `NewsItem`, `Schedule`, `Social`, четыре `Video` и ноль `Admin`. Custom-format dump размером 19 743 bytes восстановлен в пустой target; source/target DB inventories совпали. Один JPEG и один реальный MP4 скопированы между Azurite targets; name/size/content type и SHA-256 совпали. Все семь используемых media fields вернули HTTP 200 с ожидаемыми `image/jpeg`/`video/mp4`; MP4 имеет длительность 1 секунду. Полные команды и результаты: [12_DATA_COPY_WORKFLOW.md](12_DATA_COPY_WORKFLOW.md) и [evidence/DATA-02.md](evidence/DATA-02.md).

`QA-01` проверена 13 июля 2026 года на disposable PostgreSQL 17/Azurite с утверждённым `DATA-02` corpus, real API и Vite. API health и пять Home/News read endpoints вернули HTTP 200. Встроенный browser показал Home, News list и detail с fixture media; controlled delayed-empty/error scenarios доказали отдельные loading/empty/error states. Финальное happy-path окно browser console содержало 0 warnings/errors, а API log подтвердил CORS и public 200 responses. Frontend/backend builds прошли; task ESLint — 0 errors; full ESLint сохранил baseline 6 errors/20 warnings. `dotnet test` завершился exit 0, но test assembly содержит 0 тестов. Полные результаты: [evidence/QA-01.md](evidence/QA-01.md).

`API-01` проверена 13 июля 2026 года через real API startup и disposable PostgreSQL 17. `UseAuthentication()` явно выполняется перед `UseAuthorization()`. Header/cookie principal, anonymous `401`, invalid/expired `401`, Basic Admin access, Basic-to-SuperAdmin `403`, SuperAdmin `200` и logout покрыты 11 integration/wiring tests. Fresh restore, solution/API Release builds и full tests завершились exit 0; test assembly теперь содержит 11 tests. Старое предположение о полной недостижимости protected endpoints без явного middleware оказалось слишком сильным для текущего .NET 9, но явный порядок теперь закреплён и проверяется. Полные результаты: [evidence/API-01.md](evidence/API-01.md).

`DATA-03` проверена 13 июля 2026 года на clean disposable PostgreSQL 17. Временный untracked .NET helper использовал application-compatible `PasswordHasher<Admin>`, передал guarded transaction прямо в `psql` и не печатал credentials/hash. Aggregate DB check подтвердил ровно одну `Type = 1` запись; real API login вернул `200`, role `SuperAdmin` и token cookie. Повторный bootstrap отказал с exit 3, row count остался 1. Full backend restore/build/tests завершились exit 0; 11/11 tests passed. Runbook: [13_SUPERADMIN_BOOTSTRAP.md](13_SUPERADMIN_BOOTSTRAP.md). Evidence: [evidence/DATA-03.md](evidence/DATA-03.md).

`API-02` проверена 14 июля 2026 года integration tests и real Firefox browser flow на disposable PostgreSQL 17. Backend выдаёт no-store antiforgery token, валидирует login и unsafe authenticated cookie requests, сохраняет bearer-only mutations без CSRF requirement и принимает только exact `CLIENT_URL`; invalid JWT cookie сохраняет `401`. Frontend получает fresh token перед unsafe request и передаёт `X-CSRF-TOKEN`. Browser выполнил token `200` → login `200` → cookie-auth `isAdmin` `200` → token `200` → logout `200`; controlled cookie mutation без token получила `400`, unknown CORS origin не получил allow-origin. Full backend tests: 17/17; frontend build и targeted lint green. Full ESLint baseline теперь 5 errors/20 warnings вне изменённого файла и остаётся `QA-03`. Evidence: [evidence/API-02.md](evidence/API-02.md).

`FEAT-01` проверена 14 июля 2026 года real Safari browser flow на disposable PostgreSQL 17 и real API/Vite. Login получил `200`, full refresh восстановил admin UI после `isAdmin 200`, logout получил `200`, следующий refresh получил `isAdmin 401` и не показал admin controls. `401` login error прошёл browser RED/GREEN и отображается пользователю; navigation/role меняются только после `200`. Frontend build и targeted lint green; backend tests 17/17. Full ESLint сохраняет 5 errors/14 warnings вне FEAT-01 files и остаётся `QA-03`. Evidence: [evidence/FEAT-01.md](evidence/FEAT-01.md).

`FEAT-02` проверена 14 июля 2026 года TDD, ASP.NET integration tests и controlled browser role matrix. `GET /api/admin/role` возвращает JWT role для header/cookie transports. Guard до ответа показывает только loading state; forged client `SuperAdmin` при server `401` не монтирует protected content, Basic получает `403`, SuperAdmin открывает route и после full refresh остаётся на нём без flicker/loop. Frontend build и targeted lint green; backend tests 21/21. Full ESLint сохраняет 5 errors/13 warnings вне FEAT-02 и остаётся `QA-03`. Evidence: [evidence/FEAT-02.md](evidence/FEAT-02.md).

`API-03` проверена 14 июля 2026 года TDD и ASP.NET HTTP integration matrix на disposable PostgreSQL 17. Десять body-bound admin routes принимают JSON; news/schedule form routes принимают обязательные ISO dates, invalid/missing dates дают unified `400` validation response. Validation, authentication/authorization, not found, unsupported media и server failure возвращают один `application/problem+json` shape; generic `500` не раскрывает exception detail. Nullable Home schedule/video возвращают `200` с JSON `null`; schedule list содержит `status`; nullable media/Home contracts совпадают с TypeScript. Targeted contract tests: 26/26; full backend tests: 47/47; frontend build green. Full ESLint сохраняет baseline 5 errors/13 warnings и остаётся `QA-03`. Evidence: [evidence/API-03.md](evidence/API-03.md).

`DATA-04` проверена 14 июля 2026 года TDD, independent code review и real PostgreSQL 17/Azurite integration tests. Policy hard-capped: JPEG/PNG/WebP до 10 MiB, MP4 до 250 MiB; extension, MIME и signature должны совпадать. Create/replace rollback удаляет новый Blob при DB failure; old Blob удаляется только после DB success; delete сначала меняет DB; post-commit cleanup не отменяется request cancellation. Malformed legacy media URL отклоняется до upload/DB mutation. Blob cleanup failure возвращает exact container/blob identity как объяснимый safe orphan; DB никогда не ссылается на удалённый Blob. Real lifecycle/fault tests: 2/2; full backend suite с ними: 87/87; финальный review: 0 Critical/Important. Evidence: [evidence/DATA-04.md](evidence/DATA-04.md).

`FEAT-03` проверена 14 июля 2026 года browser TDD, controlled WebKit fixtures и ASP.NET contract tests. До изменения error states не имели retry, а Home не предоставлял три semantic section links. Финальная matrix доказала independent loading/empty/error/retry для schedule/news/promotion video, mixed failure isolation и переходы в `/schedule`, `/news`, `/video/section?videoType=Performance`. Frontend build и targeted lint green; Home/API contracts 26/26; filtered backend regression 85/85. Full ESLint сохраняет 4 errors/10 warnings вне FEAT-03 и остаётся `QA-03`. Evidence: [evidence/FEAT-03.md](evidence/FEAT-03.md).

`FEAT-04` проверена 14 июля 2026 года browser TDD, controlled Chromium desktop E2E и real API/PostgreSQL 17/Azurite integration. Pagination valid/empty/out-of-range/invalid, list/detail, create/edit/delete, exact dates, photo create/replace/delete и video attach/reattach/detach подтверждены mutation readback и DB/Blob assertions. Full backend suite: 93/93; frontend build и targeted News lint green; bundle не содержит localhost/mock endpoint. Full ESLint сохраняет 4 errors/8 warnings вне FEAT-04 и остаётся `QA-03`. Evidence: [evidence/FEAT-04.md](evidence/FEAT-04.md).

`FEAT-05` проверена 14 июля 2026 года browser TDD, controlled WebKit desktop E2E и real API/PostgreSQL 17/Azurite integration. List/detail/status/location/map, valid/empty/out-of-range/invalid pagination, create/edit/delete, exact datetime и photo create/public-read/replace/delete подтверждены mutation readback и DB/Blob assertions. Full backend suite: 94/94; frontend build и targeted Schedule lint green; bundle не содержит localhost/test markers. Full ESLint сохраняет 4 errors/6 warnings вне FEAT-05 и остаётся `QA-03`. Evidence: [evidence/FEAT-05.md](evidence/FEAT-05.md).

Во время первого диагностического запуска до исправления precedence существующий ignored `.env` мог направить API к non-local DB/Blob targets. Процесс остановлен после обнаружения; secrets не выводились. Старые remote endpoints затем оказались недоступны и по решению владельца не использовались для `DATA-02`; вопрос реального production content перенесён в `OPEN-002` до `OPS-03`/`M5`.

## Открытые решения

См. [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md). `M1` завершён. `OPEN-003` решён подтверждённой DATA-04 policy; `OPEN-002` относится к выбору источника реального production content перед `OPS-03`/`M5`; следующая задача backlog — `API-04`.

## Что читать перед реализацией

1. Корневые инструкции `AGENTS.md`, если файл существует, и инструкции пользователя текущего сеанса.
2. [01_REQUIREMENTS.md](01_REQUIREMENTS.md), [02_DECISIONS.md](02_DECISIONS.md).
3. Строку задачи в [04_BACKLOG.md](04_BACKLOG.md) и связанный milestone.
4. Релевантные `docs/project-analysis/*` и только затем затрагиваемый код и callers.
5. [06_VALIDATION_PLAN.md](06_VALIDATION_PLAN.md) для required evidence.

## Правило обновления состояния

После каждой принятой задачи обновлять её статус, фактические проверки, новые риски/open items, активный milestone и следующую незаблокированную задачу. Не отмечать задачу завершённой без выполнения её критериев приёмки. Scope/decision меняется только после явного решения владельца и синхронного обновления requirements, decisions, plan, backlog и traceability.
