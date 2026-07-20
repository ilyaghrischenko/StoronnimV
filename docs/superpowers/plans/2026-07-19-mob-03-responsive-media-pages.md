# MOB-03 — Responsive Music, Group и Video

> **Для goal mode:** выполнять план последовательно до полного Definition of Done или доказанного blocker. Не начинать `MOB-04`, не создавать commit, не переключать branch. При необходимости execution skill использовать `superpowers:executing-plans`.

**Goal:** сделать Music, Group, Video categories и Video list usable на `320`, `375`, `768`, `1024`, сохранив desktop `1440`.

**Architecture:** хирургические responsive SCSS-правила плюс минимальные TSX-изменения для page scoping, Swiper, keyboard actions и mobile video playback. Новые responsive components, providers, API contracts и Video categories не создаются и не меняются.

**Tech Stack:** React 18, TypeScript 5.6, Vite 6, Swiper 11, React Bootstrap, Sass 1.79.6, Playwright CLI, local mock HTTP API.

## Global Constraints

- Выполнять только `MOB-03`; следующую backlog-задачу не начинать.
- Не менять providers, contexts, API/model contracts, backend, DB/Blob, migrations, dependencies или production resources.
- Не создавать commit, не переключать branch, не выполнять `git reset`, `git clean`, `git stash`, `git merge` или `git rebase`.
- Сохранять desktop baseline из runtime `style.css`; mobile может использовать упрощённый layout без SVG-frame.
- Canonical styles: `src/styles/style.scss` и импортируемые SCSS partials; `style.css`/`style.css.map` только генерировать.
- Любые существующие изменения пользователя сохранить; при небезопасном конфликте остановиться с доказанным blocker.
- Статус `done` ставить только после полного Definition of Done.

---

## Кратко

**Текущие факты:**

- `MOB-03` существует, статус `planned`.
- Единственная зависимость `MOB-01` имеет статус `done`.
- Активен milestone `M3`.
- Worktree clean на момент подготовки плана.
- `npm run build`: exit `0`.
- `npm run lint`: exit `1`, существующий baseline `2 errors/2 warnings`.
- Следующая задача после успешной приёмки: `MOB-04`. Только записать её в state, не начинать.
- Субагенты не использовать: изменения пересекаются через единый SCSS/CSS artifact.

**Выбранный подход:** хирургические responsive SCSS-правила плюс минимальные TSX-изменения. Альтернатива с новыми responsive components/layout architecture отклонена: увеличивает scope, меняет архитектуру, не нужна критериям.

## Scope и интерфейсы

В scope:

- Music platform cards и Spotify embed.
- Group background/description, длинный текст, members Swiper, открытие member details.
- Video category grid, video list/cards, pagination, playback, back/category actions.
- Compact page flow, media sizing, touch/keyboard actions.
- Генерация canonical `style.css`/`style.css.map`.
- Evidence и state-документы MOB-03.

Вне scope:

- Providers, contexts, API/model contracts, Video categories.
- Backend, DB, Blob, migrations, production resources.
- Полная responsive modal mechanics: `MOB-04`.
- Admin forms/buttons/table layout: `MOB-05`.
- Общий accessibility/reduced-motion audit: `MOB-06`.
- Старый full ESLint baseline: `QA-03`.
- Новые dependencies, redesign desktop, новая архитектура.

Публичные API и TypeScript models не меняются. Внутренние UI-контракты:

- Music, Group и Video route containers получают отдельные page modifier classes.
- `ShortMemberItem` становится native `button`.
- Members Swiper: `1` slide по умолчанию, `2` от `640px`, `3` от `1024px`; `loop` только при числе участников больше `3`.
- Video pagination включает существующий `compactOnMobile`.
- `<video>` использует `preload="metadata"` и `playsInline`.
- Spotify iframe получает доступное имя.

## План реализации

### 1. Preflight и исходное evidence

