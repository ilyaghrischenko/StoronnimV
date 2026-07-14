# FEAT-04 — News read и CRUD

**Дата проверки:** 14 июля 2026 года  
**Итог:** `done`

## Цель и исходное состояние

Задача проверяет один полный desktop News vertical: pagination/detail/create/edit/delete, photo/video actions и точную дату. До изменения invalid pagination возвращала `200`, out-of-range page теряла реальные totals, `videoId = null/0` доходил до service, admin не мог создать первую новость из empty state, а сохранённая в `sessionStorage` страница скрывала созданную запись после reload.

Production DB/Blob не использовались. Проверка выполнена на одноразовых localhost PostgreSQL 17 и Azurite с отдельными test values.

## Изменённые файлы

- `backend/StoronnimV.Server/StoronnimV.Api/Controllers/NewsController.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/DTO/Requests/Entities/Pages/Editing/Media/EntityVideoEditRequest.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/NewsService.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/ApiContractIntegrationTests.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/NewsCrudIntegrationTests.cs`
- `frontend/storonnimv.client/src/components/contexts/NewsContext.tsx`
- `frontend/storonnimv.client/src/components/elements/news/NewsList.tsx`
- `frontend/storonnimv.client/src/components/elements/news/NewsListItem.tsx`
- `frontend/storonnimv.client/src/components/elements/news/NewsModal.tsx`
- `frontend/storonnimv.client/src/components/elements/news/forms/AddNewsItemModal.tsx`
- `frontend/storonnimv.client/src/components/elements/news/forms/EditNewsItemModal.tsx`
- status/evidence: `docs/implementation/00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`, этот файл.

Изменения DATA-04 внутри `NewsService` сохранены: FEAT-04 добавляет поверх них только корректную pagination и non-null positive video ID contract.

## Решения и реализация

- `page` и `pageSize` валидируются как positive integers на HTTP boundary; invalid values дают unified `400`.
- Empty и out-of-range pages возвращают empty `items`, но сохраняют фактические `totalItems`/`totalPages`.
- News video attach требует `videoId >= 1`; `null` и `0` больше не превращаются в успешную mutation.
- News list стартует с page 1 после reload. Pagination остаётся state-driven без устаревших News totals/page в `sessionStorage`, поэтому create readback виден.
- Admin add action доступен и при пустом списке; error state имеет retry.
- Create/edit file picker согласован с DATA-04 photo policy: JPEG, PNG, WebP.
- News images и icon-only CRUD buttons получили содержательные accessible names.
- Добавлен opt-in real API integration test полного News lifecycle с PostgreSQL/Azurite и проверкой Blob state.

## RED → GREEN evidence

| Сценарий | До исправления | После исправления |
|---|---|---|
| `page=0`, `pageSize=0/-1` | три ответа `200` | три unified `400` contract tests |
| Empty News для admin | `Новин немає`, create action отсутствует | `Додати новину` доступна |
| Create с открытой page 2 | после reload осталась page 2, новая запись не видна | reload открывает page 1 и показывает созданную запись |
| Existing data + page 999 | `totalItems=0`, `totalPages=0` | empty items с сохранёнными реальными totals |
| `videoId=null/0` | mutation возвращала `204` | два unified `400` tests |

## Проверки

| Команда/сценарий | Результат |
|---|---|
| Targeted News ESLint | exit `0`, 0 findings |
| `VITE_API_URL=https://api.example.invalid/api npm run build` | exit `0`, 535 modules |
| Isolated backend restore до финального source review | exit `0`, только existing `NU1902`/`NU1903` advisories |
| Release solution build | exit `0`, 0 errors, 4 pre-existing warnings |
| Full backend suite с `FEAT04_INTEGRATION=1 DATA04_INTEGRATION=1` | exit `0`, 93/93 passed, 0 skipped |
| Real News CRUD integration | create → list/detail → text/date edit → photo replace/delete → video reattach/detach → delete/404; DB/Blob readback подтверждён |
| Controlled Chromium desktop E2E | pagination 1/2, empty create, exact `14.07.2026`, detail, edit to `15.08.2026`, real file chooser, photo/video actions, delete/readback прошли |
| Visual before/after | desktop frame/layout не изменён; empty state получил только admin add action; финальный список/пагинация визуально проверены |
| Bundle configured URL scan | configured `api.example.invalid` найден |
| Bundle forbidden URL scan | `localhost:44315` и mock `127.0.0.1:4177` не найдены |
| `git diff --check` | exit `0` |
| Scoped secret scan | совпадений нет (`rg` exit `1`) |

Browser console финального controlled flow не содержал app warnings/errors. Два favicon DNS errors относились к внешнему `storonnimv.com` в локальной mock-среде и не блокировали News vertical.

## Критерии приёмки

| Критерий | Результат |
|---|---|
| Pagination и detail | `passed`: valid/empty/out-of-range/invalid API behavior и browser pages/detail доказаны |
| Create/edit/delete | `passed`: real API integration и browser mutation/readback |
| Photo lifecycle | `passed`: create/replace/delete, old/new Blob assertions |
| Video lifecycle | `passed`: initial attach, reattach, detach и invalid ID contract |
| Дата не подменяется | `passed`: `2026-07-14` → `14.07.2026`, edit `2026-08-15` → `15.08.2026` |
| Mutation видна после readback | `passed`: browser reload page 1 и API detail/list/404 readback |
| Media consistent | `passed`: DB response и Azurite existence/deletion assertions |
| Search/tags не добавлены | `passed` |

## Не выполнено и почему

- Полная mobile/tablet/cross-browser release matrix не относится к desktop `M2` FEAT-04; она запланирована в `M3`/`M6`.
- Production/staging smoke не выполнялся: он требует отдельного доступа и относится к `M5`/`M6`.
- Full repository ESLint завершился exit `1`: 4 errors/8 warnings находятся вне FEAT-04 files и остаются задачей `QA-03`. Targeted FEAT-04 ESLint green.
- Повторный isolated restore при финальной проверке дважды завис без output внутри sandbox и был остановлен; это environment stall, потому что предшествующий isolated restore прошёл, а последующие Release build и 93/93 tests вне sandbox завершились успешно.

## Вне scope / известные findings

- Existing `SixLabors.ImageSharp 3.1.6` advisories `NU1902`/`NU1903` и два nullable warnings в `SocialResponse` не созданы FEAT-04.
- Shared click-only `GenericList/ListItem` требует отдельной cross-feature accessibility работы в `QA-05`; News-specific controls/images в этой задаче получили names/alts.
- Предшествующие незакоммиченные DATA-04/FEAT-03 изменения сохранены без reset/clean/stash.
- Коммит, branch change и следующая задача не выполнялись.
