# FEAT-01 — Login, logout и admin detection

## Цель

Завершить Basic Admin auth vertical: понятный login, восстановление UI из server session после refresh и logout, который закрывает дальнейший admin access.

## Исходное состояние

- Login переходил на Home и записывал `sessionStorage.role` при любом ответе, кроме `401`.
- `401` и network/server failures не показывали пользователю ошибку.
- `fetchIsAdmin` дублировался в шести public pages; routes без такого effect не восстанавливали admin UI после refresh.
- Logout уже очищал client state после успешного server response; backend auth/cookie/CORS/antiforgery contract был подготовлен задачами `API-01` и `API-02`.
- До FEAT-01 worktree уже содержал незакоммиченные изменения `DATA-03`/`API-02`; они сохранены и не считались изменениями этой задачи.

## Затронутые файлы

- `frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx`
- `frontend/storonnimv.client/src/components/contexts/AdminContext.tsx`
- `frontend/storonnimv.client/src/components/elements/admin/AuthForm.tsx`
- `frontend/storonnimv.client/src/components/pages/Home.tsx`
- `frontend/storonnimv.client/src/components/pages/News.tsx`
- `frontend/storonnimv.client/src/components/pages/Schedule.tsx`
- `frontend/storonnimv.client/src/components/pages/Music.tsx`
- `frontend/storonnimv.client/src/components/pages/Group.tsx`
- `frontend/storonnimv.client/src/components/pages/Video.tsx`
- `docs/implementation/evidence/FEAT-01.md`
- `docs/implementation/00_INDEX.md`
- `docs/implementation/04_BACKLOG.md`
- `docs/implementation/09_STATE.md`

## Решения

- Server session проверяется один раз в `GlobalContextProvider` при старте SPA; page-level дубликаты удалены.
- Любой non-`200` session check закрывает admin UI и удаляет stale client role; network failure также не оставляет неподтверждённый admin UI.
- Login меняет auth state, role и route только после `200`.
- `400`, `401`, другие HTTP failures и network failure имеют разные понятные сообщения; повторный submit блокируется на время запроса.
- `ProtectedRoute` и SuperAdmin role validation не менялись: это отдельная `FEAT-02`.

## Выполненные изменения

1. `sendRequest` и `fetchIsAdmin` получили стабильные callback identities; provider выполняет initial server session check.
2. Удалены шесть дублирующих page-level `fetchIsAdmin` effects.
3. Login получил explicit success/failure status handling, loading state и accessible `role="alert"` error.
4. Success-only navigation предотвращает переход и сохранение ошибочного response как role.

## Проверки

