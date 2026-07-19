# FEAT-08 — Video, promotion и категории

## Цель и границы

Завершить desktop Video vertical: три существующие категории, stable pagination, playback, admin create/edit/delete с public readback, безопасная замена promotion video и локальные реальные category media вместо Bing placeholders.

Вне объёма: новые категории, mobile/tablet adaptation, production DB/Blob/ACL, импорт production content, CDN/storage redesign, FEAT-09 и последующие задачи. Коммит и branch change не выполнялись.

## Исходное состояние

- `FEAT-08` существовала со статусом `planned`; зависимости `FEAT-01`, `API-03`, `DATA-04` имели `done`.
- Video pagination не проверяла `pageSize`, обнуляла реальные totals для out-of-range page и не имела стабильного DB ordering.
- Video add/edit не имели FluentValidation validators; type можно было менять в/из `Promotion` через прямой PATCH.
- DATA-04 уже реализовала безопасный promotion replacement: новый Blob и atomic DB replace до удаления старого Blob; fault tests существовали.
- Video UI смешивала request error с empty, зависела от stale `sessionStorage`, выполняла navigation во время render и не давала retry.
- Home promotion link формировал запрещённый `videoType=Promotion` и попадал на 404.
- Video list давал React key warning; MP4 picker принимал любой `video/*`.
- `Page.tsx` передавал один Bing URL трём category tiles.
- `OPEN-008` находится в разделе «Решённые пункты», но вместо решения содержит вопрос, owner/deadline/fallback и не называет три утверждённых изображения. DATA-02 corpus содержит один generic JPEG, поэтому approved mapping отсутствует.

## Затронутые файлы

Backend:

- `StoronnimV.Application/Services/Entities/VideoService.cs`.
- `StoronnimV.Infrastructure/Repositories/Database/VideoRepository.cs`.
- validators в `StoronnimV.Application/Validation/Video/`.
- `StoronnimV.Tests/Application/VideoServiceMediaTests.cs`.
- `StoronnimV.Tests/Application/VideoServicePaginationTests.cs`.
- `StoronnimV.Tests/Api/ApiContractIntegrationTests.cs`.
- `StoronnimV.Tests/Api/VideoCrudIntegrationTests.cs`.

Frontend:

- `src/assets/video-categories/performance.webp`, `backstage.webp`, `repetition.webp`.
- `src/models/video/IVideoModel.ts`.
- `src/components/contexts/VideoContext.tsx`.
- `src/components/elements/home/PromotionVideoHome.tsx`.
- `src/components/elements/video/VideoList.tsx`, `VideoSections.tsx`.
- Video add/edit/delete forms.
- `src/components/pages/shared/Page.tsx`.

Evidence:

- этот документ;
- `output/playwright/feat08-before-bing.png`;
- `output/playwright/feat08-video-playback.png`;
- `output/playwright/feat08-after-category-images.png`.

Существующие незакоммиченные изменения API-04/FEAT-06/FEAT-07 и пользователя сохранены. FEAT-08 не откатывала и не перезаписывала их.

## Решения и изменения

- Public category contract остаётся ровно `Performance`, `Backstage`, `Repetition`; `Promotion` — специальный Home media type, не четвёртая category page.
- Pagination требует positive `page`/`pageSize`, invalid type даёт unified `400`, empty/out-of-range сохраняет фактические totals, rows сортируются по `CreatedAt` и `Id` до `Skip`.
- Add/edit требуют non-empty title и известный `VideoType`. Edit не может переводить обычный video в `Promotion` или promotion в категорию; replacement остаётся только через безопасный create path DATA-04.
- Добавлен opt-in real API/PostgreSQL/Azurite lifecycle test: три категории, pagination matrix, CRUD/readback, public Blob URL, failed/successful promotion replacement и Blob cleanup.
- Frontend использует общий typed category set, explicit `idle/loading/success/empty/error`, retry и React state вместо `sessionStorage` pagination.
- Invalid category рендерит redirect без list request. Home promotion ведёт на `/video/sections`.
- Category tile images имеют содержательные Ukrainian `alt`, соответствующие `Performance`, `Backstage`, `Repetition`.
- Video forms показывают non-success, блокируют повторную mutation во время loading, trim-ят title и ограничивают picker MP4.
- React keys добавлены только Video rows/preloaders; shared list не менялся.
- 17 июля 2026 года владелец явно разрешил generation и выбрал photorealistic вариант A. Built-in OpenAI image generation создала три original landscape 3:2 scenes в единой black/gold системе: live performance, backstage preparation, rehearsal studio; без текста, брендов, watermark и узнаваемых real persons.
- Финальные 1536×1024 WebP: `performance.webp` 89 KiB, `backstage.webp` 114 KiB, `repetition.webp` 135 KiB. `Page.tsx` импортирует их локально; Bing URLs удалены.

