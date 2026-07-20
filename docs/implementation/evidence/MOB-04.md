# MOB-04 — modals, media и states

**Дата:** 20 июля 2026 года  
**Вердикт:** PASS  
**Scope:** только shared modal/state mechanics, public Music/Group/Member request states, News/Schedule detail retry, public media policy и canonical SCSS artifacts.

## Dependency и preflight

- `MOB-02=done`, `MOB-03=done`, `MOB-04=planned`; активен `M3`.
- Initial branch `main`; HEAD `09f3b80e147706f436c4bc59065f1546064781ac`; финальные branch/HEAD не изменились.
- Initial `git diff --check`: exit `0`.
- Initial TypeScript: exit `0`.
- Initial full ESLint: exit `1`, ровно baseline 2 errors (`FrameLayout.tsx`, `Header.tsx`) и 2 hook warnings (`GroupDescription.tsx`, `MemberModal.tsx`).
- Initial status/diff inventory записан в `/tmp/storonnimv-mob04/preflight.md`. Существующие MOB-03/docs/generated CSS изменения и удалённый swap-файл классифицированы user-owned; они не откатывались.
- Disposable mock API слушал только `127.0.0.1:41822`; Vite — только `127.0.0.1:41823`; `VITE_API_URL` задан явно. Production API/DB/Blob не вызывались.

## RED evidence

До изменения News, Schedule и Member modals проверены в Chromium на `320x800` и `1440x900`.

- dialog role/`aria-modal`: отсутствовали;
- named close: отсутствовал;
- Escape: modal оставался открыт;
- focus: оставался на page trigger, Tab уходил в navbar;
- body scroll lock: отсутствовал;
- screenshots: `/tmp/storonnimv-mob04/before/`.

## Реализация

- Custom host заменён установленным `react-bootstrap@2.10.7 Modal`; `GlobalContext.OnShowModal`/`OnHideModal` contract сохранён.
- Custom `app-modal` root/backdrop/dialog/content/body classes ограничивают viewport; scroll находится только в body. Close всегда видим, имеет Ukrainian name, `44x44`, focus outline и stacking priority над existing admin controls.
- React Bootstrap обеспечивает backdrop/Escape/body lock/restore; explicit edge loop и focus sentinel закрывают native video и cross-origin iframe tab edges. Replacement content повторно фокусирует close без refocus при внутренних state rerендерах.
- `NoData` различает empty/status и error/alert, имеет retry focus/size contract; `ModalLoading` получил status semantics.
- Home, News, Schedule, Video, Music, Group и Member errors используют shared error variant.
- Music/Group/Member получили `loading/success/empty/error`, status validation, stale-data clear и exact retry. Member `404`/null — empty; non-`200`, invalid payload и network failure — error.
- News/Schedule detail error повторяет exact request. Member image имеет meaningful alt; compact member layout одноколоночный, URLs/names/descriptions wrap.
- News/promotion video используют `preload="metadata"`, `playsInline`, accessible label. Promotion `autoPlay` удалён; `controls`, `muted`, `loop` сохранены.
- Canonical SCSS скомпилирован только в tracked `style.css` и `style.css.map`; legacy per-partial CSS не генерировался.

## Changed files MOB-04

- Contexts: `MusicContext.tsx`, `GroupContext.tsx`.
- Shared: `ModalWindow.tsx`, `NoData.tsx`, `ModalLoading.tsx` и соответствующие SCSS partials.
- Consumers: `MusicPlatforms.tsx`, `GroupDescription.tsx`, `MemberModal.tsx`, `NewsSlider.tsx`, `ScheduleHomeContainer.tsx`, `PromotionVideoHome.tsx`, `NewsList.tsx`, `NewsModal.tsx`, `SchedulesList.tsx`, `ScheduleModal.tsx`, `VideoList.tsx`.
- Feature style: `MemberModal.scss`.
- Generated: `src/styles/style.css`, `src/styles/style.css.map`.
- State/evidence: этот файл, `00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`.

## Browser validation

### Versions и viewport matrix

- Chromium `149.0.7827.55`: full matrix PASS на `320x800`, `375x812`, `768x1024`, `1024x768`, `1440x900`, landscape `812x375`.
- WebKit `26.5`: smoke PASS на `375x812`, `1024x768`.
- Firefox `152.0.4`: smoke PASS на `375x812`, `1024x768` через existing cached executable.
- Microsoft Edge не установлен; branded Edge/full release audit остаётся `MOB-06`/`QA-05` по плану.

Chromium matrix покрыла News detail/video, Schedule detail/map, long Member, Home promotion, Music/Group/Member state machines и shared Video error. WebKit/Firefox проверили все три representative modals.

### Interaction и accessibility

