# FEAT-02 — Server-validated route guard

## Цель

Закрыть `/admin/basic-admins` по server-confirmed role: client `sessionStorage.role` не влияет на решение, а refresh проходит через явные loading, authorized, unauthorized или forbidden states без показа protected content до ответа.

## Исходное состояние

- `ProtectedRoute` вызывал `fetchIsAdmin`, но не ждал результат и не использовал его при решении.
- Route открывался только по строке `sessionStorage.role`; forged `SuperAdmin` показывал protected table до позднего server rejection.
- Отдельный endpoint чтения текущей server role отсутствовал.
- Frontend test framework отсутствует и запланирован отдельной задачей `QA-03`.
- До FEAT-02 worktree уже содержал незакоммиченные изменения `DATA-03`/`API-02`/`FEAT-01`; они сохранены и не считались изменениями этой задачи.

## Затронутые файлы

- `backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/AuthenticationIntegrationTests.cs`
- `frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx`
- `docs/implementation/evidence/FEAT-02.md`
- `docs/implementation/00_INDEX.md`
- `docs/implementation/04_BACKLOG.md`
- `docs/implementation/09_STATE.md`

## Решения

- Guard проверяет role при каждом mount через authenticated `GET /api/admin/role`; глобальный auth context не расширяется role cache, поэтому старое client значение не может дать краткий authorized state.
- Endpoint возвращает role из authenticated JWT principal. Cookie и Authorization header проверяются одной integration theory.
- До server response guard показывает accessible `role="status"`; protected children не монтируются.
- `401` ведёт на existing 401 error route; authenticated role mismatch и прочие non-success ответы fail closed на existing 403 route.
- `sessionStorage.role` оставлен как существующий FEAT-01 client hint, но guard его не читает. JWT в JS storage не добавлялся.
- Новые frontend test dependencies не добавлялись: controlled browser E2E покрывает guard states, а `QA-03` сохраняет утверждённую ответственность за test framework.

## Выполненные изменения

1. Добавлен authenticated `GET /api/admin/role`, читающий `ClaimTypes.Role` из server principal и запрещающий role-less principal.
2. Добавлены integration cases для Basic/SuperAdmin через bearer header и HttpOnly-cookie transport.
3. `ProtectedRoute` заменил synchronous storage decision на local state machine: `loading`, `authorized`, `unauthorized`, `forbidden`.
4. Effect отменяет state update после unmount и имеет полный dependency list; stale/late response не создаёт redirect loop.

## Проверки