- [ ] Повторно прочитать обязательные документы и применимый `frontend/AGENTS.md`.
- [ ] Выполнить `git status --short`; записать все существующие пользовательские изменения. Не трогать несвязанные файлы.
- [ ] Проверить строку `MOB-03`, статус `MOB-01`, текущие branch/HEAD.
- [ ] Выполнить исходные `git diff --check`, `npm run build`, `npm run lint`.
- [ ] Если `MOB-01` перестала быть `done`, остановиться без реализации.
- [ ] Если пользовательские изменения конфликтуют с обязательными файлами и безопасное совмещение невозможно, зафиксировать blocker.
- [ ] Создать disposable каталог `/tmp/storonnimv-mob03`; repo-tracked test harness не добавлять.
- [ ] Поднять localhost mock API с endpoints `/api/group-socials`, `/api/music`, `/api/group`, `/api/group/member/{id}`, `/api/videos/page/{category}/{page}?pageSize=2`, `/api/admin/isAdmin`.
- [ ] Fixtures: три music links, длинное Group description, `1/2/3/5` members, длинные member names/roles, все три Video categories, две страницы video items, длинные titles.
- [ ] Создать реальный односекундный H.264 MP4 вне Git:

```bash
ffmpeg -hide_banner -loglevel error -f lavfi -i color=c=black:s=320x180:d=1 \
  -c:v libx264 -pix_fmt yuv420p -movflags +faststart -y \
  /tmp/storonnimv-mob03/video.mp4
```

- [ ] Снять before screenshots для `/music`, `/group`, `/video/sections`, `/video/section?videoType=Performance` на `320×800`, `375×812`, `768×1024`, `1024×768`, `1440×900`.
- [ ] Записать исходные clipping/overflow/interaction failures как browser RED.

### 2. Music

**Files:**

- Modify: `frontend/storonnimv.client/src/components/pages/Music.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/music/MusicPlatformItem.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/music/SpotifyContainer.tsx`
- Modify: `frontend/storonnimv.client/src/styles/pages/Music.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/music/MusicPlatformItem.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/music/MusicPlatforms.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/music/SpotifyContainer.scss`

**Changes:**

- [ ] Добавить route-container class `music-page-shell`.
- [ ] При `<=1024px` перевести page в document flow: auto-height, visible vertical overflow, `1rem` padding.
- [ ] При `<=768px` использовать `0.75rem` padding.
- [ ] `.music-page`: column layout, `width:100%`, `min-width:0`, auto-height, `1.5rem` gap.
- [ ] `.music-platforms-container`: убрать compact fixed/min heights, дать `width:100%`, auto-height, `1rem` gap.
- [ ] `.music-platform-item`: compact `width:100%`, `height:clamp(8rem, 30vw, 12rem)`, сохранить `background-size:cover`.
- [ ] `.spotify-container`: compact `width:100%`, `min-width:0`, `height:clamp(22rem, 65dvh, 35rem)`.
- [ ] Spotify iframe: `display:block`, `width/height:100%`, `title="Spotify — Стороннім В"`.
- [ ] Music link оставить safe external anchor с `target="_blank"`/`noopener noreferrer`; добавить доступное имя, не менять URL validation/provider.
- [ ] Desktop rules `>1024px` оставить визуально неизменными.

**Narrow validation:**

- [ ] Targeted ESLint Music TSX: exit `0`.
- [ ] Browser на `320/375/768/1024`: cards и iframe внутри viewport; вертикальный scroll достигает Spotify.
- [ ] Keyboard Enter и touch открывают fixture music URL в новой вкладке.
- [ ] Invalid URL остаётся без navigable `href`.

### 3. Group

**Files:**

- Modify: `frontend/storonnimv.client/src/components/pages/Group.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/group/groupPageComponents/ShortMembers.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/group/groupPageComponents/ShortMemberItem.tsx`
- Modify: `frontend/storonnimv.client/src/styles/elements/group/Description.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/group/GroupDescription.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/group/GroupMembers.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/group/ShortMemberItem.scss`

**Changes:**

- [ ] Добавить route-container class `group-page`.
- [ ] Compact page/container: auto-height, visible overflow, responsive padding, background cover без fixed-height clipping.
- [ ] Заголовок группы при `<=1024px`: `font-size:clamp(2.25rem, 10vw, 6.25rem)`.
- [ ] Длинное описание на compact: `max-height:none`, `overflow:visible`, полный перенос текста, отключённый transform scrolling. Desktop auto-scroll сохранить.
- [ ] Ограничить существующие `.swiper` styles областью `.short-members-container`, чтобы не влиять на другие Swiper.
- [ ] Swiper config: default `slidesPerView={1}`, `spaceBetween={12}`; `640`: `2/16`; `1024`: `3/20`.
- [ ] Navigation, Autoplay, swipe и speed сохранить.
- [ ] Использовать `loop={members.length > 3}` для отсутствия loop warnings.
- [ ] `ShortMemberItem` заменить с click-only `div` на `button type="button"`.
- [ ] Добавить доступное имя, `focus-visible`, сброс native border/background/padding.
- [ ] Compact member card: `width:100%`, `max-width:22rem`, исходный `10/9` aspect ratio, без нижнего процентного margin.
- [ ] Имена и роли: responsive font, normal wrapping, `overflow-wrap:anywhere`.
- [ ] Member modal layout не менять; проверять только trigger и успешное открытие.

