# DATA-04 — Upload policy и DB/Blob consistency evidence

## Цель и границы

Предотвратить потерю активного media: ввести подтверждённую upload policy, проверять файл до Blob write и закрепить безопасный порядок create/replace/delete между PostgreSQL metadata и Azure Blob Storage. CDN, новая storage platform, antivirus/content scanning, UI/CRUD verticals и production resources не входят в DATA-04.

## Решение владельца

14 июля 2026 года владелец подтвердил:

- JPEG, PNG и WebP — до 10 MiB включительно;
- MP4 — до 250 MiB включительно;
- выбран меньший риск ложного отказа ценой большего resource risk.

Значения находятся в `MediaUpload` configuration, но startup validation hard-caps их подтверждёнными maxima. Уменьшение policy допустимо; повышение или неподдерживаемый MIME останавливает startup.

## Реализованный contract

- Проверяются non-empty length, configured/hard maximum, exact extension↔MIME pair и magic signature: JPEG, PNG, RIFF/WEBP, ISO-BMFF `ftyp` для MP4.
- Kestrel/form body limit равен 250 MiB + 1 MiB multipart overhead.
- Blob получает validated content type; имя уникально, поэтому replacement не перезаписывает старый object.
- Create: validate → upload new Blob → insert DB; DB failure удаляет new Blob.
- Replace: parse old URL → validate/upload new Blob → change DB pointer → delete old Blob. DB failure удаляет new Blob; old DB pointer/Blob сохраняются.
- Delete: parse old URL → delete DB metadata → delete old Blob. DB failure сохраняет Blob.
- Post-commit cleanup использует `CancellationToken.None`, поэтому client disconnect не отменяет cleanup после DB success.
- Blob cleanup failure не приводит к DB pointer на отсутствующий object: остаётся safe orphan, а exception содержит exact container/blob для cleanup. Если rollback нового Blob тоже падает, `MediaConsistencyException` сохраняет original и cleanup failures.
- Promotion replacement выполняет add-new/delete-old в одной PostgreSQL transaction; old promotion Blob удаляется только после commit.
- Legacy/malformed media URL вроде `default` отклоняется до upload или DB mutation.

## Затронутые области

- `StoronnimV.Application`: media options, validator, storage coordinator, typed exceptions и entity services.
- `StoronnimV.Domain`/`Infrastructure`: Blob content type/exact delete и atomic promotion replacement repository method.
- `StoronnimV.Api`: DI, startup options validation и request/form limits.
- `StoronnimV.Tests/Application`: policy/signature boundaries, fault order, cancellation, promotion rollback и real PostgreSQL/Azurite integration tests.
- Canonical docs: `02_DECISIONS.md`, `04_BACKLOG.md`, `08_OPEN_ITEMS.md`, `09_STATE.md`, `10_RUNTIME_CONTRACT.md`, `00_INDEX.md` и этот evidence.

Frontend, schema/migrations и package versions не менялись.

## TDD и review corrections

| Gate | Результат | Exit code | Что доказано |
|---|---|---:|---|
| Initial RED | Targeted test compile не нашёл ещё не созданные media contracts/options/services | 1 | Tests были написаны до implementation |
| Initial media GREEN | Validator/storage/promotion/config targeted suite: 33/33 | 0 | Основные policy и compensation branches green |
| Review RED: hard caps/cancellation | 3 failed из 16: values выше 10/250 MiB принимались; cancelled request прерывал post-DB cleanup | 1 | Independent review нашёл реальные policy/lifecycle gaps |
| Review GREEN | Те же config/storage tests: 16/16 | 0 | Hard maxima и non-cancellable post-commit cleanup закреплены |
| Review RED: legacy URL order | 2 failed из 11: `default` разбирался после upload/DB mutation | 1 | Ordering regression воспроизводилась |
| Review GREEN | `MediaStorageServiceTests`: 11/11 | 0 | Malformed URL теперь fail-fast до side effects |
| Final independent review | Critical 0; Important 0; verdict `Ready for DATA-04 acceptance` | — | Финальный diff соответствует acceptance |

## Real integration и fault injection

Disposable PostgreSQL 17 и Azurite были доступны только через `127.0.0.1`. `DATA04_INTEGRATION=1` включает два committed integration tests; без явного opt-in они динамически skipped, чтобы обычный unit run не обращался к внешнему state.

`MediaPersistenceIntegrationTests` доказали:

- create сохраняет совпадающие DB URL и Blob с `image/jpeg`;
- wrong signature отклоняется без DB/Blob mutation;
- реальный PostgreSQL constraint failure после Blob upload удаляет new Blob;
- replace переключает DB на PNG, сохраняет `image/png` и удаляет old Blob только после DB success;
- delete удаляет DB row, затем exact Blob;
- forced real transaction failure сохраняет old promotion row и не сохраняет invalid replacement;
- test-owned rows/Blobs удаляются в `finally`.

