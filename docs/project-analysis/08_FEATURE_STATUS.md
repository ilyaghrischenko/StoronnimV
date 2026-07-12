# Состояние функциональности

Статусы отражают текущий repository, а не deployed site. «Реализован» означает статически прослеживаемый сквозной код; без runtime это не гарантия.

## Матрица сценариев

| Пользовательский сценарий | Frontend | Backend | Данные | Общий статус | Доказательство | Уверенность | Что не проверено |
|---|---|---|---|---|---|---|---|
| Навигация публичных страниц | routes/layout есть | не нужен | — | реализован статически | `Page.tsx`, `Header.tsx` | Подтверждено | browser/deep links |
| Home: новости/афиша/video | UI/contexts есть | 3 endpoints есть | projections есть | частично реализован | `HomeContext.tsx`, `HomeController.cs` | Высокая | null/real data/API URL |
| Просмотр news + detail | list/pagination/modal есть | read API есть | News+optional Video | реализован статически | `NewsContext.tsx`, `NewsController.cs` | Высокая | runtime payload |
| Просмотр schedule + map | list/pagination/modal есть | read API есть | Schedule | частично реализован | `ScheduleContext.tsx`, controller | Высокая | frontend extra `status`; map/data |
| Group/members/socials | page/modals есть | read API есть | Group/Member/Social | реализован статически | `GroupContext.tsx`, `GroupPageController.cs` | Высокая | invariant одной GroupPage |
| Music platforms/Spotify | UI есть | list endpoint есть | MusicPlatform | реализован статически | `MusicContext.tsx`, `MusicController.cs` | Высокая | embed/external links |
| Video categories/list | UI/pagination есть | typed page endpoint | Video | частично реализован | `VideoContext.tsx`, `VideoController.cs` | Высокая | hardcoded category imagery/media ACL |
| Footer group socials | UI/fetch есть | all endpoint | GroupSocial | реализован статически | `Footer.tsx`, controller | Высокая | external URLs |
| Login/logout | form/cookie requests | identity code есть | Admin | состояние неизвестно без запуска | `AdminContext.tsx`, identity services | Средняя | HTTPS/CORS/cookie/domain |
| Admin detection/controls | calls есть | bearer-protected endpoint | Admin JWT | частично реализован | `fetchIsAdmin`, `AdminController` | Средняя | auth pipeline runtime |
| SuperAdmin route | role из sessionStorage | role policy | Admin.Type | вероятно сломан | `ProtectedRoute.tsx`, `Program.cs` | Высокая | policy runtime |
| Basic-admin list/create | UI/context есть | endpoints/services есть | Admin | частично реализован | `AdminContext`, `SuperAdminController` | Высокая | SuperAdmin auth/bootstrap |
| Basic-admin delete | request есть, state не обновляется | endpoint есть | Admin | частично реализован | `AdminContext.deleteAdmin` | Подтверждено | reload behavior |
| Basic-admin password edit | frontend условие неверно | validator/service есть | Admin | вероятно сломан | `AdminContext.editAdminPassword`, validator | Высокая | actual response |
| Create content/media | forms mostly multipart | `[FromForm]` endpoints | DB+Blob | частично реализован | add forms, `AdminController` | Высокая | external services/validation |
| Edit content text | forms есть | `[FromBody]` endpoints | DB | вероятно сломан в 8 flows | edit forms vs controller | Высокая | binding responses |
| Create/edit member social | FormData как JSON | `[FromBody]` | Social | вероятно сломан | social forms/controller | Высокая | actual 400/415 |
| Replace/delete media | forms/endpoints есть | DB+Blob orchestration | DB+Blob | частично реализован | media actions/services | Высокая | atomicity/ACL/files |
| Daily schedule status | UI не нужен | Hangfire job есть | Schedule | вероятно сломан | `ScheduleService.UpdateStatusesAsync` | Высокая | job execution |
| Health/Swagger | UI не нужен | configured | DB health | требует запуска | `Program.cs` | Подтверждено статически | actual endpoint |
| Mobile public UI | routes есть | не влияет | — | отсутствует как usable layout | layout/runtime CSS | Высокая | screenshot/device matrix |
| Developers page | `<p>hello</p>` | нет | — | заготовка | `Developers.tsx` | Подтверждено | intended content |
| Automated tests | нет frontend tests | пустой xUnit project | — | отсутствует | manifests/test folder | Подтверждено | — |
| Deployment automation | artifacts есть | Dockerfile есть | env externally | частично/неизвестно | Dockerfile/static config | Высокая | CI/cloud configuration |

## Реализованные области

Route/page skeleton, public resource endpoints, layered data access, core entities/migrations, public pagination/detail patterns, CRUD surface и media adapters.

## Частично реализованные

Production integration, admin/auth, mutations, error/loading states, mobile navigation/layout, deployment, background processing и media consistency.

## Отсутствующие

Рабочая automated test suite, подтверждённая mobile layout strategy, CI/CD workflows, antiforgery protection, seed/bootstrap admin и единый error contract.

## Неиспользуемые или изолированные

`ResolutionWrapper`/`MobileInDeveloping`, `DatabaseInitializer`, image resizer, отдельные GET-by-id endpoints, create/delete GroupPage UI, несколько generic service contracts и закоммиченный potentially stale `dist`.
