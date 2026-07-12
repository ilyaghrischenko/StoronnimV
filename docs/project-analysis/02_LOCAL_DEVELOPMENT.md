# Локальная разработка

## Необходимые инструменты

| Инструмент | Требование по коду | Найдено в среде анализа |
|---|---|---|
| Node.js | README: 18+; package lock используется npm | `v25.6.1` |
| npm | scripts в `package.json` | `11.12.0` |
| .NET SDK | все проекты target `net9.0` | `9.0.203` |
| PostgreSQL | EF Core Npgsql, Hangfire PostgreSQL | не проверялся |
| Azure Storage/Azurite | `BlobRepository` | не проверялся |
| Docker | только backend Dockerfile | не проверялся |

Доказательства: [package.json](../../frontend/storonnimv.client/package.json), [solution projects](../../backend/StoronnimV.Server/StoronnimV.Server.sln), [WebApplicationBuilderExtensions.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs).

## Переменные окружения

Backend фактически требует при startup:

| Имя | Назначение |
|---|---|
| `DB_CLOUD` | PostgreSQL connection string для EF, Hangfire и health check |
| `TOKEN_ISSUER` | JWT issuer |
| `TOKEN_AUDIENCE` | JWT audience |
| `TOKEN_KEY` | ключ подписи JWT |
| `TOKEN_LIFETIME` | срок токена в днях |
| `CLIENT_URL` | единственный разрешённый CORS origin |
| `BLOB_STORAGE` | Azure Blob Storage connection string; требуется при создании `BlobRepository` |
| `DOMAIN` | domain JWT cookie при login/logout |

Frontend repository содержит имя `VITE_API_URL`, но текущий код его **не читает** и использует hardcoded localhost — [GlobalContext.tsx](../../frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx). README backend перечисляет другие имена (`DB_CONNECTION_STRING`, `AZURE_STORAGE_CONNECTION`, `JWT_KEY`), которые код не использует; его пример нельзя копировать как рабочий.

Секретные значения в анализ не включались.

## Frontend

Рабочий каталог: `frontend/storonnimv.client`.

Подтверждённые scripts из manifest:

- `npm run dev` — Vite dev server;
- `npm run build` — `tsc -b && vite build`, пишет `dist`;
- `npm run lint` — ESLint;
- `npm run preview` — preview production build.

Зависимости уже присутствовали, но install не выполнялся. Uncached `tsc --noEmit` прошёл. Vite production bundling с `--outDir /tmp/...` прошёл (535 modules). `eslint .` вернул 6 ошибок и 20 warnings. Dev/preview не запускались из-за API/configuration и возможной записи cache.

## Backend

Рабочий каталог: `backend/StoronnimV.Server`.

Manifest подтверждает стандартную команду запуска `dotnet run --project StoronnimV.Api/StoronnimV.Api.csproj`, но она **не проверялась**. Перед запуском нужны все переменные выше и доступные PostgreSQL/Azure dependencies. Swagger/OpenAPI доступен только в Development; health endpoint — `/health`; Hangfire dashboard использует стандартный route.

README предлагает `dotnet ef database update`; это изменяющая команда и в анализе не выполнялась. `DatabaseInitializer.Initialize` не вызывается из `Program.cs`, поэтому автоматическое применение migrations кодом не подтверждено.

## Связь frontend/backend

- Backend routes начинаются с `/api`.
- Axios отправляет cookies через `withCredentials: true`.
- Backend CORS разрешает credentials только для `CLIENT_URL`.
- Login записывает JWT в cookie `Token`; bearer handler читает этот cookie.
- Локальные ports заданы в `launchSettings.json`, но frontend сейчас жёстко использует `https://localhost:44315/api`; согласованность с profile нужно проверять перед запуском.

## База данных и migrations

EF Core использует PostgreSQL. В repository 24 migration-файла (без designer/snapshot), последняя — `20250501144418_AddGroupSocials`. Схема содержит Admins, GroupPages, GroupSocials, Members, Socials, NewsItems, Schedules, MusicPlatforms и Videos — [StoronnimVContextModelSnapshot.cs](../../backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs).

## Тесты и статические проверки

- Backend test project настроен на xUnit/coverlet, но не содержит тестов.
- Backend build был запущен только на копии solution в `/tmp` с `--no-restore`; он не дошёл дальше `ValidateSolutionConfiguration` и не дал результата. После повторной неуспешной попытки дальнейшие build-команды прекращены. Test/run не выполнялись.
- Frontend type-check: **подтверждено запуском, проходит**.
- Frontend lint: **подтверждено запуском, не проходит**.
- Отдельного frontend test script/framework не найдено.

## Известные препятствия запуска

1. Нужны семь корректных runtime variables; README не соответствует фактическим именам.
2. Frontend API base URL нужно согласовать с backend profile/deployment; текущий production build иначе обращается к localhost.
3. Для admin нужны совместимые HTTPS, cookie, CORS и JWT settings.
4. PostgreSQL schema и initial admin не создаются подтверждённым startup-кодом.
5. Azure Blob connection нужен для media mutations.
6. Реальное влияние отсутствующего `UseAuthentication` требует runtime integration test.
