# MOB-04 Modals, Media and States Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:executing-plans` to implement this plan task-by-task. Task narrow and linear; do not dispatch subagents. Checkboxes track execution. Do not create commits.

**Goal:** Make shared modals, responsive modal media, and `loading/empty/error/retry` states usable on mobile and tablet while preserving desktop behavior.

**Architecture:** Replace only custom modal host mechanics with already-installed `react-bootstrap@2.10.7` `Modal`, isolated behind custom `app-modal` classes. Keep `GlobalContext.OnShowModal` contract and existing feature architecture. Add explicit request statuses only where Music, Group, and Member flows currently conflate empty and error; compile canonical SCSS into tracked runtime CSS.

**Tech Stack:** React 18.3.1, TypeScript 5.6, React Bootstrap 2.10.7, Vite 6, Sass 1.79.6, Playwright CLI/browser harness.

## Global Constraints

- Execute only backlog task `MOB-04`. Do not begin `MOB-05` or any later task.
- `MOB-02` and `MOB-03` must remain `done`; active milestone remains `M3`.
- Preserve existing React/TypeScript/Vite architecture and desktop runtime `src/styles/style.css` baseline.
- Use canonical SCSS partials and regenerate only `style.css` plus `style.css.map` through `npm run styles:build`.
- Do not add or update dependencies. `react-bootstrap@2.10.7` is already installed.
- Do not change backend, API contracts, DB, Blob, production resources, admin form/table layout, or deployment configuration.
- Treat current dirty worktree as user-owned. Never revert or overwrite existing MOB-03/docs changes.
- Do not commit, switch branch, stash, reset, clean, merge, or rebase.
- Never write secrets to source, evidence, commands, screenshots, or logs.
- If any mandatory acceptance criterion fails, leave `MOB-04` planned/current and record exact blocker.

---

## Current Baseline

- Branch: `main`.
- HEAD at planning: `09f3b80e147706f436c4bc59065f1546064781ac`.
- Existing dirty changes belong to MOB-03/user work, including generated `style.css`/map and implementation state docs.
- `npm exec tsc -- -p tsconfig.app.json --noEmit --incremental false`: exit `0`.
- `npm run lint`: exit `1`, baseline `2 errors/2 warnings`.
- Errors outside MOB-04: `FrameLayout.tsx`, `Header.tsx`.
- Warnings inside MOB-04: missing hook dependencies in `GroupDescription.tsx`, `MemberModal.tsx`; both must be removed by correct callback dependencies.
- `git diff --check`: exit `0`.
- Root repository `AGENTS.md` is absent; user instructions and `frontend/AGENTS.md` apply.

## Interfaces

### Preserved

```ts
OnShowModal: (mContent: ReactNode, mTitle?: string) => void;
OnHideModal: () => void;
```

No caller-wide modal API migration. Existing feature components continue sending React content through `GlobalContext`.

### Added

```ts
type RequestStatus = "loading" | "success" | "empty" | "error";

interface INoDataProps {
    style?: IStyle;
    className?: string;
    message?: string;
    actionLabel?: string;
    onAction?: () => void;
    variant?: "empty" | "error";
}
```

- `MusicContextType` adds `musicStatus: RequestStatus`.
- `GroupContextType` adds `groupStatus: RequestStatus` and `memberStatus: RequestStatus`.
- No HTTP, DTO, backend, or persisted-data interface changes.

## Task 1: Preflight and RED Evidence

**Files:** No tracked files.

- [ ] Read current user/root instructions, `frontend/AGENTS.md`, required implementation docs, MOB-04 backlog row, M3 milestone, validation plan, state, relevant analysis, and files listed in later tasks.
- [ ] Confirm `MOB-04` exists and `MOB-02`/`MOB-03` remain `done`. Stop before edits if either dependency is not done.
- [ ] Create disposable `/tmp/storonnimv-mob04/` and store preflight status/diff inventory there. Do not place raw logs in repository.
- [ ] Run and record:

```bash
git status --short
git diff --check
git diff --name-status
git branch --show-current
git rev-parse HEAD
```

Expected: current user diff present; diff check exit `0`; branch `main`; HEAD unchanged.

