# DATA-01 — Explicit migration workflow evidence

## Цель

Воспроизводимо создать ожидаемую PostgreSQL schema отдельной EF Core командой, подтвердить безопасный повторный запуск и не применять migrations при API startup.

## Исходное состояние

- Зависимость `BASE-02` имела статус `done`; `DATA-01` имела статус `planned`.
- Infrastructure содержала 24 migrations и актуальный `StoronnimVContextModelSnapshot`, но migration workflow не был проверен на пустой PostgreSQL.
- EF CLI с API startup project завершался ошибкой из-за отсутствующего `Microsoft.EntityFrameworkCore.Design` reference в API.
- EF CLI с Infrastructure как startup project находил migration tooling, но не мог создать context: parameterless `StoronnimVContext` не имел configured provider.
- Startup migrations не вызывались из `Program.cs`; неиспользуемый `DatabaseInitializer.Initialize` не менялся.
- До задачи в worktree находился пользовательский untracked `docs/implementation/.DS_Store`; он не изменялся.

## Затронутые файлы

| Файл | Изменение |
|---|---|
| `.config/dotnet-tools.json` | Зафиксирован local `dotnet-ef` 9.0.7 |
| `backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimVContextFactory.cs` | Добавлен Infrastructure-only design-time factory, читающий только `DB_CLOUD` |
| `docs/implementation/11_MIGRATION_WORKFLOW.md` | Добавлен безопасный explicit migration runbook |
| `backend/README-back.md` | Добавлена ссылка на проверенный workflow |
| `docs/implementation/10_RUNTIME_CONTRACT.md` | Синхронизированы migration project, порядок подготовки и подтверждённый статус |
| `docs/implementation/evidence/DATA-01.md` | Добавлен текущий evidence |
| `docs/implementation/04_BACKLOG.md` | `DATA-01` отмечена `done`; следующая задача обновлена на `DATA-02` |
| `docs/implementation/09_STATE.md` | Зафиксированы фактические проверки, active milestone и следующая задача |
| `docs/implementation/00_INDEX.md` | Добавлены ссылки на runbook и evidence |

Migration classes, model snapshot, runtime startup и package versions приложения не менялись.

## Принятые решения

- Использовать `IDesignTimeDbContextFactory<StoronnimVContext>` в Infrastructure вместо добавления EF Design package в API. Это исключает JWT, CORS, Blob и Hangfire startup dependencies из migration command и не запускает application startup.
- Использовать только `DB_CLOUD`; factory не загружает API `.env`, чтобы target всегда задавался явно в process environment.
- Зафиксировать проверенную CLI-версию local tool manifest. Это устраняет зависимость от заранее установленного global `dotnet-ef`; application packages не обновлялись.
- Проверять на одноразовом PostgreSQL 17 container, доступном только через `127.0.0.1` на случайном порту. Container и обе test DB удалены после проверки.
- Не добавлять отсутствующие unique `Admin.Login` и singleton `GroupPage` invariants: они являются подтверждёнными отдельными рисками и не входят в критерии migration workflow DATA-01.

## Выполненные изменения

1. Добавлен design-time factory с явной ошибкой при отсутствующем или пустом `DB_CLOUD`.
2. Добавлен local tool manifest для `dotnet-ef` 9.0.7 и подтверждён `dotnet tool restore`.
3. Добавлен runbook с предусловиями target/backup, canonical Infrastructure-only command, повторным запуском и schema inspection.
4. На подтверждённо пустой финальной БД canonical command собрала migration project и применила все 24 migrations.
5. Повторная canonical command не применила migrations; history, tables и model snapshot проверены отдельно.
6. Одноразовый PostgreSQL container остановлен и автоматически удалён.

## Среда проверки

- Дата: 13 июля 2026 года.
- OS: macOS 26.5, arm64.
- .NET SDK: 9.0.203.
- Local EF tool: 9.0.7.
- Docker server: 28.0.4.
- PostgreSQL image: локально уже присутствующий `postgres:17`; это evidence текущей проверки, не project pin.
- Target: две одноразовые local DB внутри disposable container; production/staging resources не использовались.

## Выполненные команды и результаты

