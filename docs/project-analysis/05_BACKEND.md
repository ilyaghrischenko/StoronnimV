# Backend

## Solution и ответственность проектов

Все пять проектов target-ят `net9.0` — [StoronnimV.Server.sln](../../backend/StoronnimV.Server/StoronnimV.Server.sln).

| Проект | Ответственность |
|---|---|
| `StoronnimV.Api` | startup, DI, controllers, middleware, Swagger/health/rate limits |
| `StoronnimV.Application` | orchestration services, DTO, mapping, validation, JWT, image/background logic |
| `StoronnimV.Domain` | entities, enums, projections, repository contracts |
| `StoronnimV.Infrastructure` | EF/PostgreSQL repositories, migrations, Azure Blob adapter |
| `StoronnimV.Tests` | пустая xUnit/coverlet заготовка без ProjectReference |

Dependencies: `Api → Application + Infrastructure`, `Application → Domain`, `Infrastructure → Domain`. Domain внешних packages не имеет. DB и Blob operations выполняются раздельно, общей transaction/compensation boundary нет.

## Startup, DI и конфигурация

Entry point — [Program.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Program.cs). Регистрации собраны в [WebApplicationBuilderExtensions.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs): scoped repositories/entity services/controller services, EF DbContext, Hangfire, JWT, validation, AutoMapper, Serilog, CORS, compression, rate limits, health checks.

Обязательные startup variables: `DB_CLOUD`, `TOKEN_ISSUER`, `TOKEN_AUDIENCE`, `TOKEN_KEY`, `TOKEN_LIFETIME`, `CLIENT_URL`. `BLOB_STORAGE` нужен при создании blob repository; `DOMAIN` используется cookie login/logout. README перечисляет несовместимые имена.

Изолированный код: `DatabaseInitializer`, `IAdminService`, `IImageResizerService` и `IBlobRepository.GetFileUrl` не имеют найденных runtime consumers. AutoMapper отдельно регистрируется через DI и проверяется ручной неполной configuration, не включающей Music/Video/GroupSocial profiles.

## Middleware pipeline

Порядок в коде: routing → CORS → authorization → controller mapping → HTTPS redirect → static files → exception/logging middleware → Hangfire dashboard → compression → rate limiting → health checks.

Главные выводы:

- `UseAuthentication()` отсутствует. `AdminController` задаёт bearer scheme в attribute, поэтому policy evaluator может вызвать handler; `SuperAdminOnly` содержит только role requirement и без заранее заполненного `User` статически выглядит недостижимым.
- exception middleware стоит после authorization, поэтому ранние exceptions не покрывает;
- HTTPS redirect/compression/logging расположены поздно;
- Hangfire dashboard подключён дважды (`Use` и `Map`) без явного authorization filter;
- rate limiter partition key — постоянное имя policy, то есть limit общий, не per-client.

## Controllers и endpoints

Найдено 55 controller endpoints: public home/resource reads, account login, protected admin content CRUD/media и SuperAdmin account management. Полная таблица — [06_API_AND_DATA_FLOW.md](06_API_AND_DATA_FLOW.md).

Public read API — наиболее цельная область. Pagination различает invalid `page <= 0`, но не валидирует `pageSize`/home count; empty database и out-of-range page возвращают одинаковый empty result.

## Services и repositories

Типовой поток: controller → controller service (mapping/orchestration) → entity/home/account service → repository → EF/Blob.

Подтверждённые риски:

- `ScheduleService.UpdateStatusesAsync` использует `List.ForEach(async ...)`, не await-ит updates; Hangfire job может завершиться до DB operations — [ScheduleService.cs](../../backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/ScheduleService.cs).
- DB/Blob mutations не атомарны: partial row/orphan blob возможны при сбое.
- promotion replacement удаляет старое до успешной загрузки нового — [VideoService.cs](../../backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/VideoService.cs).
- Group API допускает несколько rows, а read выбирает `First` без uniqueness invariant.
- generic update отмечает entity целиком modified; concurrency tokens отсутствуют; `UpdatedAt` объявлен `init` и фактически не обновляется.
- file size/MIME/signature/filename policy не валидируются; зарегистрированный ImageResizer не используется.

