# Runtime contract локального окружения

## 1. Назначение и границы

Это канонический документ для подготовки локального окружения StoronnimV. Он фиксирует только требования, имена параметров и текущее поведение, подтверждённые manifest-файлами, конфигурацией и кодом репозитория.

Production topology здесь не выбирается, production credentials не приводятся. Чистая backend-сборка доказана в `BASE-02`; local API startup, health и Development OpenAPI доказаны в `BASE-03`; frontend подключён через валидируемый environment API URL в `BASE-04`; применение migrations доказано в `DATA-01`; локальные PostgreSQL backup/restore и Azurite media copy доказаны в `DATA-02`; upload policy и DB/Blob lifecycle доказаны в `DATA-04`. Реальный production content отложен до `OPS-03`/`M5`.

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
| Azure Blob Storage | Обязательная media dependency для Blob operations | Azure Storage API; emulator/version не закреплены | **Подтверждено кодом и runtime:** `Azure.Storage.Blobs` `12.23.0`; `BlobServiceClient(BLOB_STORAGE)`; `DATA-02` выполнила list/download/upload/public-read на Azurite | Высокая | Azurite подтверждён как локальный test target, но его version не является project pin |
| Hangfire storage | PostgreSQL через тот же connection string | Hangfire `1.8.16`, Hangfire.PostgreSql `1.20.10` | **Подтверждено кодом и manifest-файлом:** package references и `UsePostgreSqlStorage(...DB_CLOUD...)` | Высокая | Hangfire server и recurring job регистрируются при startup |
| Git | Нужен для получения и проверки рабочей копии | точная версия не закреплена | Git repository и dry-run workflow BASE-01 | Высокая для процесса | Не является application runtime |
| Docker | Не подтверждён как обязательный локальный инструмент | неизвестно | Dockerfile существует, но Compose/service workflow отсутствует | Низкая/неизвестно | Docker — доступный artifact, а не обязательный BASE-01 prerequisite |

В `BASE-02` из `StoronnimV.Infrastructure.csproj` удалён дублирующий Windows-only `HintPath` к `Microsoft.Extensions.Configuration.dll`. Переносимая зависимость остаётся закреплена существующим `PackageReference` версии `9.0.0`; package version и target framework не менялись. Clean restore/build подтверждены на macOS 26.5 arm64 с .NET SDK 9.0.203; это evidence текущей проверки, а не новый project pin.

## 4. Локальная топология

