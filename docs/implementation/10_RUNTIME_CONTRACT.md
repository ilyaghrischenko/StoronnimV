# Runtime contract локального окружения

## 1. Назначение и границы

Это канонический документ для подготовки локального окружения StoronnimV. Он фиксирует только требования, имена параметров и текущее поведение, подтверждённые manifest-файлами, конфигурацией и кодом репозитория.

Production topology здесь не выбирается, production credentials не приводятся. Чистая backend-сборка доказана в `BASE-02`; local API startup, health и Development OpenAPI доказаны в `BASE-03`; подключение frontend через environment API URL относится к `BASE-04`, применение migrations — к `DATA-01`, восстановление существующих данных и media — к `DATA-02`.

## 2. Структура проекта

| Назначение | Путь |
|---|---|
| Solution | `backend/StoronnimV.Server/StoronnimV.Server.sln` |
| Backend startup project | `backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj` |
| EF migration project | `backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj`; migrations are under `StoronnimV.Infrastructure/Migrations`; explicit command is documented in [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md) |
| Frontend project | `frontend/storonnimv.client` |
| Frontend package manifest/lock | `frontend/storonnimv.client/package.json`, `frontend/storonnimv.client/package-lock.json` |
| Backend base/development config | `StoronnimV.Api/appsettings.json`, `StoronnimV.Api/appsettings.Development.json` |
| Backend local environment file | `StoronnimV.Api/.env` (не публикуется; шаблон — `.env.example`) |
| Backend launch profiles | `StoronnimV.Api/Properties/launchSettings.json` |

Package manager frontend — npm. Основные runtime services — ASP.NET Core API, React/TypeScript/Vite client, PostgreSQL (EF Core, Hangfire и health check используют один connection string) и Azure Blob Storage для media.

## 3. Runtime matrix

| Компонент | Требование | Версия или диапазон | Доказательство | Уверенность | Примечание |
|---|---|---|---|---|---|
| .NET SDK | SDK, способный собирать `net9.0` | major `9`; точный feature/patch не закреплён | **Подтверждено manifest-файлом:** все `.csproj` содержат `TargetFramework=net9.0`; Docker build image `dotnet/sdk:9.0`; `global.json` отсутствует | Высокая для major; точная версия неизвестна | Установленная локально версия не является требованием проекта |
| .NET runtime | ASP.NET Core/.NET runtime | `9.0` major; точный patch не закреплён | **Подтверждено manifest-файлом:** `StoronnimV.Api.csproj`; Docker runtime image `dotnet/aspnet:9.0` | Высокая для major | `RuntimeIdentifier` не задан; Docker target — Linux |
| Node.js | Runtime, поддерживаемый зафиксированным Vite | `^18.0.0 || ^20.0.0 || >=22.0.0` | **Подтверждено manifest-файлом:** lock-файл фиксирует Vite `6.0.7`, его `engines.node` содержит этот диапазон | Средняя | Корневые `engines`, `.nvmrc`, `.node-version` и Volta отсутствуют; это транзитивно подтверждённый минимум, не отдельно выбранная версия проекта |
| npm | Package manager | точная версия не закреплена | **Подтверждено manifest-файлом:** `package-lock.json` формата lockfile v3; npm scripts в `package.json` | Высокая для manager; версия неизвестна | `packageManager` отсутствует; yarn/pnpm lock отсутствуют |
| PostgreSQL | Обязательный локальный service | точная server version не закреплена | **Подтверждено кодом и manifest-файлом:** Npgsql EF provider `9.0.1`; `UseNpgsql`; Hangfire PostgreSQL; Npgsql health check | Высокая для service; версия неизвестна | Один `DB_CLOUD` используется приложением, Hangfire и health check |
| Azure Blob Storage | Обязательная media dependency для Blob operations | Azure Storage API; emulator/version не закреплены | **Подтверждено кодом и manifest-файлом:** `Azure.Storage.Blobs` `12.23.0`; `BlobServiceClient(BLOB_STORAGE)` | Высокая для Azure Blob; локальный вариант неизвестен | Azurite/Docker Compose/devcontainer в репозитории не подтверждены; не считать их поддержанными до отдельной проверки |
| Hangfire storage | PostgreSQL через тот же connection string | Hangfire `1.8.16`, Hangfire.PostgreSql `1.20.10` | **Подтверждено кодом и manifest-файлом:** package references и `UsePostgreSqlStorage(...DB_CLOUD...)` | Высокая | Hangfire server и recurring job регистрируются при startup |
| Git | Нужен для получения и проверки рабочей копии | точная версия не закреплена | Git repository и dry-run workflow BASE-01 | Высокая для процесса | Не является application runtime |
| Docker | Не подтверждён как обязательный локальный инструмент | неизвестно | Dockerfile существует, но Compose/service workflow отсутствует | Низкая/неизвестно | Docker — доступный artifact, а не обязательный BASE-01 prerequisite |

