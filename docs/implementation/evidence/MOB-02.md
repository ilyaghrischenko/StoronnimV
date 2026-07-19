# MOB-02 — Responsive Home, News и Schedule

**Дата:** 19 июля 2026 года  
**Scope:** Home grid/Swiper/states, News list/detail, Schedule list/detail/map,
opt-in mobile pagination, touch/keyboard/admin triggers и runtime CSS. Backend,
API/model contracts, dependencies, CRUD mutations, outer modal behavior и
`MOB-03` не менялись.

## Preflight и dependency gate

- `MOB-02` существовала со статусом `planned`; `MOB-01` имела статус `done`;
  активен `M3`.
- Стартовая ветка: `main`; HEAD:
  `ea77b34fc9d4c95a01ab7320260cf26c50c27769`.
- `git diff --check`: exit `0` до и после реализации.
- Исходный `npm run lint`: exit `1`, 4 errors/2 warnings. Diagnostics:
  `GroupDescription.tsx`, `MemberModal.tsx`, `FrameLayout.tsx`, `Header.tsx`,
  `PaginationSection.tsx`.
- Стартовый worktree был dirty. Полный стартовый `git status --short`:

```text
 M backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs
 M backend/StoronnimV.Server/StoronnimV.Api/Middlewares/ExceptionMiddleware.cs
 M backend/StoronnimV.Server/StoronnimV.Api/Program.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Contracts/Controllers/IAdminControllerService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Contracts/Entities/IGroupSocialService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Controllers/AdminControllerService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/GroupPageService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/GroupSocialService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/ScheduleService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/SuperAdminService.cs
 M backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/VideoService.cs
 M backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs
 M backend/StoronnimV.Server/StoronnimV.Infrastructure/Repositories/Database/VideoRepository.cs
 M backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimVContext.cs
 M backend/StoronnimV.Server/StoronnimV.Tests/Api/ApiContractIntegrationTests.cs
 M backend/StoronnimV.Server/StoronnimV.Tests/Application/VideoServiceMediaTests.cs
 M docs/implementation/00_INDEX.md
 M docs/implementation/04_BACKLOG.md
 M docs/implementation/08_OPEN_ITEMS.md
 M docs/implementation/09_STATE.md
 M docs/implementation/10_RUNTIME_CONTRACT.md
 M docs/implementation/11_MIGRATION_WORKFLOW.md
 M frontend/storonnimv.client/README.md
 M frontend/storonnimv.client/package-lock.json
 M frontend/storonnimv.client/package.json
 M frontend/storonnimv.client/src/components/contexts/AdminContext.tsx
 M frontend/storonnimv.client/src/components/contexts/MusicContext.tsx
 M frontend/storonnimv.client/src/components/contexts/VideoContext.tsx
 M frontend/storonnimv.client/src/components/elements/admin/AdminContainer.tsx
 M frontend/storonnimv.client/src/components/elements/admin/BasicAdmins.tsx
 M frontend/storonnimv.client/src/components/elements/admin/SuperAdminButtons/AddAdminModal.tsx
 M frontend/storonnimv.client/src/components/elements/admin/SuperAdminButtons/DeleteAdminModal.tsx
 M frontend/storonnimv.client/src/components/elements/admin/SuperAdminButtons/EditAdminModal.tsx
 M frontend/storonnimv.client/src/components/elements/group/forms/groupSocial/AddGroupSocialModal.tsx
 M frontend/storonnimv.client/src/components/elements/group/forms/groupSocial/EditGroupSocialModal.tsx
 M frontend/storonnimv.client/src/components/elements/group/forms/member/DeleteMemberModal.tsx
 M frontend/storonnimv.client/src/components/elements/home/PromotionVideoHome.tsx
 M frontend/storonnimv.client/src/components/elements/music/MusicPlatformItem.tsx
 M frontend/storonnimv.client/src/components/elements/music/MusicPlatforms.tsx
 M frontend/storonnimv.client/src/components/elements/music/forms/AddMusicPlatformModal.tsx
 M frontend/storonnimv.client/src/components/elements/music/forms/EditMusicPlatformModal.tsx
 M frontend/storonnimv.client/src/components/elements/shared/Footer.tsx
 M frontend/storonnimv.client/src/components/elements/video/VideoList.tsx
 M frontend/storonnimv.client/src/components/elements/video/VideoSections.tsx
 M frontend/storonnimv.client/src/components/elements/video/forms/AddVideoModal.tsx
 M frontend/storonnimv.client/src/components/elements/video/forms/DeleteVideoModal.tsx
 M frontend/storonnimv.client/src/components/elements/video/forms/EditVideoModal.tsx
 M frontend/storonnimv.client/src/components/pages/Developers.tsx
 M frontend/storonnimv.client/src/components/pages/shared/Page.tsx
 M frontend/storonnimv.client/src/models/video/IVideoModel.ts
 M frontend/storonnimv.client/src/styles/elements/shared/Footer.scss
 M frontend/storonnimv.client/src/styles/elements/shared/FrameLayout.scss
 M frontend/storonnimv.client/src/styles/elements/shared/Header.scss
 M frontend/storonnimv.client/src/styles/elements/shared/HeaderWithFooter.scss
 M frontend/storonnimv.client/src/styles/shared/App.scss
 M frontend/storonnimv.client/src/styles/style.css
 M frontend/storonnimv.client/src/styles/style.css.map
 M frontend/storonnimv.client/src/styles/style.scss
?? backend/StoronnimV.Server/StoronnimV.Application/Validation/ExternalHttpUrlRuleExtensions.cs
?? backend/StoronnimV.Server/StoronnimV.Application/Validation/GroupSocials/
?? backend/StoronnimV.Server/StoronnimV.Application/Validation/Music/
?? backend/StoronnimV.Server/StoronnimV.Application/Validation/Video/
?? backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/20260715012000_EnforceGroupPageSingleton.cs
?? backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/20260717233000_EnforceAdminLoginUniqueness.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Api/BasicAdminCrudIntegrationTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Api/GroupCrudIntegrationTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Api/HangfireDashboardIntegrationTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Api/MusicAndGroupSocialCrudIntegrationTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Api/VideoCrudIntegrationTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Application/ScheduleStatusUpdaterTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Application/SuperAdminServiceTests.cs
?? backend/StoronnimV.Server/StoronnimV.Tests/Application/VideoServicePaginationTests.cs
?? docs/implementation/FEAT-10/
?? docs/implementation/MOB-01/
?? docs/implementation/MOB-02/
?? docs/implementation/evidence/API-04.md
?? docs/implementation/evidence/FEAT-06.md
?? docs/implementation/evidence/FEAT-07.md
?? docs/implementation/evidence/FEAT-08.md
?? docs/implementation/evidence/FEAT-09.md
?? docs/implementation/evidence/FEAT-10.md
?? docs/implementation/evidence/MOB-01.md
?? frontend/storonnimv.client/src/assets/video-categories/
?? frontend/storonnimv.client/src/styles/shared/breakpoints.scss
?? frontend/storonnimv.client/src/utils/
?? output/
?? prompt.txt
```