- [ ] From `frontend/storonnimv.client`, run baseline checks:

```bash
npm exec tsc -- -p tsconfig.app.json --noEmit --incremental false
npm run lint
```

Expected: TypeScript exit `0`; lint exit `1` with exactly two errors and two warnings described above.

- [ ] Start disposable local mock API and Vite with explicit localhost-only `VITE_API_URL`. Mock `/api/admin/isAdmin`, `/api/group-socials`, and routes used by News, Schedule, Group, Music, Video, and Home. Do not contact production resources.
- [ ] Capture before screenshots for News, Schedule, and Member modals at `320×800` and `1440×900`.
- [ ] Record RED assertions proving current defect:
  - no `role="dialog"`/`aria-modal`;
  - no named close control;
  - Escape does not close;
  - focus is not contained/restored;
  - body scroll is not locked;
  - long member content/outer modal mechanics are not verified responsive.

## Task 2: Replace Shared Modal Mechanics

**Files:**

- Modify: `frontend/storonnimv.client/src/components/elements/shared/ModalWindow.tsx`
- Modify: `frontend/storonnimv.client/src/styles/elements/shared/ModalWindow.scss`
- Generated later: `frontend/storonnimv.client/src/styles/style.css`
- Generated later: `frontend/storonnimv.client/src/styles/style.css.map`

**Consumes:** Existing `GlobalContext` modal state and callbacks.

**Produces:** Accessible, viewport-bound global modal host with unchanged caller contract.

- [ ] Replace custom overlay markup with `react-bootstrap/Modal` configured exactly with:
  - `show={showModal}`;
  - `onHide={OnHideModal}`;
  - `bsPrefix="app-modal"`;
  - `backdrop` and `keyboard` enabled;
  - `autoFocus`, `enforceFocus`, and `restoreFocus` enabled;
  - `scrollable` enabled;
  - `aria-label={modalTitle || "Діалогове вікно"}`.
- [ ] Add native close button inside modal content:

```tsx
<button
    ref={closeButtonRef}
    type="button"
    className="app-modal__close"
    aria-label="Закрити діалогове вікно"
    onClick={OnHideModal}
>
    <span aria-hidden="true">×</span>
</button>
```

- [ ] Use `useRef<HTMLButtonElement>(null)` and `useEffect` to focus close button when modal opens and whenever `modalContent` is replaced while open. Do not refocus during internal loading-to-success rerenders where `modalContent` identity stays unchanged.
- [ ] Remove empty effect and old `.modal active` mechanics.
- [ ] Implement custom classes without importing Bootstrap global CSS:
  - `.app-modal` fixed viewport root and scroll container;
  - `.app-modal-backdrop` opaque backdrop below dialog;
  - `.app-modal-dialog` width/margin boundary;
  - `.app-modal-content` current black/yellow border/radius theme;
  - `.app-modal__body` only scrolling content area;
  - `.app-modal__close` persistent named close control.
- [ ] Desktop: preserve black background, yellow `2px` border, `12px` radius, roughly `50–80vw` width and `80vh` maximum height.
- [ ] `<=1024px`: outer margin at most `0.5rem`, `min-width:0`, width inside viewport, maximum height `calc(100dvh - 1rem)`, body-only scrolling, close button at least `44×44px`.
- [ ] Do not hide scrollbar. Apply `overscroll-behavior: contain`.
- [ ] Add visible `:focus-visible` outline for close button.
- [ ] Limit modal descendants `img`, `video`, and `iframe` to `max-width:100%`.
- [ ] Disable modal transition only under `prefers-reduced-motion: reduce`. Do not perform global MOB-06 motion audit.
- [ ] Run targeted check:

```bash
npm exec eslint -- src/components/elements/shared/ModalWindow.tsx
```

Expected: exit `0`, zero diagnostics.

- [ ] Browser GREEN smoke: open via keyboard, focus close, Tab/Shift+Tab stays inside, Escape closes, focus returns to trigger, backdrop closes, body overflow restores after close.

## Task 3: Make Shared States Distinct

**Files:**