Все API/DB действия выполнялись 14 июля 2026 года только с disposable local PostgreSQL 17 и synthetic/mock auth responses. Production/staging/remote resources не использовались.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Browser RED | Local Vite + mock login `SuperAdmin`; server session endpoints возвращают delayed `401`; открыть `/admin/basic-admins` | До изменения table и `Додати Адміна` отображались до server rejection | n/a | Исходный guard доверял client role и имел protected-content flicker |
| Endpoint RED | `dotnet test ... --filter FullyQualifiedName~AdminRoleEndpoint_WithToken_ReturnsServerRole` с disposable local `DB_CLOUD` | 2 cases ожидаемо получили `404` | 1 | Test отличал отсутствие role endpoint до реализации |
| Endpoint GREEN, focused | Та же filtered integration command после реализации | 4 passed: Basic/SuperAdmin через header/cookie | 0 | Server возвращает role из обоих поддерживаемых JWT transports |
| Forged-role browser GREEN | Mock login возвращает client role `SuperAdmin`; `/admin/role` delayed `401` | Сначала только `Перевірка доступу...`; admin button count `0`; затем 401 route | n/a | Forged storage не открывает route; protected content не flicker-ит |
| Basic browser GREEN | Client hint `SuperAdmin`; server role `Basic` | Loading, затем `/error?statusCode=403&message=Forbidden` | n/a | Server role имеет приоритет над client hint |
| SuperAdmin browser GREEN | Server role `SuperAdmin`; list endpoint `200` | Loading, затем protected table; full refresh повторил loading и вернулся к table на том же URL | n/a | Authorized state и refresh стабильны; redirect loop отсутствует |
| Visual before/after | In-app browser screenshots `feat-02-before.png` и `feat-02-after.png` | До: unauthorized admin table; после: 401 error без protected controls | n/a | UI diff проверен визуально |
| Frontend production build | `VITE_API_URL=https://frontend-build.invalid/api npm run build` | 535 modules transformed; build завершён | 0 | TypeScript и production bundle собираются |
| Targeted ESLint | `npx eslint src/components/elements/admin/ProtectedRoute.tsx` | 0 errors, 0 warnings | 0 | Изменённый frontend source соответствует lint rules |
| Full ESLint | `npm run lint` | Existing baseline: 5 errors, 13 warnings; FEAT-02 file отсутствует в findings | 1 | Repo-wide baseline измерен; несвязанный `QA-03` debt не скрыт |
| Backend tests, final | `dotnet test ...StoronnimV.Tests.csproj --no-restore --disable-build-servers` с disposable local `DB_CLOUD` | 21 passed, 0 failed/skipped | 0 | Полная auth/CORS/CSRF/role integration suite green |
| Solution Release build | `dotnet build ...StoronnimV.Server.sln --no-restore --configuration Release --disable-build-servers --no-incremental` | Build succeeded; 0 errors, 8 existing warnings | 0 | Backend и новый endpoint собираются в Release |
| Bundle/source contract scan | `rg` для `/admin/role`, storage read в guard, `localhost:44315` и credential-like literals | Role endpoint присутствует; guard storage read и forbidden literals отсутствуют | 0 | Bundle использует server role contract; local/secrets не встроены |
| Diff whitespace | `git diff --check` | Нарушений нет | 0 | Итоговый diff не содержит whitespace errors |
| Cleanup | `lsof` для `4175`/`4176`; filtered `docker ps -a` | Оба listener отсутствуют; disposable PostgreSQL container удалён | 1/1/0 | Локальное test environment полностью остановлено |
| Scope/no commit | `git status --short`; review full diff/HEAD | FEAT-02 ограничена перечисленными files; прежние user changes сохранены; commit не создан | 0 | Выполнена только FEAT-02; следующая задача не начата |

Диагностика до корректного RED: первый focused test без `DB_CLOUD` завершился на runtime gate, а sandboxed MSBuild не смог создать local IPC socket. Повтор вне sandbox с явным disposable localhost PostgreSQL дошёл до ожидаемого `404`; source changes для обхода environment gate не вносились.

## Невыполненные проверки

- Отдельный frontend component runner не запускался: framework отсутствует и его добавление утверждено в `QA-03`. FEAT-02 покрыта controlled browser E2E role matrix, production build, targeted lint и real ASP.NET integration tests.
- Production/staging cookie topology не проверялась: относится к `M5`/`M6` и запрещена scope FEAT-02.
- Full ESLint не green из-за пяти existing errors и тринадцати warnings вне FEAT-02; их исправление относится к `QA-03`.

## Проблемы вне scope

- `BasicAdmins` сохраняет existing React key warning и initial placeholder row; относится к будущему SuperAdmin CRUD item `FEAT-09`, FEAT-02 не блокирует.
- `FrameLayout.tsx`, `Header.tsx`, `NoData.tsx` и `PaginationSection.tsx` сохраняют пять `@ts-ignore` lint errors; не блокируют guard.
- Тринадцать existing hook warnings остаются вне FEAT-02.
- `SixLabors.ImageSharp 3.1.6` выдаёт existing `NU1902`/`NU1903`; package update не требовался и не выполнялся.
- Release build сохраняет шесть existing compiler warnings помимо двух package advisories.

## Итог критериев приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Подмена `sessionStorage` не открывает route | выполнен | Client hint `SuperAdmin` + server `401`/Basic не монтировали protected children; дали 401/403 |
| Refresh стабилен | выполнен | Server-confirmed SuperAdmin после refresh прошёл loading и снова показал table на том же URL |
| Loading/authorized/forbidden зависят от server response | выполнен | Controlled 401, Basic и SuperAdmin browser matrix; four backend role cases |
| JWT не хранится в JS storage | выполнен | Guard использует credentialed HTTP; JWT остаётся HttpOnly cookie/header transport |
| Component/E2E role tests | выполнен через E2E | Browser state matrix + real ASP.NET role integration; test framework не переносился из `QA-03` |

Все критерии `FEAT-02` выполнены. Следующая задача `API-03` не начиналась.
