# FEAT-09 — Basic Admin management

## Цель и границы

Завершить desktop SuperAdmin vertical: list/create/edit login/edit password/delete только Basic Admin accounts, live UI state без reload, согласованный password contract и защита SuperAdmin records на role/service/DB/API boundaries.

Вне объёма: recovery UI, mobile/tablet redesign, новая admin dashboard, production DB, frontend test-framework setup (`QA-03`), исправление общего ESLint baseline и обновление dependencies. Коммит и branch change не выполнялись.

## Исходное состояние

- `FEAT-09` существовала со статусом `planned`; зависимости `FEAT-02`, `API-03`, `DATA-03` имели `done`.
- `AdminContext.deleteAdmin` вычислял `filter`, но не сохранял state.
- Add/edit/delete modals выполняли `window.location.reload()` после mutation.
- Password modal передавал new password как old password и confirmation как new password; context одновременно разрешал request только при равных old/new passwords, хотя backend validator требует разные значения.
- Non-field `400` ProblemDetails содержали `detail`, но UI читал только `errors`, поэтому duplicate login и incorrect old password были невидимы.
- Service list возвращал только Basic Admin, но delete/login/password mutations не проверяли `Admin.Type`; endpoint можно было направить на SuperAdmin ID.
- Duplicate login проверялся только среди Basic Admin и отдельным read-before-write без DB constraint.
- Существующие пользовательские изменения FEAT-06–FEAT-08 и других задач сохранены.

## Затронутые файлы

Backend:

- `StoronnimV.Application/Services/Entities/SuperAdminService.cs`.
- `StoronnimV.Api/Middlewares/ExceptionMiddleware.cs`.
- `StoronnimV.Infrastructure/StoronnimVContext.cs`.
- `StoronnimV.Infrastructure/Migrations/20260717233000_EnforceAdminLoginUniqueness.cs`.
- `StoronnimV.Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs`.
- `StoronnimV.Tests/Application/SuperAdminServiceTests.cs`.
- `StoronnimV.Tests/Api/BasicAdminCrudIntegrationTests.cs`.

Frontend:

- `src/components/contexts/AdminContext.tsx`.
- `src/components/elements/admin/AdminContainer.tsx`.
- `src/components/elements/admin/BasicAdmins.tsx`.
- `src/components/elements/admin/SuperAdminButtons/AddAdminModal.tsx`.
- `src/components/elements/admin/SuperAdminButtons/EditAdminModal.tsx`.
- `src/components/elements/admin/SuperAdminButtons/DeleteAdminModal.tsx`.

Documentation/evidence:

- `docs/implementation/00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`, `11_MIGRATION_WORKFLOW.md`.
- этот документ.
- `output/playwright/FEAT-09-before.png`, `FEAT-09-after.png`, `FEAT-09-error.png`.

## Решения и изменения

- `SuperAdminService` проверяет `AdminType.Basic` до delete/login/password mutation; SuperAdmin target получает unified `400`, repository mutation не выполняется.
- Create явно устанавливает `AdminType.Basic`.
- Login conflict проверяется через all-admin lookup; self-login при edit допускается, чужой Basic/SuperAdmin login отклоняется.
- PostgreSQL unique index `IX_Admins_Login` устраняет concurrent check-then-write race. Guarded migration сначала ищет duplicates и останавливается без удаления/исправления данных.
- Exact unique-constraint violation преобразуется middleware в безопасный `400` ProblemDetails; generic DB failures сохраняют `500` contract.
- Admin list начинается с `[]`; add/login edit/delete обновляют React state. Mutation functions возвращают success, поэтому modal закрывается только после успешного response; `window.location.reload()` удалён.
- Password form отправляет фактические old/new values, требует matching confirmation и запрещает equal old/new до request; backend validation остаётся authoritative.
- Field errors показываются как раньше; пустой `errors` использует ProblemDetails `detail` под `General`. Delete modal также показывает ошибки. Ошибки очищаются перед открытием следующего Admin modal.
- `fetchBasicAdmins` memoized, а caller effect содержит корректную dependency.

## TDD и проверки