- PASS: `role=dialog`, `aria-modal=true`, accessible name.
- PASS: opening/replacement focus close; Tab/Shift+Tab остаются внутри, включая native video и map iframe edges.
- PASS: Escape, backdrop, mouse click, keyboard Enter/Space и touch close; exact trigger focus restore.
- PASS: body locked только при open и точно восстановлен после close.
- PASS: close target `>=44x44`, visible focus, остаётся видим при body scroll; admin overlay не перехватывает touch.
- PASS: long body `scrollHeight > clientHeight`, достигает конца; modal/media bounds внутри viewport; root horizontal overflow отсутствует.
- PASS: `1440` black/yellow frame, hierarchy и proportions без blocking regression; deliberate close/semantics changes видимы.

### States и media

- PASS: delayed loading не показывает stale success; empty имеет `role=status` без retry; error имеет `role=alert`, red modifier и retry.
- PASS: Music `200 []` empty; non-`200`/invalid/network error; error-to-success retry.
- PASS: Group empty valid payload, partial success, non-`200`/invalid/network error и retry.
- PASS: Member `404` empty; `500`/invalid/network error; exact-member retry.
- PASS: News/Schedule detail error-to-loading-to-success retry.
- PASS: Home three error variants и Video error variant.
- PASS: promotion initial `autoplay=false`, `paused=true`, `preload=metadata`, `playsInline=true`; native keyboard control starts playback.
- PASS: real H.264/yuv420p News fixture starts through native control and reaches `ended=true` with `media.error=null`.
- PASS: happy application diagnostics `0`; seven deliberately aborted external iframe/resource requests recorded separately. Touch close PASS with admin controls present.

Screenshots: `/tmp/storonnimv-mob04/after/chromium/`, `/tmp/storonnimv-mob04/after/webkit/`, `/tmp/storonnimv-mob04/after/firefox/`. Binary artifacts не добавлены в Git.

## Static validation

| Check | Exit | Result |
|---|---:|---|
| `npm exec tsc -- -p tsconfig.app.json --noEmit --incremental false` | 0 | strict TypeScript green |
| targeted ESLint всех MOB-04 TSX | 0 | zero diagnostics; Group/Member hook warnings removed |
| `VITE_API_URL=https://api.example.test/api npm run build` | 0 | production build green, 540 modules |
| `npm run lint` | 1 | only documented QA-03 errors in `FrameLayout.tsx` and `Header.tsx`; 0 warnings/new diagnostics |
| `npm exec sass -- --version` | 0 | Sass `1.79.6` |
| repeated `npm run styles:build` + SHA-256 | 0 | identical: CSS `5c2a07e7…f4fc`, map `7dd96980…0ab` |
| bundle scan for `localhost:44315`, `127.0.0.1`, `storonnimv-mob04` | 1 | expected no matches |
| `git diff --check` | 0 | clean |
| changed-file secret filename scan | 1 | expected no matches |

Backend tests не запускались: backend/API/contracts/DB/Blob не менялись. Production/staging/full-release проверки не запускались: находятся вне `MOB-04` и требуют later milestone/access.

## Definition of Done verdicts

### Dependency, scope, safety

- PASS: dependencies/status/preflight recorded; user changes preserved.
- PASS: production changes ограничены заявленным frontend scope; packages/backend/API/schema/DB/Blob/deployment untouched.
- PASS: no production resource contact, secret write, commit, branch/worktree/stash/reset/clean/merge/rebase.
- PASS: MOB-05+ не начаты.

### Modal

- PASS: all mandatory widths/landscape fit; no root/media overflow.
- PASS: internal long scroll, persistent close, body lock/restore.
- PASS: mouse/touch/Enter/Space/Escape/backdrop close.
- PASS: Ukrainian close name, visible focus, `44x44` compact target.
- PASS: opening/replacement focus, focus containment, exact opener restore.
- PASS: dialog semantics/name and desktop visual baseline.

### State

- PASS: loading/empty/error distinct and semantic.
- PASS: retries/messages exact for all requested public consumers.
- PASS: Music/Group/Member status/error/empty rules, stale clear и exact retry.
- PASS: Group empty members renders explicit participant state.
- PASS: News/Schedule detail retry; original hook warnings removed without suppression.

### Media

- PASS: responsive image/video/iframe, meaningful Member alt, wrapping long content.
- PASS: News/promotion metadata+inline; promotion no autoplay and paused before action.
- PASS: real H.264 native-control playback; Schedule map/photo and MOB-03 Video behavior preserved.

### Verification/evidence

- PASS: before/after screenshots visually inspected; Chromium/WebKit/Firefox matrices pass.
- PASS: keyboard/touch/focus/scroll/replacement/state/media scenarios pass.
- PASS: TypeScript/targeted lint/build/Sass/determinism/bundle/diff/secret gates pass.
- PASS: full lint contains only two documented QA-03 errors and zero warnings.
- PASS: this evidence records performed/skipped checks, versions, artifacts, verdicts and scope.

## Out-of-scope findings

- Existing QA-03 errors remain in `FrameLayout.tsx` and `Header.tsx`; not blocking MOB-04.
- Edge unavailable locally; deferred exactly as planned, not blocking MOB-04.
- Existing external Spotify/map resources were blocked by disposable harness; application diagnostics remained clean. Production external-resource audit remains later QA.

`MOB-04` accepted as `done`. `M3` remains active. Next backlog task: `MOB-05`, not started.
