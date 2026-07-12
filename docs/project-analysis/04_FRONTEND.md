# Frontend

## Стек и entry points

Frontend написан на TypeScript/TSX. Основные версии: React/React DOM 18.3.1, React Router DOM 7.1.3, Vite 6, TypeScript 5.6, Axios 1.7, Bootstrap/React Bootstrap, MUI 7, Swiper 11, React Icons и `react-helmet-async` — [package.json](../../frontend/storonnimv.client/package.json).

Цепочка запуска: [index.html](../../frontend/storonnimv.client/index.html) → [main.tsx](../../frontend/storonnimv.client/src/main.tsx) (`GlobalContextProvider`, `HelmetProvider`, `StrictMode`) → [App.tsx](../../frontend/storonnimv.client/src/App.tsx) (`BrowserRouter`, frame, modal host) → [Page.tsx](../../frontend/storonnimv.client/src/components/pages/shared/Page.tsx) (routes).

`index.html` содержит корректный viewport meta. Он напрямую подключает `src/styles/style.css`; SCSS не участвует в Vite pipeline, потому что `sass` отсутствует и `.scss` не импортируется из TSX.

## Структура

- `components/pages` — route-level screens;
- `components/elements/<feature>` — UI и admin forms по областям;
- `components/contexts` — feature state/data functions;
- `components/contexts/shared/GlobalContext.tsx` — HTTP, modal/loading/auth shared state;
- `models` — TypeScript representations response/request data;
- `styles` — SCSS source, compiled CSS и maps одновременно;
- `assets`, `public` — media, SEO/PWA assets;
- `dist` — закоммиченный build artifact, который нельзя считать source of truth.

## Маршруты и страницы

| Route | Page | Назначение |
|---|---|---|
| `/` | `Home` | ближайшая афиша, 6 новостей, promotion video |
| `/schedule` | `Schedule` | список афиш, pagination, detail/map modal |
| `/news` | `News` | список новостей, pagination, detail modal |
| `/music` | `Music` | music platforms и Spotify embed |
| `/group` | `Group` | описание, участники, member/social modal |
| `/video/sections` | `Video` + `VideoSections` | выбор одной из трёх категорий |
| `/video/section?videoType=...` | `Video` + `VideoList` | пагинированные видео категории |
| `/admin` | `Admin` + `AuthForm` | login |
| `/admin/basic-admins` | protected `AdminContainer` | SuperAdmin CRUD |
| `/developers` | `Developers` | placeholder `<p>hello</p>` |
| `/error` | `Error` | status/message из query string |
| `*` | redirect | client-side 404 |

Azure static routes перечислены вручную в [staticwebapp.config.json](../../frontend/storonnimv.client/staticwebapp.config.json). Общего `navigationFallback` нет, `/error` отсутствует: direct-link behavior для error/unknown URL требует deployment-проверки.

## Layout и ключевые компоненты

`FrameLayout` накладывает SVG frame и делит desktop viewport на content/nav. `HeaderWithFooter` помещает nav и social footer в правую колонку. `ModalWindow` — единый global modal host. Shared `NoData`, `PreloaderTile`, `PageLoading`, `ModalLoading` и `PaginationSection` обслуживают повторяющиеся состояния.

В JSX `Header` уже есть burger/drawer, но реально подключённый `style.css` не содержит соответствующих актуальных selectors из `Header.scss`; source JSX/SCSS/compiled CSS рассинхронизированы — [Header.tsx](../../frontend/storonnimv.client/src/components/elements/shared/Header.tsx), [style.css](../../frontend/storonnimv.client/src/styles/style.css), [Header.scss](../../frontend/storonnimv.client/src/styles/elements/shared/Header.scss).

## Управление состоянием

Используется Context API:

- `GlobalContext`: Axios wrapper, base URL, global page/modal loading, modal content/title, admin flag, validation errors;
- `HomeContext`, `NewsContext`, `ScheduleContext`, `MusicContext`, `GroupContext`, `VideoContext`, `AdminContext`: feature data/functions;
- `sessionStorage`: pagination metadata, current video type, active nav и строка admin role.

