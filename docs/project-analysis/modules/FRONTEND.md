# Модуль Frontend

## Назначение и границы

React SPA отвечает за public presentation, navigation, data fetching и встроенные admin controls. Граница — `frontend/storonnimv.client`; backend logic/data persistence не входят.

## Точки входа и ключевые файлы

- `index.html → src/main.tsx → src/App.tsx → components/pages/shared/Page.tsx`;
- `components/contexts/shared/GlobalContext.tsx` — HTTP/shared state;
- `src/styles/style.css` — runtime CSS;
- `staticwebapp.config.json` — hosting routes.

## Основные сущности и внутренняя структура

Route pages, feature contexts, feature elements/forms, TypeScript models, shared layout/modal/loading components. Public и admin UI живут в одном SPA.

## Зависимости

- **Входящие:** browser, static host, owner content/design.
- **Исходящие:** ASP.NET `/api`, external media/social/map/font resources.
- **Связи:** models зеркалят backend response DTO; forms — admin request DTO.

## Основной поток данных

Page → feature context → `sendRequest` → backend controller → response → context state → list/modal. Admin forms вызывают API напрямую через тот же wrapper.

## Реализовано

11 route screens, public lists/details/pagination, media embeds, shared loading/empty/modal, login и CRUD UI.

## Незавершено и риски

Hardcoded localhost, SCSS/CSS/dist drift, 9 body contract mismatches, placeholder Developers/video images, weak error/accessibility, desktop-only geometry. Vite/type-check проходят; ESLint не проходит.

## Неизвестно

Accepted design snapshot, deployed bundle, real browser behavior/data, mobile scope.

## Порядок чтения

Entry/routes → shared layout/global context → one public feature end-to-end → admin forms → runtime CSS/SCSS → deployment config.

## Доказательства

`package.json`; `Page.tsx`; `GlobalContext.tsx`; `components/contexts`; `components/elements`; `style.css`; `staticwebapp.config.json`. Подробности: [../04_FRONTEND.md](../04_FRONTEND.md).
