# Обзор проекта

## Назначение продукта

StoronnimV — мультимедийный сайт-визитка украинской рок-группы «Стороннім В». Публичная часть показывает новости, ближайшее выступление и афишу, информацию о группе и участниках, музыкальные платформы, видео и ссылки на социальные сети. Административная часть должна позволять управлять этим контентом; отдельный SuperAdmin-сценарий управляет basic-admin учётными записями.

**Подтверждено:** набор страниц и API прямо следует из [Page.tsx](../../frontend/storonnimv.client/src/components/pages/shared/Page.tsx), controllers в [`StoronnimV.Api/Controllers`](../../backend/StoronnimV.Server/StoronnimV.Api/Controllers) и сущностей в [`StoronnimV.Domain/Entities`](../../backend/StoronnimV.Server/StoronnimV.Domain/Entities).

## Пользователи и сценарии

- посетитель: знакомство с группой, новостями, событиями, музыкой и видео;
- администратор контента: вход и CRUD контента через элементы, встроенные в публичные страницы;
- SuperAdmin: создание, изменение и удаление basic admins.

## Подтверждённый стек

- **Frontend:** TypeScript 5.6, React 18.3, React Router 7, Vite 6, Axios, Context API, SCSS с закоммиченным compiled CSS, Bootstrap/MUI, Swiper, ReactPlayer — [package.json](../../frontend/storonnimv.client/package.json).
- **Backend:** .NET 9 / ASP.NET Core controllers, EF Core 9 + Npgsql/PostgreSQL, FluentValidation, AutoMapper, JWT cookie authentication, Hangfire, Serilog, rate limiting, health checks — [StoronnimV.Api.csproj](../../backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj), [Program.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Program.cs).
- **Storage:** PostgreSQL для метаданных и Azure Blob Storage для файлов — [StoronnimVContext.cs](../../backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimVContext.cs), [BlobRepository.cs](../../backend/StoronnimV.Server/StoronnimV.Infrastructure/Repositories/AzureBlobStorage/BlobRepository.cs).
- **Deployment artifacts:** backend Dockerfile и frontend Azure Static Web Apps routes; CI/CD workflow отсутствует.

README называет .NET 8/9 и заявляет полную мобильную оптимизацию. Код однозначно target-ит `net9.0`, а мобильная готовность не подтверждается; README следует считать частично устаревшим — [README-back.md](../../backend/README-back.md), [README-front.md](../../frontend/README-front.md).

## Структура и архитектура

Репозиторий — монорепозиторий двух ранее самостоятельных приложений:

- `frontend/storonnimv.client` — SPA;
- `backend/StoronnimV.Server` — solution из `Api`, `Application`, `Domain`, `Infrastructure`, `Tests`.

Backend напоминает Clean Architecture по каталогам, но границы не полностью строгие: `Application` зависит от Hangfire/ImageSharp и содержит service orchestration, `Infrastructure` реализует database/blob repositories, а `Api` вручную связывает всё через DI. Frontend организован по feature-компонентам и feature-contexts.

## Текущее состояние

**Общий вердикт:** функционально широкий, но интеграционно незавершённый проект. Публичные и административные экраны, endpoints, migrations и repository/service слои существуют. Однако production-ready состояние не подтверждено, автоматических тестов нет, lint красный, API URL захардкожен на localhost, мобильный layout в основе фиксирован на ширину не менее 1100 px, а JWT pipeline не вызывает `UseAuthentication`.

### Что реализовано статически

- публичные страницы Home, Schedule, News, Music, Group, Video и Developers;
- pagination для news/schedules/videos;
- модальные подробности и empty/loading компоненты;
- login/logout, role-based SuperAdmin UI и backend authorization attributes;
- CRUD news, schedules, videos, group, members, socials, music platforms, group socials;
- PostgreSQL schema с 9 DbSet и 24 migrations;
- Azure file upload/delete, image resizing, Hangfire status updater;
- Swagger/OpenAPI в Development, rate limiting и `/health`.

### Что незавершено или рискованно

- `GlobalContext` использует `https://localhost:44315/api`, хотя `.env.production`/README предлагают `VITE_API_URL` — [GlobalContext.tsx](../../frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx).
- `Program.cs` содержит `UseAuthorization`, но не `UseAuthentication`; защищённые endpoints, вероятно, не получают authenticated principal — [Program.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Program.cs).
- `ProtectedRoute` доверяет роли из `sessionStorage`, а server check не участвует в решении; код содержит TODO — [ProtectedRoute.tsx](../../frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx).
- мобильная заглушка существует, но wrapper закомментирован; основной frame имеет `min-width: 1100px` — [App.tsx](../../frontend/storonnimv.client/src/App.tsx), [App.scss](../../frontend/storonnimv.client/src/styles/shared/App.scss), [FrameLayout.scss](../../frontend/storonnimv.client/src/styles/elements/shared/FrameLayout.scss).
- `DatabaseInitializer` нигде не вызывается; запуск не применяет migrations автоматически — [DatabaseInitializer.cs](../../backend/StoronnimV.Server/StoronnimV.Infrastructure/DatabaseInitializer.cs).
- тестовый проект не содержит тестовых `.cs` файлов — [StoronnimV.Tests.csproj](../../backend/StoronnimV.Server/StoronnimV.Tests/StoronnimV.Tests.csproj).
- repository хранит `dist`, compiled CSS/maps, IDE metadata и исторические logs; Git-история — только initial + массовый import.

## Главные риски

1. **P0/P1 интеграция:** production frontend направляет API-запросы на localhost.
2. **P1 администрирование:** backend authentication pipeline выглядит неполным; сценарий требует runtime-проверки после исправления конфигурации.
3. **P1 мобильность:** страницы используют desktop minimum width, поэтому узкий viewport ожидаемо получает horizontal overflow/уменьшенное desktop-представление.
4. **P1 проверяемость:** нулевое покрытие тестами и не выполненный backend build.
5. **P1 эксплуатация:** Hangfire dashboard не имеет явной authorization policy; доступность зависит от deployment и требует проверки.
6. **P2 сопровождение:** README/env names расходятся с кодом, generated artifacts и logs закоммичены.

## Неизвестные области

- рабочие production URL, Azure/PostgreSQL resources и секреты;
- актуальная schema применённой базы и наличие seed/admin account;
- ожидается ли публичный доступ к Hangfire dashboard;
- какие страницы реально были приняты владельцем по дизайну;
- deployed ли текущий commit и какие сценарии там работают.

## С чего начать повторное знакомство

Прочитать [11_REENTRY_GUIDE.md](11_REENTRY_GUIDE.md), затем пройти главный поток `Page.tsx → feature context → controller → controller service → entity service → repository`. Первым разбирать admin/auth integration: она пересекает frontend configuration, cookies/CORS, JWT middleware, роли и весь content CRUD.