Во всех EF командах connection string передавался через process environment `DB_CLOUD`; значение не включено в repository или этот evidence.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Исходный API startup path | `dotnet ef migrations list` с Infrastructure project и API startup project | Ошибка: API не reference-ит `Microsoft.EntityFrameworkCore.Design` | 1 | Исходная generic startup-команда не была рабочим migration workflow |
| Исходный Infrastructure path | `dotnet ef migrations list` с Infrastructure как project/startup project | Ошибка: context создан без configured provider | 1 | Нужен design-time context creation path |
| Узкая Infrastructure build | `dotnet build backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj --no-restore --configuration Release --disable-build-servers` | Build succeeded; 0 errors, 3 существующих warnings | 0 | Design-time factory компилируется в затронутом модуле |
| Migration discovery | `dotnet ef migrations list ... --no-connect` через Infrastructure project/startup project | Перечислены все 24 migrations | 0 | Factory позволяет EF CLI создать configured context без API startup |
| Missing target guard | Та же Infrastructure-only discovery при удалённом из process environment `DB_CLOUD` | Явная ошибка `DB_CLOUD is required to run Entity Framework migrations.` | 1 | Команда не выбирает неявный target и останавливается до подключения |
| Local tool restore | `dotnet tool restore` | `dotnet-ef` 9.0.7 восстановлен | 0 | Tooling воспроизводится из repository manifest |
| Tool version | `dotnet ef --version` | Entity Framework Core tools 9.0.7 | 0 | Команда разрешается через зафиксированную версию |
| Пустой final target | Read-only query `information_schema.tables` перед update | 0 public tables | 0 | Финальная проверка началась на пустой БД |
| Canonical database update | `dotnet ef database update --project <Infrastructure.csproj> --startup-project <Infrastructure.csproj> --context StoronnimVContext` | Build succeeded; применены 24 migrations; `Done` | 0 | Пустая PostgreSQL получает schema отдельной командой |
| Повторный update | Та же canonical command без изменения target | `No migrations were applied. The database is already up to date.` | 0 | Повторный запуск безопасен и idempotent |
| Migration history | Read-only query `__EFMigrationsHistory` | 24 rows; первая `20241125211724_Initial`, последняя `20250501144418_AddGroupSocials` | 0 | Все repository migrations зарегистрированы |
| Schema tables | Read-only query `information_schema.tables` | 9 application tables и `__EFMigrationsHistory` | 0 | Создан ожидаемый набор таблиц snapshot |
| Model drift | `dotnet ef migrations has-pending-model-changes` через Infrastructure project/startup project | `No changes have been made to the model since the last migration.` | 0 | Текущая EF model соответствует последней migration snapshot |
| Clean solution restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --artifacts-path /tmp/storonnimv-data01-final-019f58c2/artifacts --disable-build-servers` | 0 errors, 2 существующих ImageSharp vulnerability warnings | 0 | Backend dependencies восстанавливаются в новом artifacts path |
| Solution Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path /tmp/storonnimv-data01-final-019f58c2/artifacts --disable-build-servers` | Build succeeded; 0 errors, 8 существующих warnings | 0 | Изменение не ломает solution |
| Backend test gate | `dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --no-build --configuration Release --artifacts-path /tmp/storonnimv-data01-final-019f58c2/artifacts --disable-build-servers` | Test runner завершился; доступных тестов нет | 0 | Test command выполняется, behavioral coverage отсутствует |
| Disposable cleanup | `docker stop storonnimv-data01-019f58c2`, затем `docker ps -a` по exact name | Container остановлен, итоговый список пуст | 0 | Временная DB не оставлена запущенной или сохранённой |
| Startup migration scan | Поиск `DatabaseInitializer`, `Database.Migrate`, `EnsureCreated`, `MigrateAsync` в `Program.cs` | Совпадений нет | 1 | API startup не применяет schema автоматически |
| Tool manifest syntax | `jq empty .config/dotnet-tools.json` | JSON валиден | 0 | Local tool manifest синтаксически корректен |
| Whitespace validation | `git diff --check` и trailing-whitespace scan всех изменённых/new файлов | Ошибок и совпадений нет | 0 / 1 | Итоговые изменения не содержат whitespace defects |
| Secret scan | Regex scan всех изменённых/new файлов и отдельно добавленных diff lines | Реальных private keys, connection strings, access/API keys и assigned passwords не найдено | 1 | Task diff не содержит распознанных secrets; существующий `local-only-change-me` остаётся безопасным placeholder |
| Итоговый scope review | `git status --short --untracked-files=all`, полный diff и чтение всех new файлов | 9 файлов DATA-01 и исходный пользовательский `.DS_Store`; commit не создан, следующая задача не начата | 0 | Изменения ограничены DATA-01, пользовательский файл сохранён |

Первый sandboxed `dotnet tool restore` завершился `FatalProtocolException` из-за sandbox network/domain restriction. Обязательная команда повторена вне sandbox и завершилась exit 0. Две ранние sandboxed build/EF попытки не дали полного MSBuild результата; все финальные обязательные проверки выполнены вне sandbox с явными exit codes.

## Невыполненные проверки

- Production/staging migration rehearsal, backup и rollback не выполнялись: production access запрещён, а rehearsal относится к `OPS-03`.
- Restore существующих PostgreSQL/Blob данных не выполнялся: это `DATA-02`.
- API startup, Hangfire, `/health` и OpenAPI не запускались: это `BASE-03`; design-time factory намеренно исключает их из migration workflow.
- Frontend и browser checks не запускались: frontend не затронут.
- Behavioral backend tests отсутствуют в test assembly; test runner был выполнен, но покрытие не получено.

## Проблемы вне scope

- `Admin.Login` не имеет unique index; read-only inspection вернул 0 соответствующих unique indexes.
- `GroupPages` имеет только primary key и не содержит singleton constraint; enforcement запланирован отдельной feature-задачей.
- `SixLabors.ImageSharp` 3.1.6 продолжает выдавать `NU1902`/`NU1903`; package не обновлялся, поскольку это не требуется для DATA-01.
- Существующие compiler warnings `CS8981`, `CS8602`, `CS8618`, `CS8629` не блокируют build и не исправлялись.
- PostgreSQL server version всё ещё не закреплена проектом; PostgreSQL 17 подтверждена только как текущая test environment.
- Пользовательский `docs/implementation/.DS_Store` остаётся untracked и не изменялся.

Эти проблемы не блокируют criteria DATA-01: repository migrations полностью применяются к пустой PostgreSQL, повторный запуск безопасен, schema и model snapshot согласованы, а startup migrations не добавлены.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Пустая БД получает ожидаемую schema | Выполнен | 0 исходных public tables; 24 migrations применены; получены 9 application tables и history table |
| Повторный запуск безопасен | Выполнен | Вторая canonical command завершилась exit 0 и не применила migrations |
| Migrations выполняются отдельной командой | Выполнен | Infrastructure-only factory и runbook; API startup не вызывается и не менялся |
| Schema соответствует текущей модели | Выполнен | History содержит все 24 migrations; pending model changes отсутствуют |
| Production ресурсы и secrets не затронуты | Выполнен | Использованы только disposable local DB; connection string не включён в tracked files |

Все критерии приёмки DATA-01 выполнены. Статус задачи установлен `done`. Следующая задача по backlog — `DATA-02`; она не начиналась.
