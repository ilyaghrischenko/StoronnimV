# Мобильная готовность

## Итог

**Текущий frontend не готов к мобильным устройствам.** Viewport meta присутствует и в отдельных SCSS-файлах есть media queries, но runtime CSS и базовый layout принудительно сохраняют desktop minimum width `1100px`. Это системная блокировка, а не набор косметических проблем.

Аудит статический; browser screenshots не делались, поскольку приложение не запускалось с реальным API. Вывод подтверждён layout/CSS, но точный вид clipping требует визуального запуска.

## Существующая responsive-стратегия

- `index.html`: `width=device-width, initial-scale=1.0`.
- `ResolutionWrapper`: breakpoint `max-width: 980px`, который должен показывать `MobileInDeveloping`.
- `Header.scss`, `Home.scss`: отдельные rules при `max-width: 768px`.
- `Description.scss`: rule при `max-width: 1200px`.

Стратегия фактически не работает целостно: wrapper закомментирован в [App.tsx](../../frontend/storonnimv.client/src/App.tsx); `style.css` не включает актуальные mobile header selectors; базовые [App.scss](../../frontend/storonnimv.client/src/styles/shared/App.scss) и [FrameLayout.scss](../../frontend/storonnimv.client/src/styles/elements/shared/FrameLayout.scss) задают `min-width: 1100px`.

## Системные проблемы

| Приоритет | Проблема | Влияние | Доказательство | Уверенность |
|---|---|---|---|---|
| P0 | `min-width: 1100px` на app/frame | Все страницы шире mobile viewport | `styles/shared/App.scss`, `styles/elements/shared/FrameLayout.scss`, runtime `style.css` | Подтверждено |
| P0 | Desktop frame `86% / 14%` | Контент и sidebar не перестраиваются | `FrameLayout.scss` — `.inFrame` | Подтверждено |
| P0 | Mobile wrapper отключён | Даже заглушка «в разработке» не защищает narrow screen | `App.tsx`, `ResolutionWrapper.tsx` | Подтверждено |
| P0 | Burger JSX без актуального runtime CSS | Mobile navigation не имеет подтверждённого оформления | `Header.tsx`, `Header.scss`, `style.css` | Подтверждено |
| P1 | Overflow скрывается | Пользователь может не добраться до обрезанного content | `Page.scss`, `Header.scss`, runtime CSS | Подтверждено |
| P2 | Click/hover-only interactions | Touch/keyboard users теряют content/actions | list/card SCSS и components | Подтверждено |
| P2 | Modal без focus/dialog semantics | Слабая accessibility на mobile/keyboard | `ModalWindow.tsx` | Подтверждено |
| P2 | Нет reduced-motion strategy; media preload | Motion/bandwidth нагрузка | Swiper/video/Description components | Подтверждено |

## Статус страниц

| Страница | Статус | Приоритет | Причина | Доказательство |
|---|---|---:|---|---|
| Home `/` | не готово | P0 | desktop min-width; Swiper по 3; hover-only news; mobile selectors не соответствуют JSX | `Home.tsx`, `NewsSlider.tsx`, `Home.scss` |
| Schedule `/schedule` | не готово | P0 | nowrap/cards и hidden overflow внутри desktop frame | `SchedulesList.tsx`, `ScheduleList.scss` |
| News `/news` | не готово | P0 | три колонки без работающего breakpoint; click-only cards | `NewsList.tsx`, `NewsList.scss`, `ListItem.tsx` |
| Music `/music` | не готово | P0 | row layout, Spotify container minimum width | `Music.tsx`, `SpotifyContainer.scss` |
| Group `/group` | не готово | P0 | desktop minimum, крупный title, Swiper=3, clipped/animated description | `Group.tsx`, `ShortMembers.tsx`, `Description.tsx` |
| Video categories | не готово | P0 | section tiles имеют desktop minimum dimensions | `VideoSections.tsx`, `VideoSections.scss` |
| Video list | не готово | P0 | две колонки и крупные cards | `VideoList.tsx`, `VideoList.scss`, `VideoListItem.scss` |
| Admin login | не готово | P0 | global desktop width; frame резервирует desktop layout; API localhost | `Admin.tsx`, `AuthForm.scss`, `GlobalContext.tsx` |
| Basic admins | не готово | P0 | desktop table/layout, нет mobile overflow strategy | `BasicAdmins.tsx`, `AdminTable.scss` |
| Developers | не готово | P1 | функциональная заглушка плюс общий layout | `Developers.tsx` |
| Error | не готово | P0 | общий min-width; direct hosting rewrite не подтверждён | `Error.tsx`, `staticwebapp.config.json` |

## Компоненты будущей работы

Это не план реализации, а границы будущего исследования: глобальная геометрия frame; navigation/header/footer; grid/list breakpoints; media sizing; modals/forms/admin tables; touch targets и keyboard semantics; reduced motion/loading; синхронизация SCSS и runtime CSS. Сначала нужно решить, сохраняется ли декоративная frame-концепция на mobile или mobile получает другой layout.

## Неизвестные моменты

- фактическое clipping на конкретных iOS/Android viewport;
- реальные размеры и aspect ratios API media;
- должен ли mobile route показывать полноценный UI или временную заглушку;
- какой из `style.css`, SCSS source или deployed CSS считается дизайнерским источником истины;
- требования владельца к tablet breakpoint и mobile admin.