**Narrow validation:**

- [ ] Targeted ESLint изменённых Group TSX: exit `0`.
- [ ] Fixtures `1/2/3/5`: loop `false/false/false/true`, console loop warnings `0`.
- [ ] Видимые slides: `1/1/2/3` на `320/375/768/1024`.
- [ ] Navigation arrow и touch gesture меняют active slide.
- [ ] Enter/Space и touch открывают правильного member.
- [ ] Длинное описание полностью доступно вертикальным scroll, без clipping/horizontal overflow.
- [ ] `1440` сохраняет три карточки и desktop description behavior.

### 4. Video categories и list

**Files:**

- Modify: `frontend/storonnimv.client/src/components/pages/Video.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/video/VideoList.tsx`
- Modify: `frontend/storonnimv.client/src/components/elements/video/VideoListItem.tsx`
- Modify: `frontend/storonnimv.client/src/styles/elements/video/VideoSections.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/video/VideoList.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/video/VideoListItem.scss`
- Modify: `frontend/storonnimv.client/src/styles/elements/shared/PreloaderTile.scss`

**Changes:**

- [ ] Добавить route-container class `video-page`.
- [ ] Compact Video page перевести в auto-height document flow.
- [ ] Categories при `<=1024px`: grid `repeat(auto-fit, minmax(min(100%, 17.5rem), 1fr))`, `1rem` gap.
- [ ] Category buttons: `width:100%`, auto-height, без `400px` minimum.
- [ ] Category images: `width:100%`, `aspect-ratio:3/2`, `height:auto`, `object-fit:cover`.
- [ ] Добавить видимый `focus-visible`; убрать hover scale на compact.
- [ ] Ожидаемые category columns: `1/1/2/3` на `320/375/768/1024`.
- [ ] Video list при `<=1024px`: auto-height; grid `repeat(auto-fit, minmax(min(100%, 26rem), 1fr))`.
- [ ] Ожидаемые video columns: `1/1/1/2`.
- [ ] Back button перевести из absolute в normal flow, `min-height/min-width:44px`.
- [ ] Video cards убрать compact `min-height:400px`; titles разрешить переносить полностью.
- [ ] Video: `width:100%`, `aspect-ratio:16/9`, без viewport-breaking max dimensions.
- [ ] Video preloader повторяет responsive card bounds.
- [ ] Передать `compactOnMobile` в `PaginationSection`.
- [ ] `<video>`: `preload="metadata"`, `playsInline`, доступное имя из title; controls сохранить.
- [ ] Admin video controls не переделывать: их responsive layout относится к `MOB-05`.
- [ ] Category enum, query names и providers не менять.

**Narrow validation:**

- [ ] Targeted ESLint Video TSX: exit `0`.
- [ ] Touch/keyboard для каждой category ведёт на точный `videoType`.
- [ ] Back button возвращает `/video/sections`.
- [ ] Pagination помещается на `320px`, меняет page и сохраняет category.
- [ ] Реальный fixture MP4 загружается без media error; user-triggered playback начинает воспроизведение и достигает конца.
- [ ] До взаимодействия `autoplay=false`, `preload=metadata`, `playsInline=true`.
- [ ] Long title полностью доступен, video bounds не выходят за viewport.

### 5. SCSS artifacts

- [ ] Менять только canonical `src/styles/style.scss` partials.
- [ ] Не редактировать legacy per-partial `.css`/`.css.map`.
- [ ] Выполнить `npm run styles:build`.
- [ ] Проверить Sass: `npm exec sass -- --version`, ожидается `1.79.6`.
- [ ] Записать SHA-256 `style.css` и `style.css.map`.
- [ ] Повторить generation; hashes должны совпасть.
- [ ] В diff оставить generated `src/styles/style.css` и `style.css.map`.
- [ ] Проверить, что desktop selectors не получили случайных global overrides.

### 6. Полная browser matrix

After screenshots сохранить вне Git:

- `/tmp/storonnimv-mob03/after/music-*`;
- `/tmp/storonnimv-mob03/after/group-*`;
- `/tmp/storonnimv-mob03/after/video-sections-*`;
- `/tmp/storonnimv-mob03/after/video-list-*`.