Весь стартовый diff считался пользовательским и сохранён. В частности,
`PromotionVideoHome.tsx` с переходом в `/video/sections` не менялся MOB-02.
Смена ветки, commit, stash/reset/clean/merge/rebase не выполнялись.

## Реализация

- Home: `<=1024px` — responsive 2-column flow; `<=768px` — одна колонка
  `Schedule / News / Video`; fixed clipping удалён. Swiper использует 1 slide,
  2 от `640px`, 3 от `1024px`, сохраняя Navigation, Autoplay, speed, swipe и
  `loop={homeNewsList.length > 3}`. Синтаксис подтверждён официальной
  документацией Swiper `v11.2.10` через Context7.
- Compact Home cards постоянно показывают title с contrast overlay; links,
  arrows, promo/retry controls имеют visible focus и минимум `44px`.
- News/Schedule route containers получили page-scoped document flow. Grid:
  `repeat(auto-fit, minmax(min(100%, 16.25rem), 1fr))` на compact и
  `1/1/2/3` columns на `320/375/768/1024`.
- News success card стала native `button`; loading skeleton остался
  non-interactive. News/Schedule compact cards показывают полный static
  title/date/location/status с `overflow-wrap:anywhere`.
- News/Schedule detail media/text и Schedule map ограничены container.
  Schedule detail складывается в одну колонку на mobile.
- Pagination получила `nav`, labels, `aria-current`, non-interactive ellipsis
  и opt-in compact mode `previous / current-total / next`; Video caller не
  передаёт opt-in prop.
- Add/edit/delete triggers на mobile перешли в normal flow, имеют финальный
  размер `46×44px` и открывают существующие forms/confirm flows. Mutations не
  отправлялись.

MOB-02 source files: 7 TSX, `Home.scss`, 5 Home partials, 3 News partials,
4 Schedule partials и `PaginationSection.scss`. Runtime artifacts
`style.css`/`style.css.map` сгенерированы только из canonical SCSS. Backend,
models, contexts, dependencies, production config и legacy partial CSS/maps
не менялись MOB-02.

## Browser RED/GREEN

Disposable mock API: `127.0.0.1:41802`; Vite: `127.0.0.1:41803`.
Fixtures: success, delayed loading, empty, controlled error, mixed Home,
long content, admin и Home news counts `1/2/3/6`. Production API/DB/Blob не
использовались. Screenshots находятся вне Git:

- before: `/tmp/storonnimv-mob02/before/{home,news,schedule}-{viewport}.png`;
- after: `/tmp/storonnimv-mob02/after/{home,news,schedule}-{viewport}.png`;
- landscape и long detail: `/tmp/storonnimv-mob02/after/*812x375.png`,
  `news-long-detail-320x800.png`, `schedule-long-detail-320x800.png`.

Chromium: `HeadlessChrome/151.0.7922.10`; WebKit UA: Safari `26.5`;
Firefox `152.0`.