Все DB/API действия выполнялись 14 июля 2026 года только с disposable local PostgreSQL 17 и synthetic Basic Admin. Production/staging/remote resources не использовались. Temporary API/Vite/mock processes, DB container и helper удалены после проверки.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Browser RED | Safari + controlled local mock: submit credentials, mock login возвращает `401` | До изменения error отсутствовал; оставались только поля и кнопка | n/a | Проверка различала исходный дефект понятной ошибки |
| Browser GREEN error | Тот же Safari/mock сценарий после изменения | Показано `Неправильний логін або пароль.`; route не изменился | n/a | `401` получает понятный UI state |
| Visual before/after | Safari screenshots до и после изменения | До: после `401` только поля и кнопка; после: под кнопкой виден error text. Временные screenshots удалены после сравнения, потому что browser chrome содержал названия несвязанных пользовательских tabs | n/a | UI diff проверен визуально без сохранения несвязанных данных в repository |
| Mock auth flow | Safari: login `200` → redirect → refresh `/developers` → logout | После refresh `Вийти` восстановлен по `isAdmin`; после logout обе кнопки исчезли | n/a | Central session detection и client logout state работают на route без прежнего page effect |
| Disposable PostgreSQL | `docker run ... postgres:17`; `pg_isready` | Local PostgreSQL принимала connections | 0 | Real API proof использует изолированный local target |
| Migrations | Infrastructure-only `dotnet ef database update` с local `DB_CLOUD` | Применены все 24 migrations | 0 | Disposable DB имеет current schema |
| Synthetic Basic Admin helper | Temporary .NET 9 helper restore/build; generated hash piped прямо в local `psql` | 0 warnings, 0 errors; insert завершён без вывода password/hash | 0 | Real login использует application-compatible password hash |
| Real API browser E2E | Safari + real Vite/API/PostgreSQL: login → Home → full refresh → logout → full refresh | Login `200`; refresh `isAdmin` `200`; logout `200`; post-logout refresh `isAdmin` `401`; logout/admin controls отсутствуют | n/a | Полный FEAT-01 Basic Admin auth vertical и invalidated cookie access |
| Automation retry diagnosis | Первый real browser submit через secure-field `set_value` | API получил пустой password и вернул validation `400`; real keystrokes отправили full payload и login получил `200` | n/a | Первая попытка была ограничением UI automation, не product failure |
| Frontend production build | `npm run build` | 535 modules transformed; build завершён | 0 | TypeScript и production bundle собираются |
| Targeted ESLint | `npx eslint` для девяти FEAT-01 source files | 0 errors, 0 warnings | 0 | Изменённый frontend scope соответствует lint rules |
| Full ESLint | `npm run lint` | Existing baseline: 5 errors, 14 warnings; errors только вне FEAT-01 files | 1 | Full baseline измерен; несвязанный `QA-03` debt не скрыт |
| Backend restore | `dotnet restore ... --no-cache --disable-build-servers` | 5 projects restored; 2 existing ImageSharp advisories | 0 | Backend dependencies разрешаются без package changes |
| Solution Release build | `dotnet build ...sln --no-restore --configuration Release` | 0 errors, 2 existing advisory warnings | 0 | Full backend собирается |
| API Release build | `dotnet build ...Api.csproj --no-restore --configuration Release` | 0 errors, 2 existing advisory warnings | 0 | Startup project собирается |
| Backend tests, diagnostic | `dotnet test ... --no-restore --no-build` без `DB_CLOUD` | 16/17 startup failures: `Environment variable not found: DB_CLOUD` | 1 | Required runtime gate сработал; assertion regression не диагностировалась |
| Backend tests, final | Та же команда с disposable local `DB_CLOUD` | 17 passed, 0 failed/skipped | 0 | Auth/CORS/CSRF/logout integration suite green |
| Bundle/local secret scan | `rg` по production bundle и FEAT-01 source/evidence для test URL, synthetic credential и local helper marker | Совпадений нет | 1 (expected no matches) | Temporary local values не попали в source, evidence или bundle |
| Cleanup | `lsof` для `5173`/`5268`; filtered `docker ps -a`; helper path check | Listeners и test container отсутствуют; helper удалён | 0 | Disposable verification environment остановлена и удалена |
| Diff whitespace | `git diff --check` | Нарушений нет | 0 | Diff не содержит whitespace errors |
| Scope/no commit | `git status --short`; `git rev-parse --short HEAD` | `FEAT-02` files не менялись; pre-existing `API-02`/`DATA-03` work сохранён; HEAD остался `23140c4` | 0 | Выполнена только `FEAT-01`; commit не создан |

## Невыполненные проверки

- Production/staging cookie domain, HTTPS и browser topology не проверялись: запрещены scope FEAT-01 и остаются deployment gates `M5`/`M6`.
- Component test suite frontend отсутствует в repository; FEAT-01 проверена required real-browser E2E, production build и targeted lint без добавления test dependency.
- Full ESLint не green из-за пяти pre-existing errors вне FEAT-01 files; их исправление относится к `QA-03` и запрещено surgical scope.

## Проблемы вне scope

- `FrameLayout.tsx`, `Header.tsx`, `NoData.tsx` и `PaginationSection.tsx` сохраняют пять `@ts-ignore` lint errors; не блокируют FEAT-01.
- Full lint сохраняет 14 existing hook warnings вне FEAT-01 files; не блокируют FEAT-01.
- `SixLabors.ImageSharp 3.1.6` выдаёт existing `NU1902`/`NU1903`; package update не требовался FEAT-01 и не выполнялся.
- `ProtectedRoute` по-прежнему доверяет client role; это следующий backlog item `FEAT-02`, не FEAT-01.

## Итог критериев приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| UI отражает server session | выполнен | Real login и refresh дали `isAdmin 200`; admin controls восстановились |
| Logout инвалидирует доступ | выполнен | Logout `200`; следующий `isAdmin` получил `401`; admin controls исчезли |
| Ошибки понятны | выполнен | `400`, `401`, other HTTP и network states имеют отдельные user messages; `401` проверен browser RED/GREEN |
| E2E Basic Admin auth | выполнен | Real Safari + Vite + API + PostgreSQL прошли login → refresh/isAdmin → logout → rejected session |

Все критерии `FEAT-01` выполнены. Следующая задача `FEAT-02` не начиналась.
