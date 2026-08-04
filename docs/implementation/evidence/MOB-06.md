# MOB-06 — cross-device accessibility audit

**Дата:** 4 августа 2026 года
**Вердикт:** PASS
**Итог:** обязательная branded Chrome/Safari/Firefox/Edge matrix, reduced-motion scenarios и static/repository gates прошли.

## Цель и scope

Цель — закрыть системные mobile/accessibility regressions frontend: overflow и clipping, DOM/tab order, keyboard/focus, accessible names, landmarks/headings/lists, loading/error/form semantics, hover-only behavior, touch targets, contrast и reduced motion.

В scope вошли только React-компоненты и canonical SCSS frontend, tracked `style.css`/`style.css.map`, disposable localhost mock/audit harness и этот evidence. Backend, API contracts/models, DB/Blob, production/staging, P3 polish, новая test architecture и dependency updates не затрагивались. `QA-02` не начата.

## Dependency и preflight

- Перед реализацией подтверждены `MOB-04=done`, `MOB-05=done`, `MOB-06=planned`; после полного PASS `MOB-06=done`, `M3=complete`, активен `M4`.
- Initial branch: `main`; initial HEAD: `1bc88278f4d316fffc38fb8f81f45fd411fea97e`.
- Branch не переключалась; HEAD до closeout commits совпадает с initial HEAD.
- Исходные owner-owned файлы `.DS_Store`, `TEST_REPORT.md` и предоставленный plan сохранены и не входят в MOB-06 diff/staging.
- Initial TypeScript и `git diff --check`: exit `0`.
- Initial full lint: exit `1`, только два `@ts-ignore` errors в `FrameLayout.tsx` и `Header.tsx`, warnings `0`.
- Реальные branded browsers: Chrome `151.0.7922.72`, Safari `26.5.2`, Firefox `153.0.1`, Edge `151.0.4129.59`.
- Chrome, Firefox и Edge установлены с разрешения владельца. Safari Remote Automation включена после отдельного явного разрешения и остаётся включённой.
- Axe `4.12.1`, mock server, scripts, reports, screenshots и silent media fixture находятся только в `/tmp/storonnimv-mob06`; tracked package/lock не изменены.

## RED baseline

Baseline восстановлен из отдельной disposable-копии exact initial HEAD после случайной перезаписи каталога harness-ом. Рабочее дерево при восстановлении не менялось.

- Chrome: 54 route/viewport cases.
- Axe violations: `image-alt`, `landmark-one-main`, `link-name`, `page-has-heading-one`, `region`.
- `13` cases с root/descendant overflow; `48` cases с неверным количеством `<h1>`.
- Public frame имел DOM `main div -> navbar div`; semantic `header/main/footer` отсутствовали.
- Drawer не имел полного keyboard/focus contract; desktop sidebar clipping, hover-only news titles и reduced-motion regressions подтверждены.
- RED screenshots/report: `/tmp/storonnimv-mob06/before/`.

## Затронутые файлы и решения

- `index.html`, `App.tsx`, public/admin pages: Ukrainian document language, один logical route `<h1>`, public/admin landmark contract.
- `FrameLayout`, `Header`, `Footer`: DOM `header -> main -> footer`, skip link, decorative SVG, bounded desktop sidebar, named links, compact drawer focus management.
- `GenericList`, News/Video/Music/Schedule lists, loading и empty/error components: valid list semantics, stable keys, single live loading announcement, decorative skeletons, status/alert roles.
- Admin auth/add/edit/delete validation: deterministic error ids, `aria-invalid`, exact `aria-describedby`, alert linkage, semantic confirmation prose.
- New `usePrefersReducedMotion`: shared live media-query subscription with cleanup. Swipers and Group Web Animation observe it; CSS motion sources have explicit reduce rules.
- News focus parity, pagination/footer/sidebar `44x44` targets, contrast fixes, narrow Error/modal layout, invalid image/video fallbacks.
- Canonical SCSS partials plus regenerated tracked `style.css` and `style.css.map`; orphan `HeaderWithFooter` and `GenericList/ListItem` TSX abstractions removed.

Internal contracts match the implementation plan:

- `FrameLayoutProps = { children, header, footer }`.
- `main#main-content[tabindex=-1]` remains on public/admin routes; empty header/footer landmarks are not rendered for admin.
- `PreloaderTile` accepts `announce?: boolean`; one tile per loading region announces, the others are decorative.
- `ValidationErrors` requires an `idPrefix`; field ids are deterministic.
- `usePrefersReducedMotion(): boolean` uses `matchMedia`, the `change` event and symmetric cleanup.

## Выполненные изменения