В `BASE-02` из `StoronnimV.Infrastructure.csproj` удалён дублирующий Windows-only `HintPath` к `Microsoft.Extensions.Configuration.dll`. Переносимая зависимость остаётся закреплена существующим `PackageReference` версии `9.0.0`; package version и target framework не менялись. Clean restore/build подтверждены на macOS 26.5 arm64 с .NET SDK 9.0.203; это evidence текущей проверки, а не новый project pin.

## 4. Локальная топология

- Vite dev server по умолчанию использует `http://localhost:5173`; собственный `server.port` или proxy в `vite.config.ts` не заданы.
- Frontend сейчас игнорирует `.env.production` и обращается к `https://localhost:44315/api`, жёстко заданному в `GlobalContext.tsx`. Это текущее поведение; `VITE_API_URL` будет реализован только в `BASE-04`.
- Backend HTTPS launch profile объявляет `https://localhost:44315` и `http://localhost:5268`; HTTP profile — `http://localhost:5269`. Это launch-profile endpoints, а не доказательство startup.
- `CLIENT_URL` задаёт единственный разрешённый CORS origin. Для стандартного Vite dev server безопасный локальный пример — `http://localhost:5173`.
- PostgreSQL доступен через `DB_CLOUD`; тот же URL используют EF Core, Hangfire storage и PostgreSQL health check. Регистрация Hangfire server/job означает, что PostgreSQL требуется для полноценного startup, проверяемого в `BASE-03`.
- Blob operations используют `BLOB_STORAGE` и containers `storonnimv-photo`/`storonnimv-video`. Repository создаёт container при upload. Доступность account, ACL и безопасного emulator workflow не доказаны.
- Health endpoint: `/health`. В `BASE-03` подтверждены `200 OK` и healthy API/PostgreSQL checks, OpenAPI JSON на `/openapi/v1.json` и Swagger UI на `/swagger/index.html` в Development.
- Hangfire dashboard сейчас маппится без environment gate. Это факт текущего кода, не утверждение о допустимой production topology; исправление отложено до `API-04`.
- `appsettings.Development.json` переопределяет только logging levels. Cookie и rate-limit settings наследуются из `appsettings.json`.

## 5. Environment matrix

Environment variables поступают из process environment. Дополнительно [backend startup code](../../backend/StoronnimV.Server/StoronnimV.Api/Program.cs) через уже подключённый пакет DotNetEnv вызывает `Env.Load` с `onlyExactPath: true`, если в текущей рабочей директории существует точный файл `.env`. Process environment имеет приоритет: `.env` заполняет только отсутствующие variables и не перезаписывает явно переданные local/CI values. Поэтому инструкция копирования действует при запуске из `backend/StoronnimV.Server/StoronnimV.Api`; наличие одного `.env.example` ничего не загружает. Для обычной .NET configuration вложенные имена ниже могут задаваться через `__`. Строки с прямыми environment reads ниже относятся к категории **Подтверждено кодом**; строки options/framework — **Подтверждено конфигурацией** и, где указан binder/framework consumer, также подтверждены кодом платформы. Значения в колонке local example — категория **Безопасный локальный пример**.

