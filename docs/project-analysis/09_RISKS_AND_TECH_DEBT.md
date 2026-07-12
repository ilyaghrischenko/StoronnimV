# Риски и технический долг

Severity: Critical блокирует основной production-сценарий; High серьёзно влияет на безопасность/данные/основной поток; Medium ухудшает сопровождение/частный сценарий; Low локальная проблема.

## Подтверждённые дефекты

| Категория | Severity | Доказательство | Влияние | Уверенность | Дополнительная проверка |
|---|---:|---|---|---|---|
| Integration | Critical | `GlobalContext.tsx` — hardcoded localhost | deployed client обращается к машине пользователя | Подтверждено | inspect built bundle/network |
| Mobile | Critical | runtime CSS/App/Frame `min-width:1100px` | narrow viewport не получает usable layout | Подтверждено | device screenshots |
| Admin state | Medium | `AdminContext.deleteAdmin` не сохраняет filter | удалённая строка остаётся в UI | Подтверждено | component integration test |
| Password edit | High | frontend допускает только equal old/new; validator требует different | запрос логически не может пройти | Подтверждено | API/UI test |
| Background data | High | `List.ForEach(async ...)` | Hangfire job не ждёт updates | Подтверждено | job test with multiple rows |
| Tests | High | test project без files/ProjectReference | regression protection отсутствует | Подтверждено | — |
| Quality gate | Medium | ESLint: 6 errors, 20 warnings | lint gate красный, hook risks | Подтверждено запуском | CI lint |

## Вероятные дефекты

| Категория | Severity | Доказательство | Влияние | Уверенность | Проверка |
|---|---:|---|---|---|---|
| API contracts | Critical | 9 forms send FormData as JSON to `[FromBody]` | mutations 400/415 | Высокая | integration requests |
| SuperAdmin auth | High | role-only policy, no `UseAuthentication` | SuperAdmin API unreachable | Высокая | valid JWT tests |
| News date | Medium | browser `yyyy-MM-dd` vs backend `dd.MM.yyyy` fallback | silently wrong date | Высокая | create news test |
| Static hosting | Medium | no global navigation fallback, `/error` omitted | direct deep links may 404 | Средняя | Azure deployment test |
| Schedule model | Medium | frontend requires `status`, response omits | inconsistent UI model | Высокая | inspect runtime JSON/UI |

## Безопасность

| Severity | Риск | Доказательство | Влияние | Проверка |
|---:|---|---|---|---|
| High | Hangfire dashboard без auth filter | `Program.cs` | operational control/data exposure | deployed route check |
| High | Cookie `SameSite=None` без antiforgery | cookie settings/admin endpoints | CSRF на mutations | cross-site PoC in controlled env |
| High | shared global rate limit | fixed policy partition | один client блокирует всех | concurrent client test |
| High | basic-admin mutations не проверяют Type | `SuperAdminService` | SuperAdmin record может быть затронут | service tests |
| Medium | raw exception messages | `ExceptionMiddleware` | internal details leak | error-path requests |
| Medium | login enumeration | `AccountService` messages | username discovery | login response check |
| Medium | media validation отсутствует | addition DTO/services/blob | oversized/unexpected uploads | upload tests |
| Medium | historical logs tracked | `backend/.../logs` | privacy/repository noise | secret/data scan by owner |

## Архитектура и целостность данных

- **High:** DB↔Blob operations без transaction/compensation; partial rows/orphan blobs.
- **High:** promotion replacement delete-before-create может оставить сайт без promo.
- **Medium:** несколько GroupPage rows допустимы, read выбирает arbitrary first.
- **Medium:** no optimistic concurrency; generic update marks all fields.
- **Medium:** `UpdatedAt init` не отражает edits.
- **Medium:** migrations не входят в startup/deployment workflow, SuperAdmin seed отсутствует.

Доказательства: entity services, `Repository.UpdateAsync`, `DatabaseInitializer`, EF snapshot. Проверка: fault-injection integration tests с real/test DB+Azurite.

## Поддерживаемость и testability

- Runtime CSS, SCSS sources, CSS maps и `dist` расходятся; pipeline source of truth не определён.
- README versions/env/mobile claims устарели.
- AutoMapper registration/validation дублируется и проверяет разные наборы profiles.
- Error contracts plain text/ProblemDetails различаются.
- Global Context объединяет HTTP, auth, modal и loading; shared loading race.
- Infrastructure project содержит Windows-specific HintPath.
- Git history — два commit, почти весь проект импортирован одним commit; intent невозможно восстановить.

## Производительность

- Blob delete-by-name enumerates whole container.
- `<video preload="auto">`, autoplay/Swiper/motion могут быть тяжёлыми на mobile.
- Отсутствуют request cancellation/cache, independent requests повторяются.
- Images в основном без lazy loading/responsive sources.

## Deployment

- CI/CD workflow отсутствует; есть только Dockerfile и static host routes.
- Docker build, runtime environment, migrations и external services не проверены.
- CORS принимает один exact origin; cookie domain/HTTPS должны совпасть.
- Закоммиченный `dist` потенциально устарел относительно source.

## Неизвестные области

Production cloud state, database migration level, blob ACL/content, deployed commit, real secrets/rotation, actual Hangfire exposure, monitoring/backup/recovery. Они не оцениваются как дефекты без проверки.
