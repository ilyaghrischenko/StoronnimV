# MOB-03 — Responsive Music, Group и Video

## Цель и scope

Music, Group, Video categories и Video list сделаны usable на `320`, `375`, `768`, `1024` с сохранением desktop `1440`. Изменены только route scoping, семантика member action, Swiper config, media attributes, responsive canonical SCSS, generated `style.css`/map и обязательные state-документы.

Вне scope остались providers, contexts, API/models, Video categories, backend, DB/Blob, dependencies, responsive modal mechanics (`MOB-04`) и admin layout (`MOB-05`). Production resources не использовались.

## Dependency и preflight

- `MOB-03` стартовала как `planned`; `MOB-01` подтверждена как `done`; milestone `M3` активен.
- Исходная branch: `main`; HEAD: `09f3b80e147706f436c4bc59065f1546064781ac`.
- Исходный `git status --short`: только пользовательский untracked `?? docs/superpowers/`; он сохранён и не редактировался.
- Checkout обычный, не linked worktree. План MOB-03 явно запрещает branch switch и commit, поэтому работа выполнена in-place без Git mutation.
- Исходный `git diff --check`: exit `0`.
- Исходный `npm run build`: exit `0`.
- Исходный `npm run lint`: exit `1`, ровно baseline `2 errors/2 warnings`: errors в `FrameLayout.tsx`/`Header.tsx`, warnings в `GroupDescription.tsx`/`MemberModal.tsx`.

## Browser RED и fixtures

Disposable harness: `/tmp/storonnimv-mob03`; mock API: `127.0.0.1:41812`; Vite: `127.0.0.1:41813`. Fixtures включали три Music links и invalid URL variant, длинное Group description, `1/2/3/5` members, длинные names/roles, три Video categories, две страницы Video и длинные titles. Реальный H.264/yuv420p MP4 длительностью `1s` создан через `ffmpeg` в `/tmp/storonnimv-mob03/video.mp4`; Range requests поддержаны mock API.

Before screenshots: `/tmp/storonnimv-mob03/before/{music,group,video-sections,video-list}-{320x800,375x812,768x1024,1024x768,1440x900}.png`.

Browser RED подтвердил:

- Music Spotify сжимался до `43.2px` на `320`;
- Group показывал desktop `3` slides на compact widths, скрывал длинное описание в `190px` и располагал последующие slides правее viewport;
- Video list сохранял `2` колонки по `140px` на `320`, long title обрезался;
- compact page children наследовали fixed/max-height clipping.

## Изменённые файлы и решения

### TSX

- `components/pages/{Music,Group,Video}.tsx`: route modifier classes.
- `elements/music/{MusicPlatformItem,SpotifyContainer}.tsx`: accessible safe link name и iframe title.
- `elements/group/groupPageComponents/{ShortMembers,ShortMemberItem}.tsx`: breakpoints, conditional loop, stable keys, native `button`.
- `elements/video/{VideoList,VideoListItem}.tsx`: compact pagination, metadata preload, inline playback, accessible video name.

### Canonical styles и artifacts

- `styles/pages/Music.scss`.
- `styles/elements/music/{MusicPlatformItem,MusicPlatforms,SpotifyContainer}.scss`.
- `styles/elements/group/{Description,GroupDescription,GroupMembers,ShortMemberItem}.scss`.
- `styles/elements/video/{VideoSections,VideoList,VideoListItem}.scss`.
- `styles/elements/shared/PreloaderTile.scss`.
- Generated: `styles/style.css`, `styles/style.css.map`.

Compact pages используют auto-height document flow и border-box page padding. Music cards/Spotify занимают доступную ширину. Group description снимает `max-height`, `overflow` и WAAPI transform через compact `transform:none !important`; desktop animation не изменена. Group Swiper styles ограничены `.short-members-container`; config: `1/12`, `640:2/16`, `1024:3/20`, `loop={members.length > 3}`. Member card стала native keyboard/touch action с `focus-visible`.

Video categories используют auto-fit grid с minimum `17.5rem`; list — `26rem`. Compact back button находится в normal flow, pagination включает `compactOnMobile`, cards/media/preloaders ограничены viewport. Video использует `preload="metadata"`, `playsInline`, controls и accessible label; autoplay отсутствует.

## Context7 confirmation

Context7 resolve выбрал Swiper `/nolimits4web/swiper/v11.2.10` и official `/websites/swiperjs`. Документация подтвердила numeric `breakpoints` object с `slidesPerView`/`spaceBetween`. Swiper v11 loop source предупреждает при недостаточном числе slides относительно `slidesPerView`/looped slides; поэтому loop включён только для `members.length > 3`. Runtime fixtures `1/2/3/5` и `5@1024` дали `0` loop warnings.

## Browser GREEN

Browsers:

- Headless Chrome `151.0.7922.10`;
- Firefox `152.0`;
- WebKit/Safari `26.5`.

