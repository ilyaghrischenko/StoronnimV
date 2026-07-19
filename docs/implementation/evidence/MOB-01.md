# MOB-01 — Responsive foundation и единый SCSS workflow

**Дата:** 19 июля 2026 года  
**Scope:** shared `html/body/root/app/frame`, Header/drawer, Footer, breakpoints
`768px`/`1024px`, SCSS generation и browser evidence. Page-specific mobile,
backend/API/DB/Blob и production deployment не менялись.

## Preflight и dependency gate

- `MOB-01` существовала со статусом `planned`.
- `FEAT-03`–`FEAT-10` и `API-04`: `done`.
- Стартовая ветка: `main`; HEAD:
  `ea77b34fc9d4c95a01ab7320260cf26c50c27769`.
- Worktree до MOB-01 был dirty: пользовательские backend/frontend/docs изменения,
  включая `Footer.tsx`, `Page.tsx`, backlog/state/index. Raw diff сюда не
  копировался; существующие изменения сохранены.
- `git diff --check`: exit `0`.
- Исходный `npm run build`: exit `0`.
- Исходный `npm run lint`: exit `1`, 4 errors и 2 warnings. Все diagnostics:
  `GroupDescription.tsx`, `MemberModal.tsx`, `FrameLayout.tsx`, `Header.tsx`,
  `PaginationSection.tsx`; это существующий QA-03 baseline.

## Реализация

- Canonical sources: `src/styles/style.scss` и импортируемые SCSS partials.
- Runtime artifacts: tracked `style.css` и `style.css.map`, только из
  `npm run styles:build`.
- Exact dev dependency: `sass@1.79.6`. Pin сохраняет legacy `@import` без
  diagnostics Sass 1.80; полный `@use` migration вне scope.
- `predev` и `prebuild` запускают generation; README документирует workflow.
- Добавлены `$layout-mobile-max: 768px` и `$layout-compact-max: 1024px`.
- `>1024px` сохраняет SVG frame и grid `86% / 14%`.
- `<=1024px` использует document flow Header/content/Footer без SVG и без
  global overflow masking. Existing wrappers flatten через `display: contents`;
  TSX не менялся.
- `<=768px` уменьшает logo/padding/drawer; burger/close остаются `44×44px`.

## Изменённые MOB-01 файлы

- `frontend/storonnimv.client/package.json`, `package-lock.json`, `README.md`;
- `src/styles/style.scss`, `style.css`, `style.css.map`;
- `src/styles/shared/breakpoints.scss`, `shared/App.scss`;
- `src/styles/elements/shared/FrameLayout.scss`, `HeaderWithFooter.scss`,
  `Header.scss`, `Footer.scss`;
- этот evidence, `00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`.

`App.tsx`, `FrameLayout.tsx`, `Header.tsx`, `Footer.tsx` и `Page.tsx` не
менялись MOB-01.

## Browser RED/GREEN

Disposable mock: `127.0.0.1:41731`; Vite: `127.0.0.1:41732`; route:
`/developers`. Mock отдавал три group-social fixtures и SVG images; остальные
API routes возвращали intentional `404`. Browser:
`HeadlessChrome/151.0.7922.10` на macOS.

Before artifacts:

- `/tmp/mob01-before-320.png`
- `/tmp/mob01-before-375.png`
- `/tmp/mob01-before-768.png`
- `/tmp/mob01-before-1024.png`
- `/tmp/mob01-before-1440.png`

After artifacts:

- `/tmp/mob01-after-320.png`
- `/tmp/mob01-after-375.png`
- `/tmp/mob01-after-768.png`
- `/tmp/mob01-after-1024.png`
- `/tmp/mob01-after-1440.png`