Нет query cache, request cancellation, error boundary или унифицированной state machine. Общий boolean `pageLoading` разделён независимыми запросами (включая Footer), поэтому возможен loading race. `AdminContext.basicAdmins` начинается с пустого объекта; `deleteAdmin` вычисляет `filter`, но не вызывает setter — [AdminContext.tsx](../../frontend/storonnimv.client/src/components/contexts/AdminContext.tsx).

## API-клиент и контракты

`sendRequest` вызывает Axios с `withCredentials: true`, превращает HTTP non-2xx в обычный response и отдельно alert-ит 429. Network failures бросаются. Главный P0: `serverRoute` hardcoded как `https://localhost:44315/api`, а `VITE_API_URL` не читается — [GlobalContext.tsx](../../frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx).

Публичные GET routes статически совпадают с backend. Существенный contract mismatch: ряд admin edit/create forms передаёт `FormData` и вручную ставит `Content-Type: application/json`, тогда как backend ожидает `[FromBody]` JSON. Это затрагивает edit news/schedule/video/group/member/social/group-social и create social; photo/create multipart endpoints в основном согласованы — [`components/elements/*/forms`](../../frontend/storonnimv.client/src/components/elements), [AdminController.cs](../../backend/StoronnimV.Server/StoronnimV.Api/Controllers/AdminController.cs).

## Формы и admin

CRUD forms встроены в feature pages и показываются по `isAdmin`. Login получает role от server, сохраняет её в `sessionStorage`; JWT ожидается в HttpOnly cookie. `ProtectedRoute` запускает `fetchIsAdmin`, но не ждёт его и принимает решение только по `sessionStorage.role`; TODO прямо отмечает незавершённость — [ProtectedRoute.tsx](../../frontend/storonnimv.client/src/components/elements/admin/ProtectedRoute.tsx).

Validation UI есть для части account forms, но большинство content forms используют `alert`/console и page reload. Нет общей модели field errors/retry.

## Loading, error, empty

- skeleton/loading: News, Schedule, Music, Video, Group;
- modal loading: detail/admin flows;
- `NoData`: основные списки;
- query-driven `Error` page.

Недостатки: contexts часто читают `response.data` без проверки status; error и реальный empty визуально смешиваются; Home не имеет целостного loading state; пользовательского retry нет; Error Boundary отсутствует.

## Стили и media

Тёмная/жёлтая тема, Marmelad, BEM-подобные классы. SCSS partials и compiled CSS/maps закоммичены вместе, но pipeline компиляции SCSS не найден. `Footer.scss` ссылается на не найденный `respond-to` mixin. Bootstrap components импортируются, но глобальный импорт Bootstrap CSS в entry point не найден.

Media: API images, native `<video>`, Spotify embed, external social/music links и Google map iframe. Video category tiles используют одну hardcoded Bing image трижды. Большинство API images не имеют содержательного `alt`; interactive cards часто click-only и не keyboard-accessible. Modal не имеет dialog semantics, focus trap или Escape handling.

## Состояние страниц

Публичная feature-структура широкая и логически связана с API, но production data flow сейчас блокируется localhost base URL. `/developers`, mobile wrapper и часть card content являются заглушками/изолированным кодом. Admin mutations дополнительно имеют body-format mismatches.

## Проверки

- `tsc -p tsconfig.app.json --noEmit --incremental false`: **проходит**.
- Vite production bundle с output в `/tmp`: **проходит**, 535 modules transformed.
- Импорт `IVideoModel.tsx` при реально существующем `IVideoModel.ts` выглядит ошибочным, но Vite/TypeScript bundler resolution успешно его разрешает; это технический долг, не подтверждённая build-блокировка.
- ESLint: **не проходит**, 6 errors и 20 warnings (explicit `any`, `@ts-ignore`, hook dependencies).

## Рекомендуемый порядок чтения

1. `package.json`, `index.html`, `main.tsx`, `App.tsx`.
2. `Page.tsx` и shared layout/modal/header/footer.
3. `GlobalContext.tsx`.
4. Для каждой feature: page → context → list/container → item/modal → admin forms → model.
5. `style.css` как runtime truth, затем `style.scss` и partials для понимания намерения.
6. `.env.production`, `staticwebapp.config.json`; `dist` — только как потенциально устаревший artifact.