- Modify: `frontend/storonnimv.client/src/components/elements/shared/NoData.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/shared/ModalLoading.tsx`
- Modify: `frontend/storonnimv.client/src/styles/elements/shared/NoData.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/shared/ModalLoading.scss`
- Modify error callers under `components/elements/home`, `news`, `schedule`, and `video`.

**Produces:** Shared empty/error/loading semantics and consistent retry presentation.

- [ ] Add `variant?: "empty" | "error"` with default `empty`.
- [ ] Build class name without literal `undefined`.
- [ ] Empty state uses `role="status"`; error uses `role="alert"` and `empty-data-container--error`.
- [ ] Error modifier uses existing cancel red palette for border/background; empty keeps current yellow/neutral presentation.
- [ ] Make state text readable on compact widths with `clamp`, image responsive, retry button minimum `44px`, and visible focus.
- [ ] `ModalLoading` uses `role="status"`, `aria-label="Завантаження"`; spinner node is `aria-hidden="true"`.
- [ ] Pass `variant="error"` to every public Home, News, Schedule, and Video error branch. Empty branches retain default.
- [ ] Keep existing Ukrainian messages and retry label `Спробувати ще раз`.
- [ ] Run targeted ESLint on changed TSX. Expected exit `0`.

## Task 4: Add Music and Group Request State Machines

**Files:**

- Modify: `frontend/storonnimv.client/src/components/contexts/MusicContext.tsx`
- Modify: `frontend/storonnimv.client/src/components/contexts/GroupContext.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/music/MusicPlatforms.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/group/GroupDescription.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/group/MemberModal.tsx`
- Modify: `frontend/storonnimv.client/src/styles/elements/music/MusicPlatforms.scss` only if state container needs scoped responsive sizing.
- Modify: `frontend/storonnimv.client/src/styles/elements/group/MemberModal.scss`

**Produces:** Explicit Music, Group, and Member loading/success/empty/error/retry contracts.

- [ ] `MusicContext`:
  - initialize `musicStatus` as `loading`;
  - clear stale platforms and set loading before request;
  - require HTTP `200` and array response;
  - set empty for zero items, success otherwise;
  - set error for non-`200`, invalid shape, or network failure;
  - always clear shared page loading in `finally`.
- [ ] `MusicPlatforms` branches only on `musicStatus`, not shared `pageLoading` or `checkIfNoData`.
- [ ] Music error copy: `Не вдалося завантажити музичні платформи`; retry calls `void fetchMusicPlatforms()`.
- [ ] Remove inline `vw` state sizing that overrides compact readability.
- [ ] `GroupContext`:
  - expose `groupStatus` and `memberStatus`;
  - wrap both fetch functions in `useCallback` with complete dependencies;
  - clear stale data before request;
  - require `200` for success;
  - treat member `404` or empty payload as empty;
  - treat non-`200`, invalid shape, or network failure as error;
  - clear matching loading flag in `finally`.
- [ ] `GroupDescription`:
  - effect calls `void fetchGroupInfo()` and includes it in dependencies;
  - render separate loading, error/retry, empty, and success branches;
  - use `fullInfo.members.length > 0`, not array truthiness;
  - apply background image only for nonempty URL;
  - partial success may show description plus `Дані про учасників відсутні`.
- [ ] Group error copy: `Не вдалося завантажити дані про групу`; retry calls `void fetchGroupInfo()`.
- [ ] `MemberModal`:
  - effect calls `void fetchMemberInfo(memberId)` with both dependencies;
  - render separate loading, error/retry, empty, and success branches;
  - error copy `Не вдалося завантажити дані учасника`;
  - empty copy `Дані про учасника відсутні`;
  - retry repeats exact member request.
- [ ] Add meaningful member image alt containing `memberFullInfo.fullName`.
- [ ] Compact Member modal: one-column photo/info/description/social layout, full-width social cards, wrapping long names/roles/descriptions/URLs, no horizontal overflow.
- [ ] Do not adapt admin forms/tables or unrelated global admin button layout.
- [ ] Run targeted ESLint:

```bash
npm exec eslint -- \
  src/components/contexts/MusicContext.tsx \
  src/components/contexts/GroupContext.tsx \
  src/components/elements/music/MusicPlatforms.tsx \
  src/components/elements/group/GroupDescription.tsx \
  src/components/elements/group/MemberModal.tsx
```

Expected: exit `0`; both original Group hook warnings gone.

- [ ] Controlled browser cases for Music, Group, and Member: delayed loading, success, `200` empty, detail `404`, controlled `500`/network failure, error-then-success retry.

## Task 5: Finish Modal Media and Detail Retry

**Files:**

- Modify: `frontend/storonnimv.client/src/components/elements/news/NewsModal.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/schedule/ScheduleModal.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/home/PromotionVideoHome.tsx`
- Modify existing feature SCSS only where modal shell rules cannot safely constrain media.

**Produces:** Responsive, user-controlled modal/promotion media and retryable detail errors.

- [ ] News detail error uses `variant="error"`, `Спробувати ще раз`, and `() => void fetchNewsFullItem(newsId)`.
- [ ] Schedule detail error uses `variant="error"`, `Спробувати ще раз`, and `() => void fetchScheduleFullInfo(scheduleId)`.
- [ ] News video uses `controls`, `preload="metadata"`, `playsInline`, responsive dimensions, and accessible label containing news title.
- [ ] Promotion video:
  - remove `autoPlay` on every viewport;
  - use `preload="metadata"` and `playsInline`;
  - retain native controls, muted, and loop;
  - add accessible label containing promotion title.
- [ ] Preserve Schedule photo/map semantics and existing MOB-02 responsive rules.
- [ ] Preserve Video list playback contract from MOB-03; only shared error variant may change.
- [ ] Run targeted ESLint on all changed detail/media TSX. Expected exit `0`.
- [ ] Use controlled real H.264 MP4 fixture. Before user action verify promotion `autoplay === false`, `paused === true`, `preload === "metadata"`, `playsInline === true`. Start via native controls and verify playback ends without media error.

## Task 6: Generate Runtime CSS and Run Full Browser Matrix

**Files:**

- Modify through generation only: `frontend/storonnimv.client/src/styles/style.css`
- Modify through generation only: `frontend/storonnimv.client/src/styles/style.css.map`

- [ ] Run:

```bash
npm exec sass -- --version
npm run styles:build
shasum -a 256 src/styles/style.css src/styles/style.css.map
npm run styles:build
shasum -a 256 src/styles/style.css src/styles/style.css.map
```

Expected: Sass `1.79.6`; both builds exit `0`; first/second hashes identical.

- [ ] Do not regenerate or edit legacy per-partial `.css`/`.css.map` files.
- [ ] Full Chromium matrix:
  - `320×800`;
  - `375×812`;
  - `768×1024`;
  - `1024×768`;
  - `1440×900`;
  - landscape `812×375`.
- [ ] Cover News detail, Schedule detail/map, long Member modal, Home promotion, Music states, Group states, and shared Video error state.
- [ ] WebKit/Safari and Firefox smoke at `375×812` and `1024×768`. Record actual versions. Microsoft Edge is not installed locally; branded Edge/full release audit remains MOB-06/QA-05.
- [ ] For every representative modal assert:
  - `role="dialog"`, `aria-modal="true"`, accessible name;
  - named close target at least `44×44` on compact viewport;
  - focus starts on close;
  - Tab/Shift+Tab stays inside;
  - Escape, close click/touch, and backdrop close;
  - focus returns to exact trigger;
  - replacement content refocuses close;
  - body is locked only while open;
  - long body has `scrollHeight > clientHeight` and reaches bottom;
  - close remains visible;
  - root `scrollWidth === clientWidth`;
  - dialog/media bounds stay inside viewport.
- [ ] State assertions:
  - loading never displays stale success content;
  - error has `role=alert`, error modifier, retry;
  - empty has `role=status`, distinct copy/appearance, no retry;
  - retry transitions through loading to factual result.
