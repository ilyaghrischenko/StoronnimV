# Evidence API-04 — Schedule job и production dashboard gate

**Дата проверки:** 15 июля 2026 года
**Итог:** `done`

## Цель и исходное состояние

`API-04` должна гарантировать, что daily Hangfire job не завершается до обновления всех просроченных `Active` Schedule, повторный запуск безопасен, а `/hangfire` недоступен в Production.

До изменения `ScheduleService.UpdateStatusesAsync` запускал repository writes через `List.ForEach(async ...)` и возвращался до их завершения. Сравнение с `DateTime.UtcNow.Date` не считало завершившееся сегодня событие просроченным. `Program.cs` подключал dashboard дважды и во всех environments; static `RecurringJob.AddOrUpdate` неявно зависел от инициализации `JobStorage.Current` dashboard middleware.

Production DB, Blob и другие production resources не использовались. Runtime-проверки выполнены на disposable localhost PostgreSQL 17 и Azurite.

## Затронутые файлы

- `backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/ScheduleService.cs`
- `backend/StoronnimV.Server/StoronnimV.Api/Program.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Application/ScheduleStatusUpdaterTests.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/HangfireDashboardIntegrationTests.cs`
- `docs/implementation/evidence/API-04.md`
- `docs/implementation/00_INDEX.md`
- `docs/implementation/04_BACKLOG.md`
- `docs/implementation/09_STATE.md`
- `docs/implementation/10_RUNTIME_CONTRACT.md`

## Решения и выполненные изменения

- Expiration сравнивается с текущим UTC instant, а не с началом UTC-дня.
- Просроченные `Active` records обновляются последовательным `foreach` с `await`. Это совместимо с одним scoped EF `DbContext`; concurrent `Task.WhenAll` для общего context не используется.
- `Passed`, `Cancelled` и future records не меняются. После первого запуска изменённые records больше не соответствуют `Active`, поэтому повторный запуск idempotent.
- Hangfire dashboard регистрируется один раз и только когда environment не Production.
- Recurring job регистрируется через DI `IRecurringJobManager`; static `JobStorage.Current` больше не требуется.
- Добавлены service/job tests completion, expired/future/status/idempotence и Production host route test.

## RED/GREEN evidence

| Сценарий | RED | GREEN |
|---|---|---|
| Job ждёт repository writes | `Assert.False`: expected `false`, actual `true` | job task остаётся незавершённым до release всех writes; 2 completed updates |
| Expired сегодня | expected `Passed`, actual `Active` | запись с текущей UTC-полуночи становится `Passed` |
| Production `/hangfire` | expected `404`, actual `500` | Production host возвращает `404` |
| Registration без dashboard side effect | после первого gate-fix startup дал `Current JobStorage instance has not been initialized yet` | DI `IRecurringJobManager` регистрирует job; Production host стартует |

## Команды и результаты

В командах ниже connection strings заменены placeholders; фактические values указывали только на disposable localhost services.

| Команда/сценарий | Результат | Exit code | Что доказывает |
|---|---|---:|---|
| Targeted RED `dotnet test ... --filter FullyQualifiedName~ScheduleStatusUpdaterTests` | 0/2 passed; ранний возврат и expired-today воспроизведены | 1 | Тесты ловят исходные дефекты |
| Targeted GREEN той же suite | 2/2 passed | 0 | Job wrapper ждёт updates; selection и idempotence корректны |
| Production route RED с `API04_INTEGRATION=1` | expected `404`, actual `500` | 1 | Dashboard pipeline был доступен в Production |
| Production route GREEN с `API04_INTEGRATION=1` | 1/1 passed | 0 | Production `/hangfire` отсутствует; DI job registration не ломает startup |
| `dotnet tool restore` | local `dotnet-ef` 9.0.7 restored | 0 | Использован repository-local EF tool |
| `dotnet ef database update --project ...Infrastructure.csproj --startup-project ...Infrastructure.csproj --context StoronnimVContext` | все 24 migrations применены к disposable PostgreSQL | 0 | Test DB подготовлена утверждённым Infrastructure-only workflow |
| Clean solution `dotnet restore ... --no-cache --artifacts-path /tmp/storonnimv-api04-final-019f62fb/artifacts --disable-build-servers` | 0 errors; existing `NU1902`/`NU1903` advisories | 0 | Dependencies восстанавливаются на clean artifacts path |
| Release solution build с тем же artifacts path | 0 errors, 7 existing warnings | 0 | Вся backend solution компилируется |
| Release API build с тем же artifacts path | 0 errors, 2 existing advisories | 0 | Изменённый startup project компилируется |
| Full backend suite с `API04_INTEGRATION=1 DATA04_INTEGRATION=1 FEAT04_INTEGRATION=1 FEAT05_INTEGRATION=1` | 97/97 passed, 0 skipped | 0 | API-04 и все существующие backend regressions green на PostgreSQL/Azurite |
| `git diff --check` | ошибок нет | 0 | Diff не содержит whitespace errors |
| Scoped secret regex scan code/test/docs | распознаваемых secrets нет | 1, ожидаемый no-match | Task files не содержат private keys, assigned passwords/API secrets |
| Final status/diff/scope review | только 9 API-04 files; commit/branch change отсутствуют | 0 | Scope ограничен API-04; следующая задача не начата |
| `docker stop` + filtered `docker ps` | оба disposable containers остановлены; remaining `none` | 0 | Local test infrastructure очищена |

## Невыполненные проверки

- Production/staging deployment smoke не выполнялся: это gate `M5/M6`, production access не разрешён и для критериев `API-04` не нужен.
- Реальный daily clock trigger не ожидался 24 часа: Hangfire registration доказана успешным Production host startup; job completion/idempotence покрыты детерминированными service/job tests.

## Проблемы вне scope

- Existing `SixLabors.ImageSharp 3.1.6` advisories `NU1902`/`NU1903` и существующие compiler warnings не созданы `API-04`; packages не обновлялись.
- Первый sandboxed targeted test не дошёл до test run из-за MSBuild named-pipe `Permission denied`; обязательные команды повторены вне sandbox и завершены.
- Первый full-suite run дал 35 unrelated auth failures из-за process-global test JWT issuer race, созданного новым Production factory. Test factory приведён к существующим `AuthApiFactory` values; повторный full run — 97/97. Production auth code не менялся.

## Критерии приёмки

| Критерий | Итог |
|---|---|
| Несколько expired rows обновляются до завершения job | `passed`: job-wrapper test блокирует writes и подтверждает 2 completed updates до return |
| Future/non-Active records не меняются | `passed`: Active future, Cancelled и Passed сохраняют статусы |
| Повторный запуск безопасен | `passed`: второй run не добавляет repository updates |
| Dashboard отсутствует в Production | `passed`: real Production host `/hangfire` возвращает `404` |
| Hangfire registration работает без dashboard side effect | `passed`: `IRecurringJobManager` startup прошёл на disposable PostgreSQL |
| Backend regression suite | `passed`: 97/97, 0 skipped |

Все критерии `API-04` выполнены. Статус: `done`. Следующая задача backlog — `FEAT-06`; она не начиналась. Коммит не создавался.