After screenshots: `/tmp/storonnimv-mob03/after/`. Chromium содержит четыре routes на пяти обязательных viewports плюс landscape `812x375`; Firefox/WebKit содержат smoke screenshots четырёх routes на `375` и `1024`.

Результаты:

- Chromium: `documentElement.scrollWidth === clientWidth` для всех 24 route/viewport combinations; media/cards active bounds внутри viewport; нижний content достигнут scroll.
- Category columns: `1/1/2/3`; Video columns: `1/1/1/2`; Swiper params: `1/1/2/3` на `320/375/768/1024`.
- Group long description: compact `max-height:none`, `overflow:visible`, полный document scroll; desktop `1440` сохраняет 3-card layout и scrolling behavior.
- `1/2/3/5` members: loop `false/false/false/true`; warnings `0`; arrow `realIndex 0→1`; real CDP touch swipe `0→1`.
- Enter, Space и touch открыли правильного member; modal layout не менялся.
- Music Enter/click/touch открыли exact fixture URLs в новой вкладке; valid links имели `_blank` и `noopener noreferrer`; invalid URL имел `href=null`, `aria-disabled=true`. Spotify iframe title и compact width подтверждены.
- Category Enter и touch дали exact `Performance`, `Backstage`, `Repetition`; Back вернул `/video/sections`; pagination дала `2 / 2`, сохранив `videoType=Backstage`.
- MP4 до действия: `autoplay=false`, `preload=metadata`, `playsInline=true`, media error `null`. Native control click начал playback (`paused=false`) и достиг `ended=true`, `currentTime=duration=1`, error `null`.
- Firefox/WebKit smoke: `scrollWidth === clientWidth` на всех 16 combinations.
- Chromium/WebKit happy paths не дали новых app warnings/errors. Firefox сообщил только browser feature-policy/Spotify Storage Access warnings. Spotify CDN/network availability не считалась local gate согласно плану.
- Визуальный before/after `1440` сохранил desktop frame, Music row, Group 3 cards, Video category row, Video 2 columns, typography/control hierarchy без blocking regression.

## Команды и static validation

| Команда | Exit | Результат |
|---|---:|---|
| targeted ESLint 9 изменённых TSX | 0 | diagnostics `0` |
| `npm exec sass -- --version` | 0 | `1.79.6 compiled with dart2js 3.5.3` |
| `npm run styles:build` | 0 | canonical CSS/map generated |
| повторная Sass generation | 0 | hashes совпали |
| `VITE_API_URL=https://api.example.test/api npm run build` | 0 | TypeScript/Vite production build green |
| bundle scan `localhost:44315`, `127.0.0.1`, mock marker | 1 | совпадений нет |
| `npm run lint` | 1 | неизменный baseline `2 errors/2 warnings`; новых diagnostics нет |
| `git diff --check` | 0 | whitespace errors нет |
| secret scan changed source/generated/docs | 1 | совпадений нет |

Deterministic hashes:

- `style.css`: `cb2a87ce38107ff061df8741fca17aa8f9876716db53f3abcff6067b2ea79f68`;
- `style.css.map`: `18d7ba1518a66b09c0c4ac920d80d1c89d91aae77e5071b48b1fe7bfa93c5321`.

Backend tests не запускались: backend/API/contracts не менялись. Production DB/Blob, credentials и mutable external resources не использовались. Full Edge/release browser matrix остаётся `MOB-06`/`QA-05`.

## Acceptance criteria

- **pass — scope/hygiene:** только MOB-03 source/styles/artifacts/docs; user diff сохранён; branch/HEAD не менялись; commit отсутствует; MOB-04 не начата.
- **pass — Music:** no root overflow, usable cards, safe keyboard/touch links, invalid non-navigable URL, named responsive Spotify, desktop baseline.
- **pass — Group:** full compact description/background flow, responsive heading, exact slides/loop behavior, arrow/touch, native Enter/Space/touch member action, desktop 3 cards/animation.
- **pass — Video:** exact category/list columns, responsive `3/2` and `16/9` media, normal-flow Back, compact pagination, full titles, exact transitions, successful real MP4 playback, metadata/inline/no-autoplay contract, desktop 2 columns.
- **pass — validation:** before/after/landscape, Chromium matrix, Firefox/WebKit smoke, targeted lint, deterministic Sass, production build, bundle/diff/secret scans, unchanged full ESLint baseline.
- **pass — evidence/state:** этот документ, backlog, state и index синхронизированы; active milestone `M3`; следующая задача `MOB-04`, но не начата.

## Ограничения и out-of-scope findings

- Full ESLint baseline остаётся `QA-03`: errors `FrameLayout.tsx`/`Header.tsx`, warnings `GroupDescription.tsx`/`MemberModal.tsx`.
- Полная mobile modal mechanics остаётся `MOB-04`; проверялись только triggers и правильный member content.
- Admin media controls остаются `MOB-05`.
- Spotify external CDN/feature-policy behavior browser-dependent и не блокирует локальный iframe contract.
- Commit, branch switch, merge, rebase, stash, reset и production access не выполнялись.