- Compact DOM и visual order теперь одинаковы; public routes имеют `header -> main -> footer`, admin — только `main`.
- Skip link является первым keyboard target и переводит focus в `main-content`.
- Drawer сообщает state/name, получает initial focus, traps Tab/Shift+Tab, закрывается Escape/link/logout/overlay/close, restores focus и восстанавливает `body.style.overflow`.
- Logo, social links, invalid-link substitutes, decorative SVG/images, headings, lists, loading/empty/error regions и iframe wrapper получили проверяемую semantic структуру.
- Auth и admin server errors связаны с точными controls.
- Desktop keyboard focus раскрывает News/Home News content эквивалентно hover.
- Required interactive boxes измерены как минимум `44x44 CSS px`; central-point hit tests проходят.
- Reduced mode отключает Swiper autoplay/transition, Group auto-scroll и известные CSS animations/transitions; normal mode сохраняет autoplay `3000ms`, speed `1800ms` и Group behavior.
- Confirm/pagination/schedule/logout foreground/background pairs исправлены; измеренные ratios перечислены ниже.
- Broken Home images получают local fallback или скрываются после второго failure; broken promo video заменяется retryable alert state.

## Static/build/style/security gates

Все команды выполнялись из `frontend/storonnimv.client`, кроме явно отмеченных Git checks.

| Check | Exit | Доказательство |
|---|---:|---|
| `npm exec tsc -- -p tsconfig.app.json --noEmit --incremental false` | 0 | TypeScript green без incremental cache |
| targeted ESLint всех changed TS/TSX | 0 | diagnostics `0` |
| `npm run lint` | 0 | errors/warnings `0`; два baseline `@ts-ignore` устранены через объяснённые `@ts-expect-error` |
| `npm exec sass -- --version` | 0 | exact Sass `1.79.6` |
| два последовательных `npm run styles:build` | 0 | CSS/map deterministic |
| SHA-256 `style.css` | 0 | `980c081b449498dc2be505101048124994d39d2c74ee5651a902ff141cb58647` |
| SHA-256 `style.css.map` | 0 | `39f3f636df1a8651f0387d3e45c5b1a5b24498e1cd7bca2612ef8ba090040589` |
| `VITE_API_URL=https://api.example.test/api npm run build` | 0 | Vite `6.0.7`, `540` modules |
| app-owned bundle local API/mock/task scan | 1 | expected no matches |
| changed source secret-pattern scan | 1 | expected no matches |
| `git diff --check` from repository root | 0 | clean |

Generic Axios code contains its upstream inert `http://localhost` origin fallback; refined app-owned API scan has matches `0`. A broad generated-CSS credential regex also identifies a Swiper embedded base64 icon-font fragment; traced to `node_modules/swiper/swiper.css`, not an app secret or source credential. Raw secret-like values are intentionally omitted.

Legacy per-partial CSS/maps не изменены. Backend tests не запускались: backend/API/contracts/schema не затронуты. Production DB/Blob/staging не открывались и не проверялись по scope.

## Browser/device acceptance

### Chrome stable — full matrix

- Routes: Home, Schedule, News, Music, Group, Video sections/list, Admin/auth/admin list, Error.
- Viewports: `320x800`, `375x812`, `768x1024`, `1024x768`, `1440x900`; landscape `812x375`.
- Final 54-case audit: axe violations `0`, overflow cases `0`, wrong-h1 cases `0`, request failures `0`.
- Public landmark/frame order `header -> main -> footer`; admin order `main`.
- State matrix: loading/empty/error/long content, six data domains at `320` and `1440`; 48 cases, violations/overflow/hit failures `0`.
- Drawer, skip link, manual keyboard path, three representative modals, auth/admin validation, touch/coarse-pointer and invalid-media scenarios pass.
- Reduced scenario ran longer than 5.2 seconds: slider index unchanged, Group transform unchanged, skeleton/spinner animation `none`; normal slider advanced.
- Focused post-review regression confirms runtime motion toggles for Home and member Swipers (`autoplay true -> false -> true`), fully visible long Group text in reduced mode, and drawer cleanup/focus restoration when resizing from `1024px` to desktop.
- Console has only expected anonymous `401 Unauthorized` admin-state probes; no app-thrown warnings/errors.
- Final screenshots/reports: `/tmp/storonnimv-mob06/chrome-final/`, `/tmp/storonnimv-mob06/after-final/` and `/tmp/storonnimv-mob06/states/`.

### Firefox stable — core matrix

- 24 core route cases at `375x812`, `1024x768`, `1440x900`; violations `0`, overflow `0`.
- Portrait/landscape drawer focus containment/Escape/restore, representative modal and reduced-motion scenario pass.
- Evidence: `/tmp/storonnimv-mob06/firefox-final/`.

### Edge stable — core/full matrix

- 54 route/viewport cases; violations `0`, overflow `0`, wrong-h1 `0`, request failures `0`.
- Structure/drawer, forms, representative modal and motion checks pass.
- Console contains only expected anonymous `401` probes.
- Evidence: `/tmp/storonnimv-mob06/edge-final/`.

### Safari stable — core matrix PASS