- Vite dev server по умолчанию использует `http://localhost:5173`; собственный `server.port` или proxy в `vite.config.ts` не заданы.
- Frontend требует `VITE_API_URL` при запуске Vite и production build. Значение должно быть абсолютным HTTP(S) URL без credentials, query или fragment; завершающие `/` удаляются до встраивания в client bundle.
- Backend HTTPS launch profile объявляет `https://localhost:44315` и `http://localhost:5268`; HTTP profile — `http://localhost:5269`. Это launch-profile endpoints, а не доказательство startup.
- `CLIENT_URL` задаёт единственный разрешённый CORS origin и валидируется как точный absolute HTTP(S) origin без credentials, path, query или fragment. Для стандартного Vite dev server безопасный локальный пример — `http://localhost:5173`; future production environment задаёт свой точный HTTPS origin без выбора hosting provider в коде.
- Browser получает request token через credentialed `GET /api/account/csrf-token`. `GlobalContext.sendRequest` запрашивает свежий token перед каждым unsafe method и передаёт его в `X-CSRF-TOKEN`; API валидирует antiforgery для login и всех unsafe authenticated cookie requests. Bearer-only requests не требуют CSRF token.
- PostgreSQL доступен через `DB_CLOUD`; тот же URL используют EF Core, Hangfire storage и PostgreSQL health check. Регистрация Hangfire server/job означает, что PostgreSQL требуется для полноценного startup, проверяемого в `BASE-03`.
- Blob operations используют `BLOB_STORAGE` и containers `storonnimv-photo`/`storonnimv-video`. Repository создаёт container при upload. `DATA-02` подтвердила безопасный local Azurite workflow с отдельными source/target instances, public `blob` ACL и test media; это не утверждение о production account/ACL.
- `MediaUpload` разрешает JPEG/PNG/WebP до 10 MiB и MP4 до 250 MiB. Size, extension, MIME и magic signature проверяются до Blob upload; configuration может уменьшать лимиты/набор типов, но startup validation не допускает превышение maxima или неподдерживаемые MIME. Multipart/Kestrel body limit равен большему media limit плюс 1 MiB на multipart overhead.
- Health endpoint: `/health`. В `BASE-03` подтверждены `200 OK` и healthy API/PostgreSQL checks, OpenAPI JSON на `/openapi/v1.json` и Swagger UI на `/swagger/index.html` в Development.
- Hangfire dashboard сейчас маппится без environment gate. Это факт текущего кода, не утверждение о допустимой production topology; исправление отложено до `API-04`.
- Base cookie contract: `HttpOnly=true`, `Secure=true`, `SameSite=Lax`, host-only domain. `appsettings.Development.json` переопределяет `Secure=false`, `SameSite=Lax` для HTTP loopback; cross-site HTTPS deployment обязан явно задать `CookieOptions__Secure=true` и `CookieOptions__SameSite=None`. Rate-limit settings наследуются из `appsettings.json`.

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
| `CLIENT_URL` | CORS | да | `AddCors` | exact absolute HTTP(S) origin без credentials/path/query/fragment | `http://localhost:5173` | нет | missing/invalid value останавливает startup |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core | нет | framework/launch profile | environment name | `Development` | нет | framework default environment applies; Development OpenAPI block is disabled |
| `CookieOptions__HttpOnly` | auth cookie | да, если base config не используется | options binding | boolean | `true` | нет | base `appsettings.json` supplies it; options validation otherwise fails |
| `CookieOptions__Secure` | auth + antiforgery cookies | да, если base config не используется | options binding/antiforgery | boolean | `false` только для `Development` HTTP loopback | нет | base config supplies `true`; Development override supplies `false` |
| `CookieOptions__SameSite` | auth + antiforgery cookies | да, если base config не используется | options binding/controllers/antiforgery | `SameSiteMode` name | `Lax` | нет | base config supplies it; missing/invalid value fails startup/validation |
| `CookieOptions__ExpiresInHours` | auth cookie | да, если base config не используется | options binding/controllers | integer hours | `1` | нет | base config supplies it |
| `CookieOptions__Domain` | auth cookie | нет | options binding/controllers | cookie domain; omit for host-only cookie | omitted | нет | login/logout use host-only cookie |
| `RateLimiterOptions__StatusCode` | rate limiting | да, если base config не используется | `AddRateLimiter` | HTTP status integer | `429` | нет | base config supplies it; missing section throws |
| `RateLimiterOptions__Policies__0__PolicyName` | rate limiting | да, если overriding | `AddRateLimiter` | string | `UserLimitPerMinute` | нет | base config supplies it |
| `RateLimiterOptions__Policies__0__Limit` | rate limiting | да, если overriding | `AddRateLimiter` | positive integer | `60` | нет | base config supplies it |
| `RateLimiterOptions__Policies__0__Expiration` | rate limiting | да, если overriding | `AddRateLimiter` | `TimeSpan` | `00:01:00` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__PolicyName` | rate limiting | да, если overriding | `AddRateLimiter` | string | `AdminLimitPerMinute` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__Limit` | rate limiting | да, если overriding | `AddRateLimiter` | positive integer | `100` | нет | base config supplies it |
| `RateLimiterOptions__Policies__1__Expiration` | rate limiting | да, если overriding | `AddRateLimiter` | `TimeSpan` | `00:01:00` | нет | base config supplies it |
| `MediaUpload__MaxPhotoBytes` | photo upload | да, если base config не используется | options binding/validator | integer `1..10485760` | `10485760` | нет | base config supplies 10 MiB; invalid/greater value fails startup |
| `MediaUpload__MaxVideoBytes` | video upload | да, если base config не используется | options binding/validator | integer `1..262144000` | `262144000` | нет | base config supplies 250 MiB; invalid/greater value fails startup |
| `MediaUpload__AllowedPhotoContentTypes__N` | photo upload | да, если overriding array | options binding/validator | indexed values from `image/jpeg`, `image/png`, `image/webp` | base array contains all three | нет | empty/unsupported array fails startup |
| `MediaUpload__AllowedVideoContentTypes__N` | video upload | да, если overriding array | options binding/validator | indexed value `video/mp4` | base array contains `video/mp4` | нет | empty/unsupported array fails startup |
| `Logging__LogLevel__Default` | framework logging | нет | current appsettings/framework | log level name | `Information` | нет | framework default/config precedence applies |
| `Logging__LogLevel__Microsoft.AspNetCore` | framework logging | нет | current appsettings/framework | log level name | `Warning` | нет | framework default/config precedence applies |
| `AllowedHosts` | ASP.NET Core host filtering | нет | current appsettings/framework | semicolon-delimited hosts or `*` | `localhost` | нет | framework/config default applies |
| `VITE_API_URL` | Frontend API base URL | да | Vite config, `GlobalContext` | absolute HTTP(S) URL без credentials/query/fragment; path `/api` включается в значение | `http://localhost:5268/api` | нет | `vite`/build завершается с явной ошибкой до bundling |