| Имя | Компонент | Обязательно | Где читается | Формат | Безопасный local example | Secret | Поведение при отсутствии |
|---|---|---:|---|---|---|---:|---|
| `DB_CLOUD` | API/EF/Hangfire/health | да | `AddDbContext`, `AddHangfire`, `AddHealthChecks` | Npgsql connection string | `Host=localhost;Port=5432;Database=storonnimv_local;Username=storonnimv_local;Password=local-only-change-me` | да | startup configuration бросает исключение |
| `BLOB_STORAGE` | Blob repository | для media operations | `BlobRepository` | Azure Storage connection string | `DefaultEndpointsProtocol=https;AccountName=<development-account>;AccountKey=<replace-with-development-key>;EndpointSuffix=core.windows.net` | да | Blob client получает `null`; media resolution/use завершится ошибкой |
| `TOKEN_ISSUER` | JWT | да | `AddOptions`, `AddJwtBearer` | непустая issuer string/URI | `https://localhost:44315` | нет | startup configuration бросает исключение |
| `TOKEN_AUDIENCE` | JWT | да | `AddOptions`, `AddJwtBearer` | непустая audience string/URI | `http://localhost:5173` | нет | startup configuration бросает исключение |
| `TOKEN_KEY` | JWT signing | да | `AddOptions`, `AddJwtBearer` | HMAC key string | `local-only-change-this-signing-key-32chars` | да | startup configuration бросает исключение |
| `TOKEN_LIFETIME` | JWT | да | `AddOptions`, `AddJwtBearer`, `JwtBearerService` | integer, days | `1` | нет | startup configuration бросает исключение; non-integer fails parsing |
| `CLIENT_URL` | CORS | да | `AddCors` | one origin, without path | `http://localhost:5173` | нет | startup configuration бросает исключение |
| `DOMAIN` | auth cookie | да для logout; login допускает null | account/admin controller services | cookie domain string; empty process value is accepted by current guard | empty value for local host-only intent | нет | logout path throws; login creates cookie without Domain |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core | нет | framework/launch profile | environment name | `Development` | нет | framework default environment applies; Development OpenAPI block is disabled |
| `CookieOptions__HttpOnly` | auth cookie | да, если base config не используется | options binding | boolean | `true` | нет | base `appsettings.json` supplies it; options validation otherwise fails |
| `CookieOptions__Secure` | auth cookie | да, если base config не используется | options binding | boolean | `true` | нет | base config supplies it; options validation otherwise fails |
| `CookieOptions__SameSite` | auth cookie | да, если base config не используется | options binding/controllers | `SameSiteMode` name | `None` | нет | base config supplies it; missing/invalid value fails validation or parsing |
| `CookieOptions__ExpiresInHours` | auth cookie | да, если base config не используется | options binding/controllers | integer hours | `1` | нет | base config supplies it |
| `RateLimiterOptions__StatusCode` | rate limiting | да, если base config не используется | `AddRateLimiter` | HTTP status integer | `429` | нет | base config supplies it; missing section throws |
| `RateLimiterOptions__Policies__0__PolicyName` | rate limiting | да, если overriding | `AddRateLimiter` | string | `UserLimitPerMinute` | нет | base config supplies it |
| `RateLimiterOptions__Policies__0__Limit` | rate limiting | да, если overriding | `AddRateLimiter` | positive integer | `60` | нет | base config supplies it |
| `RateLimiterOptions__Policies__0__Expiration` | rate limiting | да, если overriding | `AddRateLimiter` | `TimeSpan` | `00:01:00` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__PolicyName` | rate limiting | да, если overriding | `AddRateLimiter` | string | `AdminLimitPerMinute` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__Limit` | rate limiting | да, если overriding | `AddRateLimiter` | positive integer | `100` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__Expiration` | rate limiting | да, если overriding | `AddRateLimiter` | `TimeSpan` | `00:01:00` | нет | base config supplies it |
| `Logging__LogLevel__Default` | framework logging | нет | current appsettings/framework | log level name | `Information` | нет | framework default/config precedence applies |
| `Logging__LogLevel__Microsoft.AspNetCore` | framework logging | нет | current appsettings/framework | log level name | `Warning` | нет | framework default/config precedence applies |
| `AllowedHosts` | ASP.NET Core host filtering | нет | current appsettings/framework | semicolon-delimited hosts or `*` | `localhost` | нет | framework/config default applies |

`DB_LOCAL_ILYA` и `DB_LOCAL_DIMA` присутствуют только в игнорируемом локальном `.env`, но код их не читает, поэтому они не входят в contract. `VITE_API_URL` присутствует в tracked `.env.production`, но frontend его не читает; это **отложено** до `BASE-04`, а не действующая переменная.

## 6. Безопасные локальные примеры

Скопируйте `backend/StoronnimV.Server/StoronnimV.Api/.env.example` в непубликуемый `.env` рядом с `Program.cs` и замените все `<...>`/`local-only-...` значения. Локальный пароль PostgreSQL и JWT key — только placeholders; не используйте их вне локального окружения. Для `BLOB_STORAGE` нужен отдельный non-production Azure Storage connection string. Репозиторий пока не подтверждает Azurite, поэтому emulator connection string здесь намеренно не приводится.

Шаблон фиксирует синтаксис и имена. Build доказан в `BASE-02`; startup с отдельной local PostgreSQL, `/health` и Development OpenAPI доказаны в `BASE-03`. Не копируйте production DB/Blob credentials в `.env`.

## 7. Порядок подготовки окружения

1. Из корня репозитория выполнить dry-run команды из раздела 8 и сверить major/range, не устанавливая dependencies.
2. Подготовить отдельный локальный PostgreSQL и создать пустые local database/user; schema создаётся только по [явному migration workflow](11_MIGRATION_WORKFLOW.md).
3. Получить отдельный non-production Azure Storage account/connection string. Если нужен Azurite, сначала подтвердить его workflow отдельной задачей; текущий репозиторий этого не делает.
4. Из каталога `backend/StoronnimV.Server/StoronnimV.Api` создать непубликуемый `.env` по `.env.example`, заменить placeholders и сохранить реальные secrets только локально.
5. Не создавать frontend `.env` для API URL: текущий client всё равно использует hardcoded `https://localhost:44315/api`; изменение относится к `BASE-04`.
6. Проверить точные имена, working directory и launch endpoints по таблицам выше. Не запускать DB/Blob restore или production services.
7. Для повторения clean backend build использовать команды ниже. Migrations выполнять только отдельной командой из [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md).
8. После подготовки local PostgreSQL schema и process environment запустить API из корня repository: `dotnet run --project backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-launch-profile`. Проверить `/health`, `/openapi/v1.json` и `/swagger/index.html`; не направлять команду на неподтверждённые DB/Blob targets.