## Модели, DTO и mapping

Сущности: `Admin`, `GroupPage`, `Member`, `Social`, `GroupSocial`, `News`, `Schedule`, `MusicPlatform`, `Video`. Связи snapshot: required `Member 1—N Social` с cascade delete; optional `News → Video` без cascade. Остальные таблицы независимы.

Response projections + AutoMapper обычно дают camelCase JSON, совместимый с frontend. Найденные расхождения:

- `ScheduleShortResponse` не содержит `status`, frontend `IScheduleListItem` требует его;
- nullable photos в DB не везде nullable в DTO/interfaces;
- Home schedule/video могут быть `null`, хотя controller response type объявлен non-null;
- create news получает browser date `yyyy-MM-dd`, а backend разбирает `dd.MM.yyyy` и иначе подставляет текущую дату;
- девять admin forms отправляют FormData для `[FromBody]` endpoints.

## База данных и migrations

PostgreSQL/EF Core 9, 9 DbSet, 24 migrations (ноябрь 2024 — май 2025). Startup migrations не применяет. Seed/bootstrap SuperAdmin не найден. Unique index на `Admin.Login` отсутствует; application uniqueness check race-prone. SuperAdmin service получает entity по ID без проверки `AdminType`, поэтому «basic-admin» mutations потенциально могут затронуть SuperAdmin.

`StoronnimV.Infrastructure.csproj` содержит Windows-specific `HintPath` к framework DLL, что является portability risk на Linux/macOS, несмотря на package reference.

## Авторизация и безопасность

JWT содержит admin ID/name и `AdminType` role; читается из HttpOnly cookie `Token` или Authorization header. Cookie settings: Secure/SameSite/expiry из appsettings, domain из environment. CORS разрешает один origin и credentials.

Риски:

- cookie `SameSite=None` и state-changing endpoints без antiforgery/CSRF defense;
- Hangfire dashboard без auth filter;
- общий rate limit позволяет одному client исчерпать quota для всех;
- login messages различают unknown login/wrong password;
- exception response раскрывает raw `ex.Message`;
- JWT revocation отсутствует, JWT lifetime в днях расходится с cookie lifetime в часах;
- публичность Blob URL зависит от внешнего Azure ACL, который код не задаёт.

## Ошибки и логирование

`ExceptionMiddleware` возвращает plain text и mapping 499/400/404/401/415/500. `[ApiController]` и FluentValidation возвращают другой JSON/ProblemDetails contract. Frontend поэтому не имеет единого error contract.

Serilog пишет console и error files; logging middleware фиксирует method/path/status без duration/correlation/user. Исторические logs закоммичены. Exception logger передаёт строку, не exception object/stack trace.

## Внешние интеграции и background

- Azure Blob containers для photo/video;
- PostgreSQL одновременно для EF, Hangfire и health;
- Hangfire daily schedule status job;
- Dockerfile restore/build/publish для .NET 9 Linux;
- `/health`, Swagger/OpenAPI (Development).

Доступность внешних ресурсов, ACL, schema, job execution, Docker и health не проверялись.

## Тесты и завершённость

`StoronnimV.Tests` не содержит tests и даже ProjectReference. Backend не собирался/не запускался из-за read-only ограничения. Поэтому backend нельзя назвать рабочим. Статически он функционально широк, но production wiring, admin contracts, SuperAdmin auth, data/media consistency и background job незавершены.

## Рекомендуемый порядок чтения

1. `Program.cs`, `WebApplicationBuilderExtensions.cs`.
2. Controllers → matching controller services.
3. DTO/mapping → entity services.
4. Domain entities/projections/contracts.
5. `StoronnimVContext`, repositories, snapshot/migrations.
6. Blob, Hangfire, identity/cookie paths.
7. Frontend `GlobalContext`, feature contexts и forms для contract comparison.