Before: Home имела 2 columns на 320/375/768, а Home titles имели opacity `0`;
News и Schedule имели 3 сжатые columns на 320/375; pagination на 320 имела
left `-13.70px`, right `333.72px`.

| Route | 320 | 375 | 768 | 1024 | 1440 |
|---|---:|---:|---:|---:|---:|
| Home columns | 1 | 1 | 1 | 2 | desktop 2-area baseline |
| News columns | 1 | 1 | 2 | 3 | 3 |
| Schedule columns | 1 | 1 | 2 | 3 | 3 |
| client/scroll width | 320/320 | 375/375 | 768/768 | 1024/1024 | 1440/1440 |

After visual inspection: mobile cards не сжаты, all compact text видим без
hover, pagination помещается, vertical scroll достигает последней card/control.
1440 frame, 3-column content, hover behavior и visual hierarchy сохранены.
Swiper runtime показал `1/1/2/3` slides для `320/375/768/1024`.

Дополнительные browser facts:

- Home delayed: 3 preloaders, затем 3 success sections; empty: 3 blocks;
  error: 3 retry controls; mixed: 1 error плюс working Schedule/Video.
- News loading/success: `6/6`; Schedule: `3/3`; empty/error/retry/recovered
  paths прошли. Retry touch targets: `172.42×44px`.
- Home fixtures `1/2/3/6`: slide count совпал; loop был
  `false/false/false/true`; console loop warnings — 0.
- Swiper arrow изменил real index `0→1`; real Chromium touch gesture в
  `hasTouch` context изменила real index `0→2`.
- Home links открыли `/schedule`, `/news`, `/video/sections`.
- News native button открыл detail через keyboard Enter; Schedule native
  button — через Space. Focus outline был visible.
- Compact pagination click изменил `1 / 8` на `2 / 8`; previous стал enabled.
- Long News/Schedule list и detail сохранили `320/320` client/scroll width.
  Modal content имел bounds `10..310px`; photo/text/video/map — `32..288px`.
- Map сохранила точный percent-encoded Google Maps `src`, lazy iframe title
  `Карта: {address}` и controlled `256×180px` bounds. External map response был
  перехвачен controlled HTML; production resource не требовался.
- Admin add/edit/delete controls имели final `46×44px`, `position:static` и
  открыли News/Schedule add/edit/delete content. Mutations не отправлялись.
- Landscape `812×375`: Home/News/Schedule client/scroll `812/812`.
- WebKit и Firefox на 375/1024 повторили expected columns и exact
  client/scroll equality. Happy-path Chromium/WebKit/Firefox console:
  0 errors, 0 warnings. Controlled error fixtures дали только ожидаемые
  request/application diagnostics.

## Static/build checks

| Команда | Итог |
|---|---|
| targeted ESLint из плана | exit `0`, 0 findings |
| `npm run styles:build` | exit `0` |
| `npm exec sass -- --version` | exit `0`, `1.79.6` |
| повторная CSS generation | exit `0`, hashes совпали |
| `style.css` SHA-256 | `256e635386785e38ed5e05c230ee5a13a9781717c605072710220de5ad6446ec` |
| `style.css.map` SHA-256 | `ba4b0941056b3c5374003f970be3802ce49a0a76da26f534eecc859c9f4eef27` |
| `VITE_API_URL=https://api.example.test/api npm run build` | exit `0`, 540 modules |
| bundle scan `localhost:44315\|127\.0\.0\.1\|storonnimv-mob02` | exit `1`, совпадений нет |
| финальный full ESLint | exit `1`, 2 errors/2 warnings |
| `git diff --check` | exit `0` |
| secret scan task source files | exit `1`, совпадений нет |

Full ESLint улучшен относительно исходного baseline `4 errors/2 warnings`:
два `@ts-ignore` в затронутом `PaginationSection.tsx` заменены корректными
`@ts-expect-error`. Остались 2 errors в `FrameLayout.tsx`/`Header.tsx` и 2
warnings в Group files; все вне MOB-02 и остаются `QA-03`.

Backend tests не запускались: backend/API/contracts не менялись. Commit,
branch switch и `MOB-03` implementation не выполнялись.

## Итог

- **pass** — dependency/scope/worktree hygiene; пользовательский diff сохранён.
- **pass** — Home 320–1440, Swiper breakpoints/swipe/navigation/states/links.
- **pass** — News responsive cards, native semantics, detail, pagination и admin triggers.
- **pass** — Schedule responsive cards, detail/map и admin triggers.
- **pass** — mandatory viewport/landscape overflow, touch/keyboard/long content.
- **pass** — Chromium/WebKit/Firefox smoke и happy-path console/network.
- **pass** — deterministic runtime CSS, targeted lint, production build,
  bundle/diff/secret checks.

Обязательных невыполненных критериев MOB-02 нет. Outer modal mechanics,
form layout, full accessibility/cross-browser release audit и remaining ESLint
baseline остаются `MOB-04`/`MOB-05`/`MOB-06`/`QA-03`.