- Safari `26.5.2`, 24 core route cases at `375x812`, `1024x768`, `1440x900`.
- Axe app-owned violations `0`; root/descendant overflow `0`; каждый route имеет один `<h1>` и правильные landmarks.
- Drawer portrait/landscape: initial focus, bounds, Tab/Shift+Tab containment, Escape, restore focus и body unlock прошли.
- Representative News modal имеет accessible name, viewport bounds и no horizontal overflow.
- System `Reduce Motion` был временно переключён из исходного `off` в `on` и после проверки возвращён в `off`.
- Reduced Safari scenario: media query `true`; Home Swiper autoplay `false`, active index `0 -> 0` через `5.2s`, transition `0s`; Group transform `none`, animations `0 -> 0` через `4.2s`; skeleton animation `none`; drawer/overlay transition `0s`, focus/Escape/restore работают.
- Evidence: `/tmp/storonnimv-mob06/safari-final/summary.json`, `reduced-motion.json` и screenshots в том же каталоге. Ранние partial manual artifacts остаются в `/tmp/storonnimv-mob06/safari/`, но итоговый PASS основан на WebDriver evidence.

## Axe incomplete manual resolution

- `color-contrast`: image/transparent-background nodes требуют manual context. Проверенные app-owned fixed pairs: confirm `7.87:1`, hover `10.90:1`, pagination `17.99:1`, active `14.21:1`, schedule modal `10.32:1`, logout `11.23:1`, hover `13.33:1`, disabled pagination `8.67:1`.
- `video-caption`: disposable MP4 silent, без речи/аудио; captions N/A для fixture.
- `frame-tested`: Spotify — third-party iframe; app wrapper title `Spotify — Стороннім В`, name и bounds проверены. Содержимое third-party frame отделено от app-owned UI.

## Visual/touch/keyboard evidence

С одинаковыми route/viewport names визуально сравнены desktop sidebar, compact header/open drawer, Home, News, Group long text, Video, admin validation и Error. After подтверждает отсутствие clipping/overflow, bounded targets, visible focus и сохранение desktop composition без P3 redesign.

- Before: `/tmp/storonnimv-mob06/before/`.
- Chrome after: `/tmp/storonnimv-mob06/chrome-final/`.
- Safari after/reduced motion: `/tmp/storonnimv-mob06/safari-final/`.
- Firefox after: `/tmp/storonnimv-mob06/firefox-final/`.
- Edge after: `/tmp/storonnimv-mob06/edge-final/`.
- Extra state/form/media evidence: `/tmp/storonnimv-mob06/after-final/` и `/tmp/storonnimv-mob06/states/`.

## Acceptance criteria

| Criterion | Status | Evidence |
|---|---|---|
| Device matrix green | PASS | Chrome full matrix; Safari/Firefox/Edge core matrices green |
| Keyboard path complete | PASS | skip/drawer/modal/forms/navigation/focus paths green в required branded scope |
| No hover-only blocker | PASS | News focus parity и coarse-pointer/touch checks green |
| Axe/manual keyboard/screenshots/overflow | PASS | app-owned axe violations `0`; manual incomplete classified; required geometry/screenshots green |

## Definition of Done

| Раздел | Status | Причина |
|---|---|---|
| A. Scope, dependencies, hygiene | PASS | scope/dependencies clean; user-owned files preserved; QA-02 untouched |
| B. Layout, DOM order, overflow | PASS | required route/state/viewport matrices green |
| C. Navigation/keyboard | PASS | drawer/modal/focus contracts green in all four branded browsers |
| D. Names/language/semantics | PASS | app-owned axe/DOM/manual checks green |
| E. States/forms | PASS | loading/empty/error/form linkage and `320px` scenarios green |
| F. Hover/touch/focus/contrast | PASS | measured interaction boxes, hit tests and contrast pairs green |
| G. Reduced motion | PASS | Chrome/Firefox/Edge and system-preference Safari scenarios green |
| H. Axe/branded matrix | PASS | real stable Chrome/Safari/Firefox/Edge required matrices green |
| I. Static/build/style/security | PASS | all current gates green; documented dependency false positives classified |
| J. Evidence/state integrity | PASS | evidence complete; MOB-06/state/index updated only after full browser PASS; Wiki sync committed separately from committed evidence |

## Safety, omissions и closeout

- Владелец отдельно разрешил commits/push, явно разрешил `safaridriver --enable` и вручную авторизовал Safari security prompt. До full browser PASS commit не создавался; branch не переключалась.
- Implementation commit: `5630ad2a32259f894f3765c001d2d3f524485907` (`Complete MOB-06 accessibility audit`).
- Obsidian Project Wiki assessment: `success`; changed lint scanned `6` affected pages with findings `0`, atomic candidate transaction and exact staging produced separate main commit `6597db6cdb3f2850c76bb53204beb5bb8aaa3af0` (`docs(wiki): sync after 5630ad2`).
- `MOB-06=done`; `M3=complete`; active milestone `M4`; next unblocked `QA-02`, не начата.
- Wiki knowledge impact подтверждён: frontend shell/accessibility contracts и roadmap status синхронизированы только после code/evidence commit.
- Out-of-scope blocker findings отсутствуют. Expected anonymous `401`, third-party Spotify frame и upstream bundle scan false positives — non-blockers.
- Safari Remote Automation остаётся включённой согласно явному разрешению владельца. Temporary system Reduce Motion восстановлен в исходное `off`.
