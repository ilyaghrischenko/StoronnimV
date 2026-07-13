# Состояние проекта для будущих сеансов

## Текущая цель

Выполнить утверждённый план завершения StoronnimV, начиная с воспроизводимого локального запуска. `BASE-01`, `BASE-02`, `DATA-01` и `BASE-03` завершены: runtime contract зафиксирован, clean backend restore/build доказаны, explicit migration workflow проверен на пустой локальной PostgreSQL, local API startup подтверждён вместе с health и Development OpenAPI.

## Утверждённый объём

- Public: Home, Schedule, News, Music, Group, Video, Footer/socials, Error и пустая static Developers page.
- Admin: полный content/media CRUD и SuperAdmin management Basic Admin accounts.
- Devices: mobile, tablet и desktop для public/admin.
- Data: существующие PostgreSQL/Azure Blob данные после backup/inventory.
- Operations: Hangfire status job; production dashboard disabled; explicit migrations; последующий deployment.

## Исключено

Analytics, contact/booking forms, commerce/tickets, search, multilingual UI, новая admin dashboard, automatic startup migrations, public Hangfire dashboard и новая logging strategy.

## Активный milestone

`M1 — Воспроизводимый локальный запуск`.

## Следующая задача

`DATA-02 — Получить безопасную копию контента и media`. Зависимость `DATA-01` завершена; доступность backup и разрешение на чтение остаются внешним пунктом `OPEN-002`. В worktree существуют пользовательские untracked artifacts с именами `DATA-02`; `BASE-03` их не изменяла и не оценивала как завершённую задачу.

## Ключевые ограничения

- Читать актуальный root/user `AGENTS.md` перед каждой задачей.
- Выполнять только одну backlog task или явно согласованный связанный набор.
- Не менять production DB/Blob до backup и authorization.
- Не хранить secrets/credentials в Git, logs или документации.
- Сохранять React/.NET/PostgreSQL/Azure/Hangfire architecture.
- Desktop visual baseline — runtime `style.css`; mobile frame можно упрощать.
- Migrations только отдельной командой; SuperAdmin вручную.

## Команды проверки

Канонический runtime contract: [10_RUNTIME_CONTRACT.md](10_RUNTIME_CONTRACT.md). Evidence: [evidence/BASE-02.md](evidence/BASE-02.md), [evidence/DATA-01.md](evidence/DATA-01.md), [evidence/BASE-03.md](evidence/BASE-03.md).

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

Во время первого диагностического запуска до исправления precedence существующий ignored `.env` мог направить API к non-local DB/Blob targets. Процесс остановлен после обнаружения; secrets не выводились. Возможное подключение или Hangfire startup side effect не проверялись из-за запрета remote access. Перед использованием этих targets требуется отдельное явное разрешение владельца на audit.

## Открытые решения

См. [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md). Первый milestone потенциально зависит от доступа к backup/content; следующая задача по backlog — `DATA-02`, для которой остаётся внешний пункт `OPEN-002`.

## Что читать перед реализацией

1. Корневые инструкции `AGENTS.md`, если файл существует, и инструкции пользователя текущего сеанса.
2. [01_REQUIREMENTS.md](01_REQUIREMENTS.md), [02_DECISIONS.md](02_DECISIONS.md).
3. Строку задачи в [04_BACKLOG.md](04_BACKLOG.md) и связанный milestone.
4. Релевантные `docs/project-analysis/*` и только затем затрагиваемый код и callers.
5. [06_VALIDATION_PLAN.md](06_VALIDATION_PLAN.md) для required evidence.

## Правило обновления состояния

После каждой принятой задачи обновлять её статус, фактические проверки, новые риски/open items, активный milestone и следующую незаблокированную задачу. Не отмечать задачу завершённой без выполнения её критериев приёмки. Scope/decision меняется только после явного решения владельца и синхронного обновления requirements, decisions, plan, backlog и traceability.
