# API-03 — Request и error contracts

## Цель

Разблокировать CRUD forms едиными HTTP-контрактами: JSON для body-only mutations, multipart для file-bearing requests, ISO dates без silent fallback, согласованные nullable/status public DTO и один предсказуемый error shape.

## Исходное состояние

- Девять text-only forms и отдельная news-video mutation создавали `FormData`, но объявляли `application/json`; backend ожидал `[FromBody]` JSON.
- News create принимал browser `yyyy-MM-dd`, но service разбирал `dd.MM.yyyy` и при ошибке молча подставлял текущую дату.
- Schedule edit формировал время через точку, а service ожидал двоеточие.
- `ScheduleShortResponse` не содержал `status`, хотя TypeScript model его требовала.
- Nullable news/schedule media и nullable Home schedule/video были объявлены non-null в части C#/TypeScript contracts.
- Exceptions возвращали `text/plain`; model validation использовала другой JSON shape; framework `401`/`403`/`404` и antiforgery `400` имели пустое body.
- Worktree уже содержал незакоммиченные изменения `DATA-03`, `API-02`, `FEAT-01` и `FEAT-02`; они сохранены. Production/remote resources не использовались.

## Затронутые файлы

- Backend HTTP/error composition: `StoronnimV.Api/Models/ApiErrorResponse.cs`, `Middlewares/ExceptionMiddleware.cs`, `Program.cs`.
- Backend request contracts/services: news/schedule addition/edit DTO, `NewsService.cs`, `ScheduleService.cs`.
- Backend public contracts: `HomeController.cs`, `IHomeControllerService.cs`, `HomeControllerService.cs`, Home response DTO, schedule short DTO/mapping/projection/repository.
- Frontend requests: news, schedule, video, group, member, social и group-social edit/add forms.
- Frontend public models/rendering: Home/News/Schedule models, `ScheduleListItem.tsx`, `ScheduleModal.tsx`.
- Tests: `ApiContractIntegrationTests.cs`.
- State/docs: этот evidence, `00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`.

## Решения

- Backend routes сохраняют существующее разделение: `[FromBody]` получает JSON object; `[FromForm]` остаётся только для multipart/file flows. API целиком не переписывался.
- News requests используют `DateOnly`, schedule requests — `DateTime`; browser отправляет ISO `yyyy-MM-dd` и `yyyy-MM-ddTHH:mm`. Invalid/missing date отклоняется binding validation вместо подмены даты.
- Error response всегда имеет `status`, `title`, `detail`, `instance`, `errors`; media type — `application/problem+json`. Validation заполняет `errors`, остальные paths возвращают пустой dictionary.
- Generic `500` не возвращает exception detail. Known 4xx exceptions сохраняют безопасное прикладное сообщение.
- Empty framework errors заполняются через status-code pages; FluentValidation/model binding используют тот же shape.
- Public nullability исправлена на C# и TypeScript sides; schedule list projection/response теперь содержит `status`.
- Upload validation/DB↔Blob consistency, CRUD readback и Home state UX остаются соответственно `DATA-04`, `FEAT-04`/`FEAT-05` и `FEAT-03`.

## Выполненные изменения

1. Text-only forms переведены с mislabeled `FormData` на JSON objects; file-bearing calls сохранены multipart.
2. News/schedule request dates стали typed ISO contracts; silent current-date fallback и несовместимый schedule format удалены.
3. Добавлен единый problem JSON для validation, exceptions, antiforgery/framework `400`, `401`, `403`, `404`, `415` и `500`.
4. Generic server errors больше не раскрывают exception detail клиенту.
5. Schedule list DTO/projection получил `status`; nullable public media и Home results синхронизированы с TypeScript.
6. Добавлена HTTP integration matrix: десять body-bound routes, news/schedule multipart dates, invalid и missing dates, validation/framework/exception error classes, nullable Home wire responses, server-detail redaction и public DTO reflection.

## Проверки