- [ ] Capture after screenshots matching before artifacts plus full mandatory matrix. Store under `/tmp/storonnimv-mob04/`, not Git.
- [ ] Inspect `1440` before/after: frame, theme, hierarchy, modal content and page geometry have no blocking regression beyond deliberate close/semantics changes.
- [ ] Happy-path console has no new errors/warnings. Controlled error fixtures may contain only expected request/application diagnostics.

## Task 7: Static Validation and Scope Audit

- [ ] Run TypeScript and production build:

```bash
npm exec tsc -- -p tsconfig.app.json --noEmit --incremental false
VITE_API_URL=https://api.example.test/api npm run build
```

Expected: both exit `0`.

- [ ] Run targeted ESLint over every MOB-04 TSX. Expected exit `0`, zero diagnostics.
- [ ] Run full lint:

```bash
npm run lint
```

Expected: exit `1` only because documented QA-03 errors remain in `FrameLayout.tsx` and `Header.tsx`; zero warnings and no new diagnostics. Do not fix those errors in MOB-04.

- [ ] Scan production bundle; expected `rg` exit `1`, no matches:

```bash
rg -n 'localhost:44315|127\.0\.0\.1|storonnimv-mob04' dist
```

- [ ] Run repository checks:

```bash
git diff --check
git status --short
git diff --name-status
git diff --stat
git diff
git branch --show-current
git rev-parse HEAD
```

Expected: diff check exit `0`; branch/HEAD unchanged; no MOB-05 files; no commit.

- [ ] Scan changed source/generated/docs for private keys, cloud account keys, connection strings, bearer/JWT literals. Expected no matches. Do not print secret values if any suspected match appears.
- [ ] Review every changed hunk against MOB-04. Preserve original MOB-03/user hunks in overlapping generated/docs files.
- [ ] Do not run backend tests because backend/API/contracts are unchanged. Record this reason in evidence; do not claim backend validation.

## Task 8: Evidence and State Updates

**Files:**

- Create: `docs/implementation/evidence/MOB-04.md`
- Modify after full acceptance only: `docs/implementation/04_BACKLOG.md`
- Modify after full acceptance only: `docs/implementation/09_STATE.md`
- Modify: `docs/implementation/00_INDEX.md`

- [ ] Evidence includes:
  - task goal and scope;
  - dependency/preflight result;
  - original dirty worktree classification;
  - changed files;
  - selected React Bootstrap approach and no-autoplay decision;
  - implemented modal/state/media behavior;
  - commands, exit codes, and what each proves;
  - browser versions, viewport matrix, accessibility/interaction results;
  - screenshot paths without embedding binary artifacts;
  - skipped checks and exact reasons;
  - out-of-scope findings and whether they block MOB-04;
  - pass/fail verdict for every acceptance criterion.
- [ ] Avoid long raw logs and secrets.
- [ ] Only after every mandatory criterion passes:
  - set MOB-04 status to `done`;
  - add MOB-04 to backlog done summary;
  - set next backlog task to `MOB-05`;
  - keep milestone `M3` active;
  - update state with actual validation and confirmed facts;
  - add evidence link to index.
- [ ] If any mandatory criterion fails:
  - keep MOB-04 `planned` and current;
  - do not name MOB-05 as current implementation work;
  - record diagnostic blocker and minimal owner decision/resource required.

## Definition of Done

MOB-04 is complete only when every checked condition below is supported by recorded evidence.

### Dependency, scope, and worktree safety

- [ ] MOB-04 exists; MOB-02 and MOB-03 are `done`.
- [ ] Initial branch, HEAD, status, diff inventory, TypeScript, lint, and diff-check baselines are recorded.
- [ ] All pre-existing user changes remain intact.
- [ ] Changed production code is limited to shared modal/states, Music/Group state consumers, modal media, and necessary canonical styles/artifacts.
- [ ] No backend, API schema, DB, Blob, package, deployment, or architecture change exists.
- [ ] No production resource was contacted or mutated.
- [ ] No commit or prohibited Git operation occurred.
- [ ] MOB-05 and later backlog tasks were not started.

### Modal behavior