До committed integration tests отдельный real probe дал тот же lifecycle result:

```text
blob_create_content_type=image/jpeg
blob_replace_content_type=image/png
invalid_upload_blob_state=unchanged
create_db_failure_blob_state=rolled_back
delete_blob_count=0
promotion_transaction_success_count=1
promotion_transaction_failure_preserved_old=true
```

Targeted committed integration run: 2/2 passed, 0 failed/skipped, exit 0.

## Финальные проверки

Финальный restore/build/test использовал отдельные `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH` и artifacts path under `/tmp`; connection values не выводились и не записывались в Git.

| Проверка | Команда или сценарий | Результат | Exit code |
|---|---|---|---:|
| Clean restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --artifacts-path <isolated> --disable-build-servers` | Все 5 projects restored; 0 errors; existing ImageSharp `NU1902`/`NU1903` | 0 |
| Isolated Release build | `dotnet build ...sln --no-restore --configuration Release --artifacts-path <isolated> --disable-build-servers -m:1` | 0 errors; 5 warnings: 2 existing advisories + 3 existing nullable warnings | 0 |
| Full backend suite | `DATA04_INTEGRATION=1 dotnet test ...sln --no-restore --no-build --configuration Release --artifacts-path <isolated> --disable-build-servers -m:1` с disposable local PostgreSQL/Azurite | 87 passed, 0 failed, 0 skipped | 0 |
| Blob/PostgreSQL probe | Real lifecycle/fault scenarios listed above | Все expected invariants matched | 0 |
| Direct-call audit | `rg` по Blob repository calls и `OpenReadStream` | Entity services не обходят media coordinator | 0 |
| Diff whitespace | `git diff --check` | Нарушений нет | 0 |
| Secret scan | Task-specific connection/password/token values и private-key markers по source/docs | Совпадений нет; broad `AccountKey` scan находит только существующие safe template placeholders | 0 |
| Disposable cleanup | Exact DATA-04 PostgreSQL/Azurite containers остановлены; task-owned temp probe удалён | Task-owned services/artifacts не оставлены | 0 |

Первый non-isolated full run одновременно инициализировал Hangfire schema из нескольких test hosts и получил один existing PostgreSQL namespace race: 79/80. Повтор после schema initialization прошёл 80/80; финальный isolated serial run с расширенной DATA-04 suite прошёл 87/87. Source correction для этого environment race не выполнялась.

## Невыполненные проверки

- UI screenshots/browser flow не выполнялись: frontend/layout не менялись. CRUD browser readback относится к `FEAT-04`–`FEAT-08`.
- Production/staging Blob/DB не использовались; production topology и real content остаются `OPS-01`/`OPS-03`.
- Antivirus, full media decoding и polyglot detection не выполнялись: DATA-04 требует size/type/signature policy, не malware scanning.
- Durable outbox для rare safe orphan не добавлялся. При исчерпании Azure SDK retries exception фиксирует exact cleanup identity; schema/outbox расширение не требовалось критерием и не вводилось скрыто.

## Проблемы вне scope

- `SixLabors.ImageSharp 3.1.6` сохраняет existing `NU1902`/`NU1903` advisories.
- Existing nullable warnings остаются в `SocialResponse.cs` и `NewsService.cs`; DATA-04 их не создавала.
- Hangfire test-host schema initialization может race на первом параллельном запуске против чистой DB; serial/follow-up run green.

Ни одна проблема не блокирует DATA-04 acceptance.

## Итог по критериям приёмки

| Критерий | Итог |
|---|---|
| Configurable size/type/signature policy | Выполнен: подтверждённые types/maxima, startup validation, extension/MIME/signature checks |
| Invalid file отклоняется | Выполнен: oversize, empty, unsupported/mismatch/signature и real invalid upload cases |
| Fault injection не оставляет необъяснимое состояние | Выполнен: rollback нового Blob; exact orphan identity после post-commit cleanup failure; malformed URL fail-fast |
| Old file сохраняется до success | Выполнен: DB pointer меняется до old Blob delete; DB/transaction failure сохраняет old Blob/promotion |
| Create/replace/delete consistency | Выполнен unit fault matrix и real PostgreSQL/Azurite lifecycle |
| Regression suite green | Выполнен: 87/87, final review 0 Critical/Important |

Все критерии `DATA-04` выполнены. Статус: `done`. Следующая задача backlog — `FEAT-03`; она не начиналась. Коммит не создавался.