`DB_LOCAL_ILYA` и `DB_LOCAL_DIMA` присутствуют только в игнорируемом локальном `.env`, но код их не читает, поэтому они не входят в contract. Frontend local example находится в `frontend/storonnimv.client/.env.example`; tracked `.env.production` задаёт build-time production value без secrets.

## 6. Безопасные локальные примеры

Скопируйте `backend/StoronnimV.Server/StoronnimV.Api/.env.example` в непубликуемый `.env` рядом с `Program.cs` и замените все `<...>`/`local-only-...` значения. Для frontend скопируйте `frontend/storonnimv.client/.env.example` в игнорируемый `.env.local` и при необходимости замените local API endpoint. Локальный пароль PostgreSQL и JWT key — только placeholders; не используйте их вне локального окружения. Для `BLOB_STORAGE` используйте отдельный non-production Azure Storage connection string либо local Azurite connection string по [DATA-02 workflow](12_DATA_COPY_WORKFLOW.md).

Шаблон фиксирует синтаксис и имена. Build доказан в `BASE-02`; startup с отдельной local PostgreSQL, `/health` и Development OpenAPI доказаны в `BASE-03`. Не копируйте production DB/Blob credentials в `.env`.

## 7. Порядок подготовки окружения

1. Из корня репозитория выполнить dry-run команды из раздела 8 и сверить major/range, не устанавливая dependencies.
2. Подготовить отдельный локальный PostgreSQL и создать пустые local database/user; schema создаётся только по [явному migration workflow](11_MIGRATION_WORKFLOW.md).
3. Подготовить отдельный non-production Azure Storage account либо local Azurite по [DATA-02 workflow](12_DATA_COPY_WORKFLOW.md); не использовать production connection string для локального запуска.
4. Из каталога `backend/StoronnimV.Server/StoronnimV.Api` создать непубликуемый `.env` по `.env.example`, заменить placeholders и сохранить реальные secrets только локально.
5. Из `frontend/storonnimv.client` скопировать `.env.example` в `.env.local`; проверить, что `VITE_API_URL` указывает на выбранный local API endpoint и содержит `/api`.
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

`psql`, Docker, Azure CLI и Azurite не включены в обязательный toolchain `BASE-01`; они являются проверенными prerequisites только для воспроизведения `DATA-01`/`DATA-02` workflows.

## 9. Известные ограничения и отложенные проверки

- Backend clean restore/build доказан в `BASE-02`; Windows-specific `HintPath` удалён. Полные команды и результаты: [evidence/BASE-02.md](evidence/BASE-02.md).
- Реальный API startup, `/health`, OpenAPI/Swagger и Hangfire registration не доказаны (`BASE-03`).
- Все 24 migrations применены к пустой локальной PostgreSQL и повторный запуск не изменил schema; команды и ограничения зафиксированы в [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md). Production/staging rehearsal остаётся в `OPS-03`.
- Локальный PostgreSQL/Azurite test corpus скопирован и проверен в `DATA-02`; реальные production data/resources намеренно отложены до `OPS-03`/`M5`.
- `VITE_API_URL` проверяется при dev/build startup; browser-to-local-API request и отсутствие hardcoded `localhost:44315` в production bundle доказаны в `BASE-04`.
- `API-02` доказала local credentialed login/logout topology: exact-origin credentialed CORS, host-only JWT cookie, fresh antiforgery token/header для unsafe requests и отказ cookie mutation без token. Exact production DNS/TLS/origin и возможный `SameSite=None` override остаются deployment gate `OPS-01`/`M5`.
- Точные .NET SDK patch, npm и PostgreSQL server versions неизвестны. Node фиксируется только диапазоном transitive Vite engine.
- Local Azurite Blob workflow подтверждён в `DATA-02`; Docker Compose/devcontainer по-прежнему не определены.
- `DATA-04` подтвердила create/replace/delete и promotion rollback на disposable PostgreSQL/Azurite. После DB commit Blob cleanup выполняется независимо от request cancellation; при исчерпании Blob retries safe orphan идентифицируется exact container/blob exception и требует operational cleanup.
- Production Azure Storage account/container ACL и доступ остаются неизвестны до `OPS-03`/`M5`.
- Production hosting/topology не выбраны; Hangfire dashboard production gate ещё не реализован.
- Локальные версии инструментов ниже являются evidence текущей машины, а не project pins: `.NET SDK 9.0.203`, Node `v25.6.1`, npm `11.12.0` (проверено 12 июля 2026 года).
