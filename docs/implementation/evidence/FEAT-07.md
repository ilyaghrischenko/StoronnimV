# FEAT-07 — Music и group socials CRUD

## Цель и границы

Завершить desktop vertical Music/Footer: create/edit/delete music platforms и group socials, create/replace/delete photos, public readback, безопасное открытие внешних ссылок и отклонение malformed/unsafe URL.

Вне объёма остались analytics, смена Spotify/embed provider, mobile adaptation, production DB/Blob и следующая задача `FEAT-08`.

## Исходное состояние

- Music и Footer читали public endpoints; create/edit/delete forms существовали.
- Backend не проверял schemes/форму external URL: bare domain, `javascript:` и `ftp:` проходили add/edit contracts.
- `MusicPlatformItem` принимал любой URI scheme; Footer напрямую передавал stored `linkUrl` в `href`.
- Group social photo можно было создать и удалить вместе с записью, но отдельного replace endpoint/UI не было.
- Music/Footer fetch effects имели hook dependency warnings; Music preloader list имел runtime key warning.
- До production-кода contract RED дал: invalid create `201`, invalid edit `204`, group-social photo patch `405`. Browser RED показал два `javascript:` href.

## Затронутые файлы

Backend:

- `StoronnimV.Api/Controllers/AdminController.cs`.
- `StoronnimV.Application/Contracts/Controllers/IAdminControllerService.cs`.
- `StoronnimV.Application/Contracts/Entities/IGroupSocialService.cs`.
- `StoronnimV.Application/Services/Controllers/AdminControllerService.cs`.
- `StoronnimV.Application/Services/Entities/GroupSocialService.cs`.
- `StoronnimV.Application/Validation/ExternalHttpUrlRuleExtensions.cs`.
- validators в `StoronnimV.Application/Validation/Music/` и `Validation/GroupSocials/`.
- `StoronnimV.Tests/Api/ApiContractIntegrationTests.cs`.
- `StoronnimV.Tests/Api/MusicAndGroupSocialCrudIntegrationTests.cs`.

Frontend:

- `src/utils/externalUrl.ts`.
- `MusicContext.tsx`, `MusicPlatforms.tsx`, `MusicPlatformItem.tsx`.
- Music add/edit forms.
- `Footer.tsx`; group-social add/edit forms.

Evidence/state:

- `docs/implementation/evidence/FEAT-07.md`.
- `docs/implementation/00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`.
- `output/playwright/feat07-before-unsafe-links.png`, `feat07-after-safe-links.png`.

## Решения и изменения

- Accepted URL contract: absolute `http://` или `https://`, non-empty host, no surrounding whitespace, no embedded credentials. Bare domains и другие schemes отклоняются.
- FluentValidation применяется к Music/GroupSocial add/edit DTO; explicit `null` возвращает unified `400`, не `500`.
- Frontend повторяет protocol/credentials guard для legacy public rows. Unsafe stored links остаются видимыми, но не получают `href` и помечаются `aria-disabled`.
- Добавлен `PATCH /api/admin/group-socials/photo`; replacement использует подтверждённый DATA-04 `MediaStorageService.ReplaceAsync`: old Blob удаляется только после успешной DB mutation.
- Group-social edit form теперь заменяет photo; file pickers ограничены JPEG/PNG/WebP; URL inputs используют `type=url` и `https?` pattern.
- Music/Footer fetch callbacks стабилизированы; статические preloader rows получили keys. Full lint baseline уменьшился с 4 errors/6 warnings до 4 errors/4 warnings.

## Проверки