- [ ] Modal fits entirely within `320`, `375`, `768`, `1024`, and `1440` widths.
- [ ] Long modal content scrolls inside dialog; underlying page stays locked.
- [ ] Close remains visible while body scrolls.
- [ ] Close works with mouse, touch, Enter, and Space.
- [ ] Close control has Ukrainian accessible name, visible focus, and compact target at least `44×44px`.
- [ ] Escape and backdrop close modal.
- [ ] Opening moves focus to close.
- [ ] Tab and Shift+Tab cannot move focus outside dialog.
- [ ] Closing returns focus to exact opener.
- [ ] Replacing modal content does not leave focus on removed node.
- [ ] Dialog exposes `role=dialog`, `aria-modal=true`, and accessible name.
- [ ] Body scroll state is restored exactly after closure.
- [ ] No modal/media combination creates horizontal root overflow.
- [ ] Desktop `1440` theme, proportions, hierarchy, and content behavior have no blocking regression.

### State behavior

- [ ] Loading, empty, and error are visually distinct.
- [ ] Empty uses status semantics; error uses alert semantics.
- [ ] Error always shows exact Ukrainian message and retry where request can be repeated.
- [ ] Music, Group, and Member expose explicit loading/success/empty/error status.
- [ ] Non-`200`, invalid payload, and network failure never render as empty.
- [ ] Music zero-length array renders empty, not error.
- [ ] Group empty members array renders participant empty state, not blank success.
- [ ] Member `404` renders empty; member `500`/network failure renders error.
- [ ] Retry repeats exact request, renders loading, clears stale content, and resolves from factual response.
- [ ] News and Schedule detail retries work inside modal.
- [ ] Home, News, Schedule, Video, Music, Group, and Member errors use shared error variant.
- [ ] Original Group/Member hook warnings are eliminated without suppression.

### Media behavior

- [ ] Modal images/video/iframe stay within modal body and preserve usable aspect ratio.
- [ ] Member image has meaningful alt text.
- [ ] News and promotion video use `preload=metadata` and `playsInline`.
- [ ] Promotion video has no autoplay on any viewport.
- [ ] Promotion remains paused until user action.
- [ ] Native controls start real H.264 fixture and playback completes without media error.
- [ ] Long content and social URLs wrap without clipping or viewport expansion.
- [ ] Existing MOB-02 Schedule map/photo and MOB-03 Video playback behavior remain intact.

### Verification

- [ ] Before/after screenshots exist and were visually inspected.
- [ ] Chromium full viewport matrix and landscape pass.
- [ ] WebKit/Safari and Firefox compact/tablet smoke pass.
- [ ] Modal keyboard, touch, focus, scroll, replacement-content, and restore-focus scenarios pass.
- [ ] Controlled loading/empty/error/retry fixtures pass.
- [ ] TypeScript, targeted ESLint, Sass generation, and production build exit `0`.
- [ ] Full lint contains only two documented QA-03 errors and zero warnings/new diagnostics.
- [ ] Repeated Sass generation produces identical hashes.
- [ ] Bundle scan finds no development URLs/mock markers.
- [ ] `git diff --check` exits `0`.
- [ ] Secret scan finds no credentials/secrets.
- [ ] Full diff contains no unrelated task changes.
- [ ] Every validation record includes command/scenario, result, exit code where applicable, and proven criterion.
- [ ] Unperformed production/backend/full-release checks are explicitly named with valid scope reason.

### Evidence and state

- [ ] `docs/implementation/evidence/MOB-04.md` contains all required evidence sections and per-criterion verdicts.
- [ ] MOB-04 becomes `done` only after all prior boxes pass.
- [ ] `09_STATE.md` contains actual checks, confirmed facts, active `M3`, and next task `MOB-05`.
- [ ] `00_INDEX.md` links MOB-04 evidence.
- [ ] No other implementation document changes unless it became factually false.
- [ ] Final `git status --short` and full diff were reviewed.
- [ ] No commit exists.

## Expected Final Report

Report briefly:

1. MOB-04 requirement.
2. Implemented behavior.
3. Changed files.
4. Executed checks with results.
5. Unexecuted checks with reasons.
6. Acceptance verdict.
7. Backlog status.
8. Evidence path.
9. Out-of-scope findings and blocking status.
10. Next task `MOB-05`, explicitly not started.