Обязательные сценарии:

- [ ] Все четыре routes на пяти widths.
- [ ] Landscape `812×375` для Music, Group и обеих Video pages.
- [ ] Для каждого viewport: `documentElement.scrollWidth === clientWidth`.
- [ ] Relevant media/card rectangles не имеют отрицательного left и не выходят правее viewport.
- [ ] Нижний content достижим вертикальным scroll.
- [ ] Console не содержит неожиданных warnings/errors.
- [ ] Chromium: полная width/interaction matrix.
- [ ] WebKit/Safari и Firefox: smoke на `375` и `1024`.
- [ ] Записать фактические browser versions.
- [ ] `1440` before/after сравнить визуально: desktop frame, row/columns, typography и control placement без блокирующих изменений.
- [ ] Production/staging Spotify availability не считать локальным gate. Проверить iframe contract, viewport bounds и URL; video playback доказать локальным реальным MP4.
- [ ] Full Edge/browser release matrix оставить `MOB-06`/`QA-05`; не приписывать её MOB-03.

### 7. Static и repository validation

Из `frontend/storonnimv.client`:

- [ ] Targeted ESLint всех изменённых TSX: exit `0`, diagnostics `0`.
- [ ] `npm run styles:build`: exit `0`.
- [ ] `npm exec sass -- --version`: exit `0`, `1.79.6`.
- [ ] Deterministic CSS regeneration: одинаковые hashes.
- [ ] `VITE_API_URL=https://api.example.test/api npm run build`: exit `0`.
- [ ] Scan `dist` на `localhost:44315`, `127.0.0.1`, mock marker: exit `1`, совпадений нет.
- [ ] `npm run lint` выполнить обязательно.
- [ ] Допустим только доказанный исходный baseline `2 errors/2 warnings`.
- [ ] Errors `FrameLayout.tsx`/`Header.tsx`, warnings `GroupDescription.tsx`/`MemberModal.tsx` записать как существующие QA-03 findings.
- [ ] Любой новый diagnostic или ухудшение count блокирует `done`.
- [ ] `git diff --check`: exit `0`.
- [ ] Secret scan изменённых source/generated/docs files: совпадений нет.
- [ ] Просмотреть полный diff; каждый changed line относится к MOB-03.
- [ ] Backend tests не запускать: backend/API/contracts не меняются. Причину записать.
- [ ] Production DB/Blob и external mutable resources не использовать.

### 8. Evidence и state

Создать `docs/implementation/evidence/MOB-03.md` со следующими разделами:

- цель, scope, exclusions;
- dependency/preflight;
- исходный branch/HEAD/status;
- исходные failures и before screenshots;
- затронутые файлы;
- решения по breakpoints, long description, semantic member action, video preload;
- Context7 confirmation для Swiper v11 breakpoint syntax и loop limitation;
- все команды, exit codes, краткий доказанный результат;
- browser versions, fixtures, viewport matrix, artifact paths;
- невыполненные проверки и причины;
- out-of-scope findings;
- итог каждого acceptance criterion;
- подтверждение отсутствия production access, branch switch и commit.

Только после полного Definition of Done:

- [ ] `docs/implementation/04_BACKLOG.md`: `MOB-03` получает `done`.
- [ ] `docs/implementation/09_STATE.md`: добавить MOB-03 в выполненные, описать фактические проверки, оставить active milestone `M3`, следующая задача `MOB-04`, добавить подтверждённые responsive/media facts.
- [ ] `docs/implementation/00_INDEX.md`: добавить MOB-03 в state и ссылку на evidence.
- [ ] Другие документы не менять без фактической неверности.
- [ ] `MOB-04` не реализовывать.

## Definition of Done

### Scope и hygiene

- [ ] `MOB-03` существует; `MOB-01` подтверждена как `done`.
- [ ] Начальный user diff сохранён; несвязанные изменения не исправлены и не перезаписаны.
- [ ] Изменены только Music, Group, Video, canonical SCSS artifacts и обязательные implementation docs.
- [ ] Providers, categories, API/models, backend, DB/Blob и dependencies не изменены.
- [ ] Branch не переключалась; commit не создан; начальный и финальный HEAD совпадают.
- [ ] `MOB-04` и иные backlog-задачи не начаты.

### Music acceptance