Все runtime tests выполнялись 14 июля 2026 года только с disposable local PostgreSQL 17 на localhost. Blob/production data не использовались.
После финальных тестов контейнер остановлен; `docker ps -a` подтвердил его отсутствие.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| TDD RED | Targeted `dotnet test ... --filter FullyQualifiedName~ApiContractIntegrationTests` с disposable local PostgreSQL до implementation | 11 passed, 9 failed: invalid dates принимались; errors были empty/plain text; schedule status/nullability отсутствовали | 1 | Tests реально обнаруживали отсутствующее API-03 behavior |
| Review RED | Та же targeted command после добавления regression cases для missing dates и nullable Home wire responses | 22 passed, 4 failed: missing dates проходили form binding; `Ok(null)` сериализовался как `204` | 1 | Финальное ревью обнаружило два реальных contract gap до закрытия задачи |
| Contract GREEN | Та же targeted command после implementation и review corrections | 26 passed, 0 failed/skipped | 0 | JSON/multipart binding, required ISO dates, nullable `200` JSON, unified errors, redaction и public DTO contracts green |
| Backend restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --disable-build-servers` | 5 projects restored; existing ImageSharp advisories `NU1902`/`NU1903` | 0 | Dependencies разрешаются без package changes |
| Solution Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --disable-build-servers` | Build succeeded, 0 errors, 2 existing advisories | 0 | Полный backend компилируется |
| Full backend tests | `dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --no-build --configuration Release --disable-build-servers` с disposable local `DB_CLOUD` | 47 passed, 0 failed/skipped | 0 | Полный auth/CSRF/role/API-contract regression suite green |
| Frontend build | `npm run build` | TypeScript + Vite: 535 modules transformed | 0 | Изменённые forms/models типизируются; production bundle строится |
| Targeted frontend lint | `npm exec -- eslint <API-03 frontend files>` | 0 errors, 1 pre-existing `ScheduleModal` hook warning | 0 | API-03 request/type edits не добавили lint errors |
| Full frontend lint | `npm run lint` | Existing baseline: 5 errors, 13 warnings | 1 | Repo-wide baseline измерен; ошибки находятся вне API-03 files, один existing warning — в touched display file |
| Request source audit | `rg` по `FormData`, `Content-Type` и изменённым forms | JSON routes получают objects; remaining `new FormData()` calls относятся к multipart file routes | 0 | Старый FormData-as-JSON mismatch отсутствует |
| Bundle contract scan | Search built `dist` for hardcoded legacy/test endpoints and test-only literals | Forbidden values отсутствуют | 0 | Test/local contract values не встроены в bundle |
| Secret scan | Strong credential/private-key pattern scan с exclusions generated/vendor dirs | Совпадений нет | 0 | Изменения не добавили распознаваемые secrets |
| Diff whitespace | `git diff --check` | Нарушений нет | 0 | Итоговый diff не содержит whitespace errors |

Первый sandboxed RED запуск не дошёл до tests: MSBuild не смог создать local IPC pipe. Первый запуск с dummy closed DB не дошёл до app: Hangfire получил connection refused. Повтор вне sandbox с disposable localhost PostgreSQL дал валидный RED; source обходы environment gate не добавлялись.

## Невыполненные проверки

- Browser screenshot before/after не выполнялся: API-03 не меняет CSS, layout или copy; изменения касаются payload transport, DTO typing, error JSON и отсутствия `<img>` при `null` URL. Визуальная CRUD/readback проверка остаётся в feature E2E задачах после `DATA-04`.
- Полный frontend lint не green из-за существующего baseline 5 errors/13 warnings. Их исправление относится к `QA-03`; API-03 не добавила errors.
- Production/staging contracts не проверялись: production access запрещён и относится к M5/M6.
- Полный content/media CRUD readback не выполнялся: upload policy/consistency ещё `DATA-04`, vertical E2E — `FEAT-04`–`FEAT-07`.

## Проблемы вне scope

- `SixLabors.ImageSharp 3.1.6` сохраняет existing `NU1902`/`NU1903`; package update не требовался API-03.
- Full ESLint сохраняет 5 `@ts-ignore` errors и 13 hook warnings; один warning в `ScheduleModal.tsx` предшествовал nullable rendering edit.
- Existing accessibility debt: schedule images не имеют `alt`; исправляется в утверждённой accessibility/QA работе, не в API contract task.
- Hangfire startup требует доступный PostgreSQL даже для HTTP contract tests; disposable local DB использована без production access.

Ни одна проблема не блокирует API-03 acceptance.

## Итог по критериям приёмки

| Критерий | Итог |
|---|---|
| Все затронутые requests bind | Выполнен: 10-route JSON matrix + required ISO news/schedule multipart tests; 26/26 contract tests |
| Validation errors предсказуемы | Выполнен: один problem JSON shape для model/FluentValidation, framework/auth и exception paths; invalid dates дают `400` + `errors` |
| Public DTO совпадают с TypeScript | Выполнен: nullable media/Home contracts синхронизированы; schedule list содержит `status` |
| JSON vs multipart согласованы | Выполнен: objects для body routes, `FormData` только для file-bearing routes |
| Dates не подменяются | Выполнен: typed ISO binding; invalid values отклоняются до service |

Все критерии `API-03` выполнены. Статус: `done`. Следующая задача backlog — `DATA-04`; она не начиналась.
