# FEAT-10 evidence — Developers page

## Цель

Удалить placeholder с `/developers`, сохранив пустую страницу в общем layout и direct SPA link.

## Исходное состояние

- Branch: `main`; HEAD: `ea77b34fc9d4c95a01ab7320260cf26c50c27769`.
- `FEAT-10` до начала имела статус `planned`; единственная зависимость `BASE-04` — `done`.
- `Page.tsx` уже содержит route `/developers`; `staticwebapp.config.json` содержит ровно один rewrite `/developers` в `/index.html`.
- Worktree был dirty до начала. `Developers.tsx` и `staticwebapp.config.json` не имели исходного diff; пользовательский diff `Page.tsx` (FEAT-08) не изменялся.
- Исходный full lint: exit `1`, 4 errors и 2 warnings вне `Developers.tsx` (QA-03 baseline).

## Scope и решение

Изменён только `frontend/storonnimv.client/src/components/pages/Developers.tsx`: удалены `<p>hello</p>` и ложный description. Компонент остаётся именованным `FC`; `Helmet defer={false}` сохраняет route-specific title при React 18 StrictMode без DOM content. Backend, DB, Blob, CSS, `Page.tsx`, static hosting config, dependencies и production resources не трогались.

## Проверки

| Команда или сценарий | Exit | Результат |
|---|---:|---|
| `npm run lint` до изменения | 1 | Исходный QA-03 baseline: 4 errors, 2 warnings вне Developers |
| placeholder scan до изменения | 0 | Найдены `hello` и старый description |
| `npm exec eslint -- src/components/pages/Developers.tsx` | 0 | Targeted lint green |
| placeholder scan после изменения | 1 | `hello` и description отсутствуют |
| `npm run build` | 0 | TypeScript и Vite production build green |
| static rewrite Node assertion | 0 | `/developers -> /index.html` |
| production bundle scan | 1 | Старый description и `localhost:44315` отсутствуют |
| `npm run lint` после изменения | 1 | Ровно тот же unrelated QA-03 baseline; Developers diagnostics нет |
| `curl http://127.0.0.1:41732/developers` | 0 / HTTP 200 | Получен Vite SPA entrypoint |
| `git diff --check` | 0 | Whitespace errors нет |

## Browser и visual result

Disposable mock API обслуживал только `GET /api/group-socials` как `200 []`; Vite запускался с `VITE_API_URL=http://127.0.0.1:41731/api`. Production API, DB и Blob не использовались.

- Before: `/tmp/storonnimv-feat10-before.png`; Safari direct navigation показала `/developers`, title `Розробники - Стороннім В`, shared navigation и видимый `hello`.
- After: `/tmp/storonnimv-feat10-after.png`; `/developers` остаётся на том же pathname, shared navigation/frame сохранены, `hello` и иной page content отсутствуют.
- Во время диагностики default deferred Helmet update оставлял title `Стороннім В`. `defer={false}` устранил race: финальная fresh direct navigation показала точный `Розробники - Стороннім В`.

## Status

Все обязательные FEAT-10 gates green. `04_BACKLOG.md`, `09_STATE.md` и `00_INDEX.md` обновлены: FEAT-10 `done`, M2 завершён, M3 активен; MOB-01 не начиналась.

## Acceptance summary

| Критерий | Итог |
|---|---|
| Placeholder и ложный description удалены | pass |
| Existing route и static rewrite сохранены | pass |
| Direct link возвращает SPA/200 | pass |
| Shared layout и пустой content | pass |
| Targeted lint/build/bundle checks | pass |
| Full lint baseline не ухудшен | pass |
| Точный document title | pass |
| Private-key headers, `AccountKey`, `SharedAccessSignature` в task source/evidence/bundle | pass (no matches) |
