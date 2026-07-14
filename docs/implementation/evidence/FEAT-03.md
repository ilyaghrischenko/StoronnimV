# FEAT-03 — Home and shared-state evidence

## Цель и границы

Завершить Home: безопасно обработать nullable schedule/video, сделать loading, empty, error и retry наблюдаемыми независимо для трёх параллельных секций и дать данным семантические переходы в Schedule, News и соответствующую Video category.

- Desktop visual baseline сохранён; добавлены только требуемые retry/navigation controls.
- Backend contracts, CRUD, mobile layout, frontend test framework и следующие backlog tasks не менялись.
- Production DB/Blob не читались и не изменялись. Использовались controlled localhost mock API и disposable PostgreSQL 17.
- Коммит и новая ветка не создавались.

## Исходное состояние

- `FEAT-03` существовала со статусом `planned`; зависимости `QA-01` и `API-03` имели статус `done`.
- Worktree уже содержал пользовательские изменения `DATA-04` в backend/docs; они сохранены и не редактировались как часть `FEAT-03`.
- Home уже различал loading/empty/error по секциям и принимал nullable schedule/video после `API-03`, но error state не имел retry.
- Schedule использовал click-only container; News и promotion video не имели семантических ссылок в соответствующие разделы.
- Home effects оставляли три `react-hooks/exhaustive-deps` warnings; shared `NoData` содержал запрещённый ESLint `@ts-ignore`.

## Затронутые файлы и решения

| Область | Изменение |
|---|---|
| `HomeContext.tsx` | Сохранены три независимые state machine; fetch functions стабилизированы через `useCallback`, чтобы параллельные секции не зависели от global loading race |
| `NoData.tsx` | Добавлены optional `actionLabel`/`onAction`; существующие consumers без action не изменили render |
| Home schedule/news/video components | Error state показывает `Спробувати ще раз` и повторяет только свой request |
| Schedule/news navigation | Click-only presentation заменена семантическими React Router links в `/schedule` и `/news` |
| Promotion navigation | Добавлена отдельная top-right ссылка в `/video/section?videoType=<type>` без перекрытия native video controls |
| Runtime styles | `style.css` и непосредственно связанные SCSS partials синхронизированы; happy-path grid/frame baseline не переработан |

Backend source не менялся: `API-03` уже закрепила `200 application/json null` для отсутствующих Home schedule/video; это повторно проверено contract tests.

## Проверки и результаты

| Проверка | Команда или сценарий | Результат | Exit code | Что доказано |
|---|---|---|---:|---|
| Dependency/worktree gate | `04_BACKLOG.md`; `git status --short` | `QA-01=done`, `API-03=done`; существующие `DATA-04` changes отделены | 0 | Задача разблокирована; пользовательские изменения сохранены |
| Browser RED — retry | Controlled WebKit + error fixtures до implementation | `Expected 3 retry buttons, found 0` | 1 | Test обнаружил отсутствующий retry |
| Browser RED — navigation | Controlled WebKit + success fixtures до implementation | `Expected 3 Home section links, found 0` | 1 | Test обнаружил несемантические/отсутствующие переходы |
| Browser GREEN matrix | WebKit 26.5, Vite, controlled error → retry → success, mixed, empty, delayed loading fixtures | Matrix passed; три retries восстанавливают свои sections; mixed schedule failure не скрывает healthy news/video; empty/loading/error различимы | 0 | Все Home state paths и независимость параллельных requests наблюдаемы |
| Navigation GREEN | Browser links `Test news`, `Test schedule`, `Test promotion` | URLs: `/news`, `/schedule`, `/video/section?videoType=Performance` | 0 | Данные ведут в правильные разделы |
| Visual before/after | 1440×900 WebKit screenshots error baseline/after + happy path | Grid/frame сохранены; error states получили retry; promo link перенесён top-right и не перекрывает controls | 0 | UI проверен визуально, не только через DOM |
| Frontend targeted ESLint | ESLint по шести изменённым TS/TSX files | 0 errors, 0 warnings | 0 | FEAT-03 files не оставляют diagnostics |
| Frontend production build | `VITE_API_URL=https://api.example.invalid/api npm run build` | TypeScript + Vite; 535 modules; production bundle создан | 0 | Production frontend собирается |
| Bundle URL scan | `rg` по `dist` для legacy/local/test API URLs | `api.example.invalid` найден; `localhost:44315` и localhost mock URL отсутствуют | 0 | Bundle не содержит local API origin |
| Home/API contracts | `dotnet test ... --filter FullyQualifiedName~ApiContractIntegrationTests` с disposable localhost PostgreSQL | 26 passed, 0 failed/skipped | 0 | Nullable Home endpoints и общий API contract остаются green |
| Backend regression без opt-in DATA-04 integration | `dotnet test ... --filter FullyQualifiedName!~MediaPersistenceIntegrationTests -m:1` | 85 passed, 0 failed/skipped | 0 | Текущий backend regression set вне media opt-in green |
| Full frontend lint | `npm run lint` | 4 errors, 10 warnings; все вне FEAT-03 files | 1 | Baseline уменьшен с 5/13 до 4/10; остаток принадлежит `QA-03` |
| Diff/secret gates | `git diff --check`; added-line secret pattern scan; scoped diff review | Whitespace clean; secret matches отсутствуют; только FEAT-03 code/docs поверх сохранённого DATA-04 work | 0; scan 1 expected for no matches | Diff пригоден; secrets и scope drift отсутствуют |

