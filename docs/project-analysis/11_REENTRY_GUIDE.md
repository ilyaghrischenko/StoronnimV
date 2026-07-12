# Руководство по возвращению в проект

## Быстрый маршрут

1. Прочитать [01_PROJECT_OVERVIEW.md](01_PROJECT_OVERVIEW.md) и [03_ARCHITECTURE.md](03_ARCHITECTURE.md).
2. Посмотреть [08_FEATURE_STATUS.md](08_FEATURE_STATUS.md), чтобы не принять наличие кода за готовность.
3. Пройти один public flow по [06_API_AND_DATA_FLOW.md](06_API_AND_DATA_FLOW.md).
4. Затем разобрать admin/auth как первый cross-cutting модуль.
5. До backlog ответить на product/deployment вопросы из [10_OPEN_QUESTIONS.md](10_OPEN_QUESTIONS.md).

## 10 ключевых исходных файлов

1. [Page.tsx](../../frontend/storonnimv.client/src/components/pages/shared/Page.tsx) — карта продукта/routes.
2. [GlobalContext.tsx](../../frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx) — HTTP, auth/loading/modal wiring.
3. [App.tsx](../../frontend/storonnimv.client/src/App.tsx) — composition/layout и отключённый mobile wrapper.
4. [style.css](../../frontend/storonnimv.client/src/styles/style.css) — фактически подключённые стили.
5. [AdminContext.tsx](../../frontend/storonnimv.client/src/components/contexts/AdminContext.tsx) — login/SuperAdmin state.
6. [Program.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Program.cs) — backend composition/pipeline.
7. [WebApplicationBuilderExtensions.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs) — dependencies/security/config.
8. [AdminController.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs) — content mutation contract.
9. [ScheduleService.cs](../../backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/ScheduleService.cs) — representative service + background defect.
10. [StoronnimVContextModelSnapshot.cs](../../backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs) — authoritative current schema model.

## Основной пользовательский сценарий

Для News: `Page` выбирает `News` → `NewsList` вызывает `NewsContext` → `GlobalContext.sendRequest` → `NewsController` → `NewsControllerService` → `NewsService` → `NewsRepository`/PostgreSQL → projection/AutoMapper response → Context state → cards/detail modal. Это наиболее удобный эталон чтения остальных public features.

## Где что находится

- public/admin frontend: `frontend/storonnimv.client/src/components`;
- runtime frontend data layer: `components/contexts`;
- runtime CSS: `src/styles/style.css`, intended partials: `src/styles/**/*.scss`;
- API: `StoronnimV.Api/Controllers`;
- orchestration/business operations: `StoronnimV.Application/Services`;
- data contracts/model: `Application/DTO`, `Domain`;
- PostgreSQL/Azure adapters: `StoronnimV.Infrastructure`.

## Порядок изучения модулей

1. Public News end-to-end.
2. Shared frontend layout/state/error patterns.
3. Auth/admin path: login → cookie/JWT → `isAdmin` → content mutation → SuperAdmin.
4. Media lifecycle DB↔Blob.
5. Schedule + Hangfire.
6. Deployment/env/migrations.
7. Mobile layout after choosing style source of truth.

## Что пока лучше не менять

- migrations/schema до проверки production DB level и backup;
- cookie/JWT/CORS отдельными точечными изменениями без end-to-end design;
- SCSS или compiled CSS до выбора source of truth;
- Blob naming/deletion без inventory/compensation strategy;
- mobile frame без решения владельца о сохранении декоративной концепции;
- generated/tracked artifacts и logs без отдельной cleanup authorization.

## Решения до backlog

- environment/deployment target и доступные external resources;
- accepted UI/style baseline;
- mobile product scope;
- singleton invariant GroupPage;
- SuperAdmin bootstrap/auth policy;
- desired media atomicity/validation;
- какие admin scenarios всё ещё нужны;
- minimum test and deployment gates.

## Первый модуль для разбора

**Admin/auth integration.** Он одновременно вскрывает текущий API URL, CORS/cookie domain, JWT middleware, role policy, frontend route guard, JSON/FormData contracts и доступ ко всем mutations. Пока этот путь не определён, backlog по отдельным content features будет строиться на недоказанных предпосылках.