## TDD и проверки

| Команда или сценарий | Результат | Exit code | Что доказано |
|---|---|---:|---|
| `VideoServicePaginationTests` RED | 3/4 failed: `pageSize=0` и unknown category не rejected, out-of-range totals `0` | 1 | Tests фиксируют реальные pagination gaps |
| Video add/edit contract RED на disposable localhost DB | invalid type/title дали `201/204` вместо `400` | 1 | Validators отсутствовали |
| Promotion type transition RED | 2/2 expected exceptions отсутствовали | 1 | Direct PATCH обходил replacement invariant |
| `VideoServicePaginationTests` + `VideoServiceMediaTests` GREEN | 11/11 passed | 0 | Pagination, fault order и promotion transition invariant |
| Targeted Video contract GREEN | 4/4 passed | 0 | Invalid title/type дают unified `400` |
| `FEAT08_INTEGRATION=1 ... VideoCrudIntegrationTests` | 1/1 passed | 0 | Real API/PostgreSQL/Azurite categories, pagination, CRUD, public media, failed/successful promotion replacement, DB/Blob readback |
| All seven integration flags + full solution tests | 118/118 passed, 0 skipped | 0 | Full backend regression suite на task-owned localhost PostgreSQL/Azurite |
| `dotnet restore ... --no-cache --disable-build-servers` | 5 projects restored; existing ImageSharp advisories | 0 | Fresh dependency restore |
| `dotnet build ...sln --no-restore --configuration Release` | 0 errors, 2 existing advisory warnings | 0 | Release solution compilation |
| Targeted ESLint всех изменённых FEAT-08 TS/TSX files | No findings | 0 | Changed frontend code lint-clean |
| `VITE_API_URL=https://api.example.test/api npm run build` | 537 modules; bundle built | 0 | TypeScript/Vite production build |
| Повторный ESLint `VideoSections.tsx` + production build после `alt` correction | No findings; 537 modules | 0 | Содержательные category image labels compile и lint-clean |
| `npm run lint` | 4 errors, 3 warnings вне FEAT-08 | 1 | Existing QA-03 baseline remains; FEAT-08 Video warning removed |
| Browser RED, controlled WebKit API `500` | Error показан как empty; retry отсутствовал | 1 | Error/empty gap воспроизведён |
| Browser RED, Home promotion | href был `/video/section?videoType=Promotion` | 1 | Promotion navigation bug воспроизведён |
| Browser RED, categories | 3 Bing image sources; before screenshot | 1 | Placeholder criterion не выполнен |
| Controlled WebKit error/navigation GREEN | retry visible; promotion href `/video/sections`; invalid category issued 0 list requests | 0 | Исправлены error/retry и routing |
| Controlled WebKit categories/pagination/playback | 3/3 categories loaded и проиграли real one-second MP4; page 2 loaded | 0 | Public category, playback и pagination browser flow |
| Controlled WebKit Basic Admin forms | CSRF token + MP4 create/edit/delete дали `201/204/204` | 0 | Frontend form submission, loading/status handling и mutation requests |
| Browser console повтор | React key warning отсутствует; остаются ожидаемые local manifest DNS и anonymous admin `401` | 0 | FEAT-08 console warning устранён |
| Repository и full Git history media audit | Category assets/mapping отсутствуют; история `Page.tsx` содержит только тот же Bing URL; `OPEN-008` с первого commit был вопросом | 0 | Решение владельца нельзя восстановить из repository history |
| Built-in image generation + visual inspection | 3 distinct photorealistic 1536×1024 PNG; scene/category mapping, crop safety, black/gold palette, no visible text/brands/watermark | 0 | Owner-approved original source media созданы без third-party image dependency |
| `cwebp -q 85 -m 6` + `sips` inspection | 3 WebP, 1536×1024; 89/114/135 KiB | 0 | Project-bound assets имеют web-ready format/weight и точное 3:2 ratio |
| Targeted `Page.tsx`/`VideoSections.tsx` ESLint + production build после assets | No findings; 540 modules; все 3 WebP emitted | 0 | Local imports type-check, lint и bundle корректны |
| Controlled WebKit category media after | 3/3 local WebP natural size 1536×1024, `object-fit: cover`, semantic `alt`, 0 Bing requests; after screenshot | 0 | Placeholder criterion и визуальный desktop gate выполнены |
| Fresh empty-DB all-flags diagnostic | 15/118 failed: parallel test hosts raced while creating absent EF/Hangfire schema | 1 | Подтверждена обязательная precondition explicit migrations; FEAT-08 assertions не были причиной |
| Canonical Infrastructure-only `dotnet ef database update` | 25 migrations applied to disposable localhost PostgreSQL | 0 | Test schema подготовлена утверждённым workflow без startup migration |
| Fresh all seven integration flags + full solution tests после migration | 118/118 passed, 0 skipped | 0 | Итоговый backend regression/API/PostgreSQL/Azurite gate green |
| `git diff --check` | No diagnostics | 0 | Whitespace valid |