## Диагностические запуски

- Первый frontend build после удаления `@ts-ignore` получил `TS2307` для `no-data.svg?react`; suppression возвращён как проверяемый `@ts-expect-error` с причиной. Финальный build green.
- Финальный повтор browser matrix дважды завершился `Abort trap: 6` при запуске WebKit внутри sandbox до открытия страницы; тот же неизменённый сценарий вне sandbox прошёл. Это ограничение запуска browser process, не сбой Home flow.
- Первый backend run без explicit env завершился startup failures по отсутствующему `DB_CLOUD`; повтор использовал только disposable localhost PostgreSQL и явные test env values.
- Первый parallel filtered backend run на пустой DB дал 84/85: два WebApplicationFactory одновременно создавали Hangfire schema и один получил `pg_namespace_nspname_index`. После controlled schema initialization тот же filtered suite дал 85/85. Source fix не требовался.

## Невыполненные проверки и проблемы вне scope

- Два `MediaPersistenceIntegrationTests` не перезапускались: они требуют `DATA04_INTEGRATION=1`, disposable Azurite и media lifecycle topology; `FEAT-03` media/backend persistence не меняла. Подтверждение `DATA-04` сохранено отдельно.
- Full ESLint остаётся red: 4 banned `@ts-ignore` errors и 10 hook warnings в Admin/Group/Music/News/Schedule/Footer/Frame/Header/Pagination/Video files. Их массовое исправление — `QA-03`, не `FEAT-03`.
- Backend restore/build сообщает существующие NU1902/NU1903 advisories для `SixLabors.ImageSharp` 3.1.6. Dependency update вне scope.
- Mobile/tablet matrix, real production content и production browsers не проверялись; они относятся к `MOB-*` и `M5/M6`.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Nullable schedule/video не вызывают падение | Выполнен | Empty browser fixtures + 26/26 API contracts |
| Loading, empty, error наблюдаемы | Выполнен | Delayed/empty/error browser matrix для всех трёх секций |
| Retry доступен и восстанавливает данные | Выполнен | Error → per-section retry → success browser flow |
| Параллельные requests независимы | Выполнен | Mixed schedule error при healthy news/video; один retry control |
| Данные ведут в правильные разделы | Выполнен | Семантические links и browser URL assertions |
| Desktop visual baseline не переработан | Выполнен | Before/after и happy-path screenshots |

Все критерии `FEAT-03` выполнены. Статус: `done`. Следующая незаблокированная задача backlog — `FEAT-04`; она не начиналась.
