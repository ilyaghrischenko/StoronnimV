# BASE-03 — Local API startup evidence

## Цель

Доказать локальный запуск ASP.NET Core API с безопасными local dependencies, доступность `/health` и Development OpenAPI, а также понятную остановку при отсутствующей обязательной конфигурации.

## Исходное состояние

- `BASE-03` имела статус `planned`; её единственная зависимость `DATA-01` имела статус `done`.
- Clean backend build и explicit migration workflow уже были доказаны в `BASE-02` и `DATA-01`.
- Startup API, Hangfire, PostgreSQL health check и OpenAPI ранее не запускались.
- `Program.cs` загружал `.env` с default DotNetEnv precedence, при котором файл перезаписывал явно переданные process variables.
- В worktree до задачи находились пользовательские untracked artifacts `DATA-02`; они не изменялись.

## Scope и затронутые файлы

| Файл | Изменение |
|---|---|
| `backend/StoronnimV.Server/StoronnimV.Api/Program.cs` | `.env` больше не перезаписывает явно заданные process variables |
| `docs/implementation/10_RUNTIME_CONTRACT.md` | Зафиксированы проверенные startup endpoints, precedence и команда запуска |
| `docs/implementation/evidence/BASE-03.md` | Добавлен текущий evidence |
| `docs/implementation/04_BACKLOG.md` | `BASE-03` отмечена `done` |
| `docs/implementation/09_STATE.md` | Зафиксированы проверки, факты и следующая planned task |
| `docs/implementation/00_INDEX.md` | Добавлена ссылка на evidence |

Frontend, feature logic, auth middleware, Hangfire dashboard policy, migrations, package versions и пользовательские `DATA-02` artifacts не менялись.

## Решение и выполненное изменение

DotNetEnv сохранён как local fallback, но `LoadOptions` получил `clobberExistingVars: false`. Явные process variables теперь имеют приоритет над `.env`. Это минимально устраняет blocker канонического `dotnet run`: local/CI invocation может доказуемо выбрать отдельные resources, не используя неподтверждённые targets из ignored файла.

Финальная проверка API выполнялась на одноразовой PostgreSQL 17 с `POSTGRES_HOST_AUTH_METHOD=trust`, bind только к `127.0.0.1` на случайном host port и без persistent volume. Existing EF migrations были применены отдельной командой до startup. Blob operations не вызывались; `BLOB_STORAGE=UseDevelopmentStorage=true` использовался только как local non-production configuration value.

## Выполненные команды и результаты

Секретные значения не приводятся. Local connection string не содержал password; JWT key был одноразовым local-only test value.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Dependency status | Чтение `04_BACKLOG.md` | `DATA-01=done`, `BASE-03=planned` до реализации | 0 | Зависимость завершена, задача может выполняться |
| Disposable PostgreSQL | `docker run --rm ... postgres:17`, localhost random port; `pg_isready` | PostgreSQL 17 принимает connections | 0 | Используется отдельная local dependency без production data |
| Local schema | `dotnet ef database update` через Infrastructure-only workflow | Применены все 24 migrations | 0 | API получает ожидаемую schema; migrations не выполняются startup-кодом |
| Узкая Debug build | `dotnet build .../StoronnimV.Api.csproj --no-restore --configuration Debug --disable-build-servers` | Build succeeded; 0 errors, 1 существующий vulnerability warning в финальном повторе | 0 | Изменённый startup code компилируется |
| Canonical startup | Явные local process variables + `dotnet run --no-build --project .../StoronnimV.Api.csproj --no-launch-profile` | Kestrel слушает `127.0.0.1`; Development; Hangfire использует одноразовую localhost PostgreSQL | controlled shutdown 0 | `dotnet run` стартует на явно выбранных local dependencies; `.env` их не перезаписывает |
| Health | `GET /health` | `200`; overall `Healthy`; `API=Healthy`; `PostgresSQL=Healthy` | 0 | API отвечает и PostgreSQL доступна через runtime health check |
| OpenAPI JSON | `GET /openapi/v1.json`, затем `jq` | `200`; OpenAPI `3.0.1`; 49 paths | 0 | Development OpenAPI сгенерирован и является валидным JSON |
| Swagger UI | `GET /swagger/index.html` | `200`; HTML response получен | 0 | Development Swagger UI доступен |
| Missing environment | Изолированный `env -i` запуск актуального DLL без `.env` и `DB_CLOUD` | Явная `EnvVariableNotFoundException: Environment variable not found: DB_CLOUD` | 134 | Отсутствующая обязательная startup variable не маскируется и названа в ошибке |
| Clean solution restore | `dotnet restore ...StoronnimV.Server.sln --no-cache --artifacts-path /tmp/storonnimv-base03-final-019f58e7/artifacts --disable-build-servers` | 0 errors; 2 существующих ImageSharp vulnerability warnings | 0 | Backend dependencies восстанавливаются в новом artifacts path |
| Solution Release build | `dotnet build ...StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path ... --disable-build-servers` | Build succeeded; 0 errors; 8 существующих warnings | 0 | Изменение не ломает backend solution |
| API Release build | `dotnet build ...StoronnimV.Api.csproj --no-restore --configuration Release --artifacts-path ... --disable-build-servers` | Build succeeded; 0 errors; 2 существующих ImageSharp warnings | 0 | Startup project отдельно собирается в Release |
| Backend test gate | `dotnet test ...StoronnimV.Server.sln --no-restore --no-build --configuration Release --artifacts-path ... --disable-build-servers` | Test runner завершился; доступных tests нет | 0 | Test command выполняется; behavioral test coverage отсутствует |
| Disposable cleanup | `docker stop`, затем exact-name `docker ps -a` | Контейнер остановлен и удалён; итоговый список пуст | 0 | Временная DB не оставлена |