| Команда или сценарий | Результат | Exit code | Что доказано |
|---|---|---:|---|
| Targeted contract RED | 4 URL cases: expected `400`, actual `201/204`; photo route expected `204`, actual `405` | 1 | Tests действительно фиксировали отсутствующее поведение |
| Explicit-null contract RED | Music и group-social edit вернули `500` вместо `400` | 1 | Обнаружен null dereference в URL predicate до финальной проверки |
| Browser RED, controlled WebKit `/music` | Music/Footer snapshot содержал `javascript:alert(1/2)` href | 0 | Реальный client рендерил unsafe legacy links |
| Targeted contract GREEN | 7/7: malformed/unsafe/null URL и group-social photo multipart | 0 | Add/edit validation, unified `400`, photo route contract |
| `FEAT07_INTEGRATION=1 ... dotnet test ... --filter FullyQualifiedName~MusicAndGroupSocialCrudIntegrationTests` | 1/1 | 0 | Real API/PostgreSQL/Azurite create/edit/delete, valid/invalid URL, public readback, photo replace/delete, DB/Blob state |
| `dotnet restore ... --no-cache --artifacts-path /tmp/storonnimv-feat07-final/artifacts` | Restore complete; 2 existing ImageSharp advisories | 0 | Clean dependency restore |
| `dotnet build ...StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path ...` | 0 errors, 4 existing warnings | 0 | Fresh full backend compilation |
| `dotnet build ...StoronnimV.Api.csproj --no-restore --configuration Release --artifacts-path ...` | 0 errors, 2 existing advisories | 0 | Startup API compiles |
| All integration flags + `dotnet test ...StoronnimV.Server.sln --no-restore --no-build ...` | 107/107, 0 skipped | 0 | Full backend regression suite, including all real integration gates |
| `VITE_API_URL=https://api.example.test/api npm run build` | 536 modules; production bundle built | 0 | TypeScript/Vite production build |
| Targeted ESLint over every changed FEAT-07 TS/TSX file | No findings | 0 | Changed frontend code has no lint errors/warnings |
| `npm run lint` | 4 errors, 4 warnings, all outside FEAT-07 files | 1 | Known QA-03 baseline remains; FEAT-07 introduced no findings and removed two warnings |
| Controlled WebKit unsafe-link GREEN | `unsafe=0`, `aria-disabled=2` | 0 | Unsafe legacy Music/Footer links cannot navigate |
| Controlled WebKit valid-link E2E | Music opened `/destination/music`; Footer opened `/destination/social` in separate tabs | 0 | Valid external links remain functional with `target=_blank` |
| Before/after screenshots + visual inspection | Desktop layout/Spotify/frame unchanged | 0 | No visual baseline drift; only link safety changed |
| `git diff --check` | No whitespace errors | 0 | Patch formatting valid |

Первый full-suite запуск без старых integration flags дал 98 passed и 7 dynamic-skip failures; source failure отсутствовал. На заново созданной БД immediate all-flags run дал 102/107 из-за concurrent Hangfire schema bootstrap (`23505`/`40001`). Один serial FEAT-07 host test инициализировал existing Hangfire schema; канонический повтор без source changes дал 107/107.

## Невыполненные проверки

- Production/staging external URLs и production DB/Blob не проверялись: запрещено scope; требуется M5/M6 authorization.
- Full frontend lint не green из-за 4 errors/4 warnings в неизменённых `AdminContainer`, `GroupDescription`, `MemberModal`, `FrameLayout`, `Header`, `PaginationSection`, `VideoList`; это существующий `QA-03` backlog, не блокирует FEAT-07 targeted gate.
- Browser console сохранил ожидаемый anonymous `401 /api/admin/isAdmin` и недоступный external `https://storonnimv.com/site.webmanifest` в local controlled environment; FEAT-07 link flow не блокирован.

## Проблемы вне scope

- Existing `SixLabors.ImageSharp 3.1.6` advisories `NU1902`/`NU1903`; dependency update запрещена без отдельной доказанной задачи. FEAT-07 не блокирует.
- Оставшийся full ESLint baseline относится к `QA-03`; не исправлялся.
- External manifest availability относится к deployment/hosting, не FEAT-07.
- Existing FEAT-06/API-04 working-tree changes сохранены без отката или перезаписи.

## Итог критериев приёмки

| Критерий | Итог |
|---|---|
| Music platform create/edit/delete/photo | Выполнен: real API/DB/Blob integration + public readback |
| Group-social create/edit/delete/photo; Footer updates | Выполнен: real API/DB/Blob integration + public list readback; photo replace endpoint/UI добавлен |
| Валидные ссылки открываются | Выполнен: headed WebKit открыл Music и Footer destinations в новых tabs |
| Invalid input отклоняется | Выполнен: malformed, `javascript:`, `ftp:`, bare и null URL дают `400`; changed forms ограничены |
| Media consistency | Выполнен: create/replace/delete Blob assertions green через DATA-04 pattern |
| Builds/tests/targeted lint | Выполнен; full ESLint baseline явно зарегистрирован вне scope |

`FEAT-07` принята как `done`. Следующая backlog task — `FEAT-08`; она не начиналась. Коммит не создан.