- [ ] `/music` не имеет horizontal overflow на `320/375/768/1024/1440`.
- [ ] Platform cards полностью помещаются, сохраняют background media и имеют usable touch area.
- [ ] Valid links доступны touch/keyboard, открывают ожидаемый URL в новой вкладке с safe `rel`.
- [ ] Invalid link не становится navigable.
- [ ] Spotify iframe имеет доступное имя, не шире viewport, не clipped, достижим scroll.
- [ ] Desktop `1440` сохраняет исходную композицию без блокирующего visual regression.

### Group acceptance

- [ ] Group photo/background не растягивает и не обрезает page container за viewport.
- [ ] Длинное описание полностью читаемо на compact widths; текст не скрыт fixed max-height.
- [ ] Compact heading не выходит за viewport.
- [ ] Swiper показывает `1/1/2/3` slides на `320/375/768/1024`.
- [ ] Fixtures `1/2/3/5` не дают Swiper loop warning; loop включён только для `5`.
- [ ] Arrow navigation и touch swipe работают.
- [ ] Каждая member card является native keyboard/touch action с видимым focus.
- [ ] Enter/Space/touch открывают правильный member detail.
- [ ] Modal responsive layout не менялся; это явно оставлено `MOB-04`.
- [ ] Desktop `1440` сохраняет исходный three-card layout и animation behavior.

### Video acceptance

- [ ] Все три category cards видимы, не clipped, media сохраняет `3/2`.
- [ ] Category actions доступны touch/keyboard и передают неизменённые `Performance`, `Backstage`, `Repetition`.
- [ ] Category grid имеет `1/1/2/3` columns на `320/375/768/1024`.
- [ ] Video list имеет `1/1/1/2` columns на тех же widths.
- [ ] Video cards, long titles, preloaders, pagination и back button не выходят за viewport.
- [ ] Compact pagination usable на `320px`.
- [ ] Back/category/pagination transitions работают без route error.
- [ ] Реальный MP4 воспроизводится через native controls без media error.
- [ ] Video использует metadata preload и inline mobile playback; тяжёлое autoplay отсутствует.
- [ ] Desktop `1440` сохраняет две video columns и текущую visual hierarchy.

### Проверки

- [ ] Before/after screenshots созданы для четырёх routes и пяти widths.
- [ ] Landscape `812×375` пройден.
- [ ] Chromium full matrix green.
- [ ] WebKit/Safari и Firefox smoke green на `375/1024`.
- [ ] Happy-path browser console не содержит новых warnings/errors.
- [ ] Targeted ESLint: exit `0`.
- [ ] Sass generation: exit `0`; version `1.79.6`.
- [ ] Повторная CSS generation deterministic.
- [ ] Production build: exit `0`.
- [ ] Bundle scan не нашёл localhost/mock endpoints.
- [ ] Full ESLint запущен; baseline не ухудшен. Новые diagnostics отсутствуют.
- [ ] `git diff --check`: exit `0`.
- [ ] Secret scan: совпадений нет.
- [ ] Полный diff просмотрен; несвязанных изменений нет.

### Evidence и статус

- [ ] `evidence/MOB-03.md` содержит цель, исходное состояние, files, decisions, changes, команды, exit codes, browser evidence, omissions, findings и criterion matrix.
- [ ] Каждый критерий backlog имеет отдельный `pass` либо доказанный blocker.
- [ ] Невыполненные проверки не скрыты.
- [ ] При полном pass backlog status установлен `done`.
- [ ] `09_STATE.md` отражает MOB-03, реальные проверки, active `M3`, следующую `MOB-04`.
- [ ] `00_INDEX.md` ссылается на evidence.
- [ ] При любом обязательном failure статус остаётся `planned`, MOB-03 остаётся текущей, evidence содержит blocker.
- [ ] Финальный `git status --short`, полный diff, backlog, state и evidence перепроверены.
- [ ] После итогового отчёта работа остановлена. Следующая задача не начата.

## Blocker policy

`done` запрещён, если остаётся clipping, media overflow, неработающий category/member action, failed local playback, новый lint/build diagnostic, nondeterministic CSS, отсутствующее обязательное evidence либо небезопасный конфликт с user changes.

Допустимый известный non-blocker: исходный full ESLint baseline `2 errors/2 warnings`, только если count и locations не ухудшились, изменённые TSX проходят targeted ESLint, findings записаны как scope `QA-03`.

Минимальное обращение к владельцу требуется только при конфликтующих user changes, необходимости менять providers/categories/architecture, недоступности обязательного browser/media toolchain после безопасных попыток либо требовании production access.