| Команда или сценарий | Результат | Exit code | Что доказано |
|---|---|---:|---|
| Initial targeted service RED (`SuperAdminServiceTests`) | 4/5 service assertions failed: delete/login/password изменяли SuperAdmin; create допускал SuperAdmin login | 1 | Type и all-admin login boundaries отсутствовали |
| Browser RED, controlled WebKit create | Mutation завершилась, `performance.navigation.type` стала exact `reload` | 0 для сценария; критерий failed | UI зависел от full reload |
| `AdminLoginDatabase_RejectsDuplicateRows` RED | Expected `DbUpdateException` отсутствовал | 1 | DB допускала duplicate logins |
| Focused `SuperAdminServiceTests` GREEN | 5/5 passed | 0 | Non-Basic mutations rejected before write; valid password change работает |
| Canonical EF update на disposable PostgreSQL 17 | Предыдущие 25 migrations и новая login migration применены отдельными commands | 0 | Schema содержит все 26 migrations без startup mutation |
| Повторный `dotnet ef database update ...` | `No migrations were applied. The database is already up to date.` | 0 | Migration command idempotent |
| `dotnet ef migrations has-pending-model-changes ...` | `No changes have been made to the model since the last migration.` | 0 | Model, snapshot и migration согласованы |
| `AdminLoginDatabase_RejectsDuplicateRows` GREEN | 1/1 passed | 0 | PostgreSQL отклоняет duplicate Basic/SuperAdmin login |
| `BasicAdminCrud_RealApiPostgres_EnforcesRoleAndAccountTypeBoundaries` | 1/1 passed | 0 | Basic token `403`; SuperAdmin CRUD/readback; password login; SuperAdmin target `400`; concurrent same-login `200/400` и одна DB row |
| All integration flags + full solution tests | 125/125 passed, 0 skipped | 0 | Full backend regression/API/PostgreSQL/Azurite gate |
| `dotnet restore backend/.../StoronnimV.Server.sln --disable-build-servers` | Restore completed | 0 | Dependencies available |
| `dotnet build ...sln --no-restore --configuration Release --disable-build-servers -m:1` | 0 errors, 2 existing ImageSharp advisory warnings | 0 | Release solution compiles |
| Targeted ESLint six changed Admin TS/TSX files | No findings | 0 | FEAT-09 frontend code lint-clean |
| `npm run build` | 540 modules; production bundle built | 0 | TypeScript/Vite production build |
| `npm run lint` | 4 errors, 2 warnings in unchanged Group/shared files | 1 | Existing QA-03 baseline remains outside FEAT-09; Admin warning removed |
| Bundle/changed-file scan for localhost mock values, connection value and private-key/storage markers | No matches | 0 | Generated bundle/source contain no task endpoint, disposable connection value or secret marker |
| Controlled WebKit GREEN: add/edit login/edit password/delete | Table changed after each mutation; `performance.timeOrigin` stayed `1784329330936` throughout | 0 | UI state updates without reload |
| Controlled WebKit password mismatch | Error remained visible in modal; API log contained no PATCH | 0 | Confirmation blocks invalid request |
| Controlled WebKit valid password | PATCH body exact `{"oldPassword":"AAAaa11111","newPassword":"BBBbb22222"}` | 0 | Frontend/backend password contract aligned |
| Controlled WebKit duplicate login | Modal remained open; `General: Admin with this login already exists` visible | 0 | Non-field ProblemDetails preserved for user |
| Before/after screenshot inspection | Desktop layout visually unchanged; same Basic Admin table/frame after behavior fix | 0 | No desktop redesign; stable final visual state |
| Independent focused code review after fixes | 0 Critical, 0 Important; assessment ready | 0 | Security/state/migration/test diff independently checked |
| `git diff --check` | No diagnostics | 0 | Whitespace valid across worktree diff |

Первый combined focused test run после migration завис после discovery; exact process был остановлен, а DB uniqueness и real CRUD tests затем выполнены отдельно с exit 0. Несколько параллельных solution build attempts также зависли без output; exact task-owned processes остановлены, финальный serialized `-m:1` build завершился exit 0. Source correction для этих runner/process stalls не требовалась.

## Невыполненные проверки и ограничения

- Production/staging DB и account records не проверялись и не изменялись: запрещено scope; real-environment smoke остаётся M5/M6.
- Automated frontend component suite не добавлялась: test framework входит в `QA-03`. FEAT-09 UI acceptance выполнен controlled WebKit scenario с network/body/navigation evidence и screenshots.
- Mobile/tablet Admin не проверялся: `MOB-05` зависит от FEAT-09 и остаётся отдельной задачей.
- Chrome отсутствовал локально; browser verification выполнена Playwright WebKit.

## Проблемы вне scope

- `SixLabors.ImageSharp 3.1.6` выдаёт existing `NU1902`/`NU1903`; dependency update запрещён без отдельной доказанной задачи.
- Full ESLint сохраняет 4 errors/2 warnings в `FrameLayout.tsx`, `Header.tsx`, `PaginationSection.tsx`, `GroupDescription.tsx`, `MemberModal.tsx`; это `QA-03`.
- Controlled browser сообщает external `https://storonnimv.com/site.webmanifest` DNS error; FEAT-09 network/API requests и UI не затронуты.

## Итог критериев приёмки

| Критерий | Итог |
|---|---|
| List/create/delete/edit login/password | Выполнен: real API/PostgreSQL CRUD/readback и controlled WebKit |
| UI обновляется без reload | Выполнен: add/login edit/password/delete при неизменном `performance.timeOrigin` |
| Password rule согласован | Выполнен: old/new/confirmation wiring, client checks, backend validator, real login новым password |
| SuperAdmin нельзя изменить Basic Admin endpoints | Выполнен: delete/login/password service guards и real API/DB assertions |
| Login identity однозначна | Выполнен: all-admin precheck, guarded unique DB index, concurrent request test |
| Role/service/E2E tests | Выполнен: existing role policy tests, 5 service tests, 2 real integration tests, controlled WebKit flow |

Все критерии выполнены. `FEAT-09` имеет статус `done`; backlog/state/index синхронизированы. Следующая задача — `FEAT-10`, но она не начиналась.