| Viewport | Before client/scroll | After client/scroll | After Header Y/H | After main Y/H | After Footer Y/H |
|---:|---:|---:|---:|---:|---:|
| 320×800 | 320/1100 | 320/320 | 0/105.03 | 105.03/634.70 | 739.73/60.27 |
| 375×812 | 375/1100 | 375/375 | 0/105.03 | 105.03/639.38 | 744.41/67.59 |
| 768×1024 | 768/1100 | 768/768 | 0/105.03 | 105.03/846.97 | 952/72 |
| 1024×768 | 1024/1100 | 1024/1024 | 0/142.70 | 142.70/529.30 | 672/96 |
| 1440×900 | 1440/1440 | 1440/1440 | 36/746.83 | 0/900 | 782.83/81.17 |

На всех viewport frame/Header/main/Footer имели `x >= 0` и `right <=
clientWidth`. На compact SVG и desktop links скрыты; burger видим и `44×44px`.
На 1440 SVG видим, content grid сохранил `86% / 14%`, burger скрыт.

320/375/768/1024: drawer открылся click/touch path, показал все 5 links,
остался внутри viewport (`275.19/280/280/300px`), закрылся close button; body
получал `overflow: hidden` только при open и восстанавливался. На 320 также
проверены overlay close, link-close и keyboard `Tab` + `Enter` для burger/close.

Desktop before/after: frame/content/main/Header/Footer bounding boxes совпали.
Typography, colors и frame geometry визуально не сместились. Единственная
видимая разница — скрыта прежняя неоформленная белая burger-линия, как требует
locked desktop criterion.

Fresh network session: runtime CSS, JS, assets и fixture SVG получили `200`;
`group-socials` получил `200`; intentional mock `admin/isAdmin` получил `404`
до и после. Новых console errors нет. Существующий Google Fonts stylesheet/font
загрузился с public CDN; запросов к production StoronnimV API/DB/Blob не было.

Диагностически открыты Home, Schedule, News, Music, Group, обе Video routes,
Admin, Developers и Error shell при 320px. Global `scrollWidth` остался 320.
Mock `404` не предоставлял feature content, поэтому page-specific clipping не
считалось проверенным и остаётся в MOB-02, MOB-03 и MOB-05.

## Команды и результаты

| Команда | Exit/result |
|---|---|
| `npm install --save-dev --save-exact sass@1.79.6` | `0` |
| `npm run styles:build` | `0` |
| `npm exec sass -- --version` | `0`, `1.79.6` |
| повторный `npm run styles:build` | `0`; CSS/map SHA-256 совпали |
| blocker `rg` для `1100px`/`respond-to(mobile)` | `1`, совпадений нет |
| `VITE_API_URL=https://api.example.test/api npm run build` | `0`; prebuild + 540 modules |
| bundle scan `localhost:44315|127.0.0.1` | `1`, совпадений нет |
| финальный `npm run lint` | `1`, тот же baseline 4 errors/2 warnings |
| targeted TSX lint | не требуется: TSX не менялся |

Generated CSS diff ограничен shared foundation sizing и responsive media
queries; feature-specific SCSS не перегенерирован с drift. Global
`overflow-x: hidden` не добавлялся.

## Acceptance criteria

- **pass** — dependency/status gate и MOB-01-only scope.
- **pass** — пользовательский diff сохранён; branch/HEAD/commit неизменны.
- **pass** — exact Sass dependency, scripts, README и deterministic artifacts.
- **pass** — `1100px` blocker и undefined mixin отсутствуют.
- **pass** — breakpoints ровно `768px`/`1024px` для shared foundation.
- **pass** — compact Header/content/Footer, no SVG, no global overflow.
- **pass** — compact burger/drawer/links/close/overlay/keyboard/body lock.
- **pass** — 1440 SVG/grid/colors/typography/frame baseline сохранён.
- **pass** — build и bundle scan green; lint не получил новых diagnostics.
- **pass** — before/after screenshots и viewport measurements существуют.
- **pass** — production API/DB/Blob и secrets не использовались.

Обязательных невыполненных проверок нет. Полный cross-browser/accessibility и
page-specific responsive audit остаются MOB-02–MOB-06 по утверждённому scope.