```bash
dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache
dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release
dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-restore --configuration Release
```

## 8. Dry-run проверки

Команды безопасны: они показывают metadata/tool versions, не устанавливают dependencies, не выполняют restore/build/run и не обращаются к DB/Blob.

| Команда (из корня repo) | Что проверяет | Ожидаемый тип результата | Требование или характеристика машины |
|---|---|---|---|
| `dotnet --info` | наличие SDK/runtime и platform | сведения о .NET 9 SDK/runtime либо явное отсутствие | Локальный результат; project требует major 9, не конкретный показанный patch |
| `dotnet --list-sdks` | доступные SDK | хотя бы один SDK `9.x` | Наличие major 9 требуется; список patch — характеристика машины |
| `node --version` | наличие Node | версия, входящая в Vite range | Проверяется против lock-file range |
| `npm --version` | наличие npm | номер версии | npm требуется; точная версия — характеристика машины |
| `git status --short` | состояние рабочей копии | пустой вывод или осознанный список изменений | Процессная проверка, не runtime requirement |

`psql`, Docker, Azure CLI и Azurite CLI не включены: репозиторий не фиксирует их как обязательный toolchain BASE-01.

## 9. Известные ограничения и отложенные проверки

- Backend clean restore/build доказан в `BASE-02`; Windows-specific `HintPath` удалён. Полные команды и результаты: [evidence/BASE-02.md](evidence/BASE-02.md).
- Реальный API startup, `/health`, OpenAPI/Swagger и Hangfire registration не доказаны (`BASE-03`).
- Все 24 migrations применены к пустой локальной PostgreSQL и повторный запуск не изменил schema; команды и ограничения зафиксированы в [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md). Production/staging rehearsal остаётся в `OPS-03`.
- Реальные PostgreSQL/Blob данные не восстановлены и production resources не проверялись (`DATA-02`).
- Frontend API URL остаётся hardcoded до `BASE-04`; `VITE_API_URL` сейчас не действует.
- Точные .NET SDK patch, npm и PostgreSQL server versions неизвестны. Node фиксируется только диапазоном transitive Vite engine.
- Azurite, Docker Compose/devcontainer и локальный Blob emulator workflow не подтверждены.
- Azure Storage account/container ACL и доступ к non-production Blob resource неизвестны.
- Production hosting/topology не выбраны; Hangfire dashboard production gate ещё не реализован.
- Локальные версии инструментов ниже являются evidence текущей машины, а не project pins: `.NET SDK 9.0.203`, Node `v25.6.1`, npm `11.12.0` (проверено 12 июля 2026 года).
