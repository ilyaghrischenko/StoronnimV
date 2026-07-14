# FEAT-05 — Schedule read и CRUD

**Дата проверки:** 14 июля 2026 года
**Итог:** `done`

## Цель и исходное состояние

Задача закрывает один полный desktop Schedule vertical: pagination, list/detail, status/date/location/map, create/edit/delete и photo actions. До изменения schedule card скрывал title/date/location/status, list/detail не различали loading/empty/error, out-of-range page обнуляла реальные totals, `pageSize = 0` приводил к server failure, отдельного photo delete action не было, а edit datetime ломался после первого изменения из-за повторного преобразования уже ISO-formatted value.

Production DB/Blob не использовались. Проверка выполнена на одноразовых localhost PostgreSQL 17 и Azurite с отдельными test values.

## Изменённые файлы

- `backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/Contracts/Controllers/IAdminControllerService.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/Contracts/Entities/IScheduleService.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/Services/Controllers/AdminControllerService.cs`
- `backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/ScheduleService.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/ApiContractIntegrationTests.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/ScheduleCrudIntegrationTests.cs`
- `frontend/storonnimv.client/src/components/contexts/ScheduleContext.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/LocationMap.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/ScheduleListItem.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/ScheduleModal.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/SchedulesList.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/forms/AddScheduleModal.tsx`
- `frontend/storonnimv.client/src/components/elements/schedule/forms/EditScheduleModal.tsx`
- `frontend/storonnimv.client/src/styles/elements/schedule/ScheduleListItem.scss`
- `frontend/storonnimv.client/src/styles/style.css`
- status/evidence: `docs/implementation/00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`, этот файл.

Изменения DATA-04 внутри `ScheduleService` сохранены. FEAT-05 добавляет поверх них корректную pagination и compensated photo detach через существующий media lifecycle.

## Решения и реализация

- `page` и `pageSize` требуют positive integers; invalid values дают unified `400`.
- Empty/out-of-range pages возвращают empty `items`, сохраняя фактические `totalItems`/`totalPages`.
- Schedule list/detail имеют явные loading, empty, error и retry states без устаревшего `sessionStorage` gate.
- Card показывает title, exact date/time, location и status; card и icon-only CRUD controls имеют accessible names.
- Detail показывает location/status и map с encoded location и содержательным iframe title.
- Edit хранит `datetime-local` как стабильное ISO-local value; повторное изменение больше не очищает поле.
- Добавлен самостоятельный admin photo delete route. DB detach выполняется до Blob cleanup через подтверждённый DATA-04 compensation pattern.
- Create/edit file picker согласован с DATA-04 photo policy: JPEG, PNG, WebP.
- Добавлен opt-in real API integration test полного Schedule lifecycle с PostgreSQL/Azurite, public Blob read и cleanup.

## RED → GREEN evidence

| Сценарий | До исправления | После исправления |
|---|---|---|
| Existing data + page 999 | `totalItems=0`, `totalPages=0` | empty items с сохранёнными реальными totals |
| `page=0` / `pageSize=0` | invalid page / server failure | unified `400` |
| Photo detach | route отсутствовал, `405` | `204`, response photo `null`, Blob удалён |
| Schedule card | видна только картинка | видны title/date/location/status |
| Datetime edit | после первого change formatter очищал value | `2037-09-16T18:20` стабильно редактируется и сохраняется |

## Проверки

| Команда/сценарий | Результат |
|---|---|
| Targeted Schedule ESLint | exit `0`, 0 findings |
| `npm run build` | exit `0`, 535 modules |
| Solution restore | exit `0`, только existing `NU1902`/`NU1903` advisories |
| Release solution build | exit `0`, 0 errors, 2 existing advisory warnings |
| Full backend suite с `FEAT05_INTEGRATION=1 FEAT04_INTEGRATION=1 DATA04_INTEGRATION=1` | exit `0`, 94/94 passed, 0 skipped |
| Targeted real Schedule CRUD integration | exit `0`, 1/1 passed |
| Real API/PostgreSQL/Azurite lifecycle | create → list/detail → text/date/location edit → photo replace/delete → entity delete/404; DB/Blob readback подтверждён |
| Public media check | созданный photo URL вернул HTTP `200` при Blob public-read policy |
| Controlled WebKit desktop E2E | create, exact date/location/status readback, detail/map, edit, photo delete и entity delete/readback прошли |
| Map inspection | title `Карта: Kyiv, Ukraine`; encoded `q=Kyiv%2C%20Ukraine`; iframe визуально отрисован |
| Visual before/after | до изменения card показывала только photo; после — читаемый overlay title/date/location/status; detail показывает location/status/map |
| Bundle forbidden-value scan | localhost API и FEAT-05 test markers не найдены (`rg` exit `1`) |
| `git diff --check` | exit `0` |
| Scoped secret scan | совпадений нет (`rg` exit `1`) |

Browser console обнаружил внешний `storonnimv.com/site.webmanifest` DNS error в localhost-среде. Первоначальный Azurite photo URL дал `403` из-за private test-container ACL; тестовый setup исправлен на production-compatible public Blob read и закреплён HTTP `200` assertion. Оба наблюдения не потребовали production access.

## Критерии приёмки

| Критерий | Результат |
|---|---|
| List/detail/status | `passed`: API и browser readback показывают точные поля |
| Date/time | `passed`: create `16.09.2037 18:20`, edit `17.10.2037 21:35`; integration test отдельно подтвердил 2036 values |
| Map/location | `passed`: location readback, encoded query, title и визуальный iframe |
| Create/edit/delete | `passed`: real API integration и browser mutation/readback |
| Photo lifecycle | `passed`: create/public read/replace/delete и old/new Blob assertions |
| Empty/error/loading/retry | `passed`: явные state branches; targeted lint/build green |
| Mutation видна после readback | `passed`: list/detail/photo-null/delete absence и API 404 |
| Ticketing не добавлен | `passed` |

## Не выполнено и почему

- Mobile/tablet и полная cross-browser release matrix относятся к `M3`/`M6`, не к desktop `M2` FEAT-05.
- Production/staging smoke не выполнялся: он требует отдельного доступа и относится к `M5`/`M6`.
- Full repository ESLint завершился exit `1`: 4 errors/6 warnings находятся вне FEAT-05 files и остаются задачей `QA-03`. Targeted Schedule ESLint green.
- Первый sandboxed solution restore/build завис без output и был остановлен. Повторные команды вне sandbox завершились успешно; это environment/IPC stall, не source failure.

## Вне scope / известные findings

- Existing `SixLabors.ImageSharp 3.1.6` advisories `NU1902`/`NU1903` не созданы FEAT-05.
- Автоматическое обновление expired Schedule statuses и production Hangfire dashboard gate остаются следующей задачей `API-04`; она не начиналась.
- Предшествующие незакоммиченные DATA-04/FEAT-03/FEAT-04 изменения сохранены без reset/clean/stash.
- Коммит и branch change не выполнялись.