Две sandboxed Debug build попытки зависли без MSBuild output; оба exact build processes были остановлены, а обязательные сборки повторены вне sandbox и прошли. Sandboxed localhost EF/HTTP requests были отклонены окружением; те же обязательные проверки повторены вне sandbox и прошли.

## Невыполненные проверки

- Blob upload/read/delete не выполнялись: media behavior не входит в `BASE-03`, а local emulator workflow не утверждён.
- Public/admin feature endpoints и authentication не проверялись: это последующие `QA-01`, `API-01` и feature tasks.
- Production/staging startup не выполнялся и не требуется для M1.
- Behavioral backend tests отсутствуют в test assembly; test runner выполнен, но coverage не получено.

## Проблемы вне scope

- Первый диагностический `dotnet run` до исправления precedence подхватил существующий ignored `.env`. После обнаружения процессы были остановлены; secrets и target values не выводились. Безопасная классификация показала, что DB/Blob targets не local либо имеют нераспознанный формат. Подключение и возможный Hangfire startup side effect удалённого target не проверялись, поскольку remote access запрещён. Перед дальнейшим использованием этих targets нужен явно разрешённый owner audit. Финальный evidence получен заново только на одноразовой local PostgreSQL.
- `SixLabors.ImageSharp` 3.1.6 продолжает выдавать `NU1902`/`NU1903`; package не обновлялся, поскольку это не startup blocker `BASE-03`.
- Существующие compiler warnings `CS8981`, `CS8602`, `CS8618`, `CS8629` не блокируют build и не исправлялись.
- Test assembly не содержит tests.
- При HTTP-only local smoke `UseHttpsRedirection` записал warning о невозможности определить HTTPS port; проверяемые endpoints ответили `200`, поэтому warning не блокирует критерии.
- Пользовательские untracked `DATA-02` artifacts сохранены без изменений и не оценивались как завершение `DATA-02`.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| API стартует | Выполнен | Канонический `dotnet run`; Kestrel и Hangfire стартовали на explicit local configuration; controlled shutdown exit 0 |
| `/health` отвечает | Выполнен | `200`; API и PostgreSQL entries `Healthy` |
| Development OpenAPI отвечает | Выполнен | `/openapi/v1.json` и `/swagger/index.html` вернули `200`; JSON валиден |
| Missing env даёт понятную ошибку | Выполнен | Изолированный запуск назвал отсутствующий `DB_CLOUD` и завершился до startup |
| Startup blocker устранён минимально | Выполнен | Process environment больше не перезаписывается `.env`; feature defects не менялись |
| Local dependencies очищены | Выполнен | Одноразовый PostgreSQL container удалён |

Все критерии приёмки `BASE-03` выполнены. Статус задачи установлен `done`. Следующая planned task по backlog — `DATA-02`; она не начиналась в рамках `BASE-03`.