Первый fresh restore и первый explicit build внутри sandbox зависли без output из-за local IPC restriction и были остановлены по exact PID. Повторы вне sandbox завершились exit 0. Source correction не требовалась.

`playwright-cli` не стартовал из-за отсутствующего system Chrome. Проверки выполнены bundled Playwright WebKit из workspace runtime; browser automation и screenshots завершились успешно.

## Невыполненные проверки и ограничения

- Browser form flow выполнен на controlled admin mock, а auth/mutation/DB/Blob readback — отдельно на real API integration с Basic bearer. Повтор полного cookie login flow не выполнялся: он уже принят в FEAT-01/API-02 и не менялся FEAT-08.
- Production/staging DB/Blob/ACL не проверялись: запрещено scope и отложено до M5/M6.
- Full frontend lint не green из-за 4 errors/3 warnings в неизменённых `AdminContainer`, `GroupDescription`, `MemberModal`, `FrameLayout`, `Header`, `PaginationSection`; это `QA-03`, не FEAT-08.

## Проблемы вне scope

- Existing `SixLabors.ImageSharp 3.1.6` advisories `NU1902`/`NU1903`; dependency update запрещена без отдельной задачи.
- External local manifest DNS и anonymous admin `401` относятся к controlled local environment, не Video vertical.
- Full ESLint baseline относится к `QA-03`; не исправлялся.
- `FEAT-08` source всё ещё ссылается на superseded `DEC-006`; для M1–M4 применён accepted `DEC-017` и только localhost test corpus.

## Итог критериев приёмки

| Критерий | Итог |
|---|---|
| Три категории работают | Выполнен: real API integration + controlled WebKit playback |
| Category pagination | Выполнен: valid/empty/out-of-range/invalid + stable ordering + browser page 2 |
| Video CRUD/readback/media | Выполнен в real API/PostgreSQL/Azurite; frontend формы build/lint green |
| Promotion не теряется при failed replace | Выполнен: service fault tests и real API/DB/Blob invalid replacement |
| Нет Bing placeholder; реальные согласованные category media | Выполнен: owner-approved generated 3:2 WebP, local imports, 0 source/browser Bing matches |
| Integration/E2E/media failure checks | Выполнены для API/CRUD/categories/playback/promotion failure и imagery after |

Все критерии выполнены. `FEAT-08` имеет статус `done`; backlog/state/index синхронизированы. Следующая задача — `FEAT-09`, но она не начиналась.
