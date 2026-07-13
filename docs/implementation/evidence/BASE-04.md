# BASE-04 — Frontend environment API URL evidence

## Цель

Подключить frontend к обязательному валидируемому `VITE_API_URL`, исключить hardcoded `https://localhost:44315/api` из client code и production bundle и доказать реальный browser request к отдельно заданному local API URL.

## Исходное состояние

- `BASE-04` имела статус `planned`; её единственная зависимость `BASE-03` имела статус `done`.
- `GlobalContext.tsx` задавал `https://localhost:44315/api` строковым литералом; существующая проверка `if (!serverRoute)` была недостижима.
- Tracked `.env.production` уже содержал `VITE_API_URL`, но client его не читал; frontend local env example отсутствовал.
- `git status --short` перед задачей был пуст: пользовательских изменений в worktree не было.

## Scope и затронутые файлы

| Файл | Изменение |
|---|---|
| `frontend/storonnimv.client/vite.config.ts` | Добавлены обязательная build/dev-time validation и нормализация `VITE_API_URL` |
| `frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx` | `serverRoute` получает встроенное Vite environment value вместо hardcoded URL |
| `frontend/storonnimv.client/src/vite-env.d.ts` | Добавлен TypeScript contract для `VITE_API_URL` |
| `frontend/storonnimv.client/.env.example` | Добавлен безопасный local API example |
| `frontend/README-front.md` | Описана подготовка `.env.local` и требования к URL |
| `docs/implementation/10_RUNTIME_CONTRACT.md` | Environment matrix и local runbook синхронизированы с реализованным поведением |
| `docs/implementation/evidence/BASE-04.md` | Добавлен текущий evidence |
| `docs/implementation/04_BACKLOG.md` | `BASE-04` отмечена `done` |
| `docs/implementation/09_STATE.md` | Зафиксированы проверки, факты и следующая planned task |
| `docs/implementation/00_INDEX.md` | Добавлена ссылка на evidence |

HTTP state architecture, Axios wrapper/callers, API contracts, backend, package versions, `.env.production` value и feature behavior не менялись.

## Решения и выполненные изменения

- Сохранён существующий contract `serverRoute` и все его callers; изменён только источник base URL.
- `VITE_API_URL` включает `/api`, как существующий tracked production value и все callers, которые добавляют controller-relative paths.
- Vite до dev server/build требует непустой absolute HTTP(S) URL и отклоняет credentials, query и fragment. Поэтому invalid configuration останавливается до создания bundle.
- URL нормализуется стандартным `URL`; завершающие `/` удаляются, чтобы caller `${serverRoute}/...` не создавал двойной slash.
- Local example использует `http://localhost:5268/api`; реальный endpoint при запуске должен быть согласован с backend и `CLIENT_URL`.

## Выполненные команды и результаты

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Dependency/status | Чтение `04_BACKLOG.md`; `git status --short` | `BASE-03=done`, `BASE-04=planned`; worktree чист | 0 | Зависимость завершена; пользовательских изменений нет |
| TypeScript project build | `npm exec tsc -- -b --pretty false` | Без diagnostics | 0 | Изменённые TypeScript/Vite contracts компилируются |
| Local-mode build | `VITE_API_URL=http://localhost:5268/api/ npm exec vite -- build --mode test --outDir /tmp/storonnimv-base04-valid --emptyOutDir` | 535 modules; bundle создан вне repo | 0 | Local/build mode принимает valid URL с trailing slash |
| Missing value | `env -u VITE_API_URL npm exec vite -- build --mode base04-missing ...` | `VITE_API_URL is required`; bundling не начат | 1, ожидаемый | Обязательная переменная проверяется fail-fast |
| Invalid absolute URL | `VITE_API_URL=not-a-url npm exec vite -- build --mode base04-invalid ...` | Явная ошибка absolute HTTP(S) URL | 1, ожидаемый | Произвольная строка не принимается |
| Invalid URL forms | Test-mode builds с `ftp://...`, credentials и query | Все три отклонены общей явной ошибкой | 1 каждый, ожидаемый | Protocol/credentials/query contract реально enforced |
| Narrow lint | `npm exec eslint -- vite.config.ts src/vite-env.d.ts` | Без diagnostics | 0 | Новая Vite validation и env typing проходят ESLint |
| Production build | `npm run build` | `tsc -b && vite build`; 535 modules; production bundle создан | 0 | Обязательные type-check и Vite build проходят вместе |
| Production bundle search | `rg --fixed-strings 'localhost:44315' dist` | Совпадений нет | 1, ожидаемый для отсутствия | Hardcoded прежний endpoint отсутствует в bundle |
| Configured production URL | `rg --fixed-strings 'https://api.storonnimv.com/api' dist` | URL найден в сгенерированном JS | 0 | Bundle использует tracked production environment value |
| Browser network inspection | Local mock API `127.0.0.1:41731`; Vite `127.0.0.1:41732` с `VITE_API_URL=http://127.0.0.1:41731/api/`; встроенный browser открыл `/developers` | Страница: URL `/developers`, title `Розробники - Стороннім В`, body `hello`; API log: два `GET /api/group-socials` без двойного slash | 0 | Реальный browser client обращается к environment-selected local API; повтор вызова вызван существующим React `StrictMode` |
| Local process cleanup | Controlled stop mock API и Vite | Оба процесса остановлены после smoke | 130, ожидаемый SIGINT | Временные local services не оставлены |
| Full frontend lint | `npm run lint` | 6 errors и 20 warnings — тот же documented pre-task baseline | 1 | Общий lint gate ещё не закрыт; BASE-04 не добавила новые diagnostics в Vite config/env typing |
| Diff whitespace | `git diff --check` | Ошибок нет | 0 | Изменения не содержат whitespace/conflict-marker defects |
| Secret scan | Поиск в добавленных строках diff private keys, access keys и неплейсхолдерных secret assignments | Совпадений нет | 1, ожидаемый для отсутствия | В изменения не добавлены распознаваемые secrets |
| Final scope review | `git status --short`, полный diff и чтение двух untracked files | Только 10 файлов BASE-04; unrelated changes и commit отсутствуют | 0 | Итоговый worktree соответствует scope; следующая задача не начата |

Standalone Playwright CLI сначала был запущен для browser inspection, но не смог стартовать из-за отсутствия Chrome installation. Browser evidence получен встроенным браузером Codex без установки новой dependency.

## Невыполненные проверки

- Реальный backend не запускался: корректный local API startup уже доказан `BASE-03`, а BASE-04 проверяет frontend routing без DB/Blob side effects.
- Production API connectivity не проверялась: production topology и exact URLs остаются `OPEN-001` до M5.
- Visual before/after screenshots не создавались: задача не меняет DOM, CSS или layout; обязательная проверка — network inspection.
- Full ESLint gate не проходит из-за существующих 6 ошибок и 20 warnings. Их исправление относится к `QA-03`, если не потребуется раньше отдельной feature task.

## Проблемы вне scope

- Existing lint defects: `no-explicit-any` в `GlobalContext.tsx`, пять `@ts-ignore` errors и 20 hook-dependency warnings. Они существовали в documented baseline до BASE-04, build не блокируют и не исправлялись.
- Standalone Playwright CLI зависит от отсутствующей локальной Chrome installation; это не blocker, поскольку browser scenario выполнен встроенным браузером.
- Tracked `.env.production` содержит ранее заданный `https://api.storonnimv.com/api`, но реальная production topology не подтверждена (`OPEN-001`). Значение не менялось и внешняя connectivity не заявляется.
- В разделе известных ограничений `10_RUNTIME_CONTRACT.md` остаётся pre-existing stale строка, что API startup якобы не доказан в `BASE-03`; актуальные evidence и `09_STATE.md` подтверждают обратное. Это не блокирует BASE-04 и не исправлялось как несвязанная документационная ошибка.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Frontend использует `VITE_API_URL` в local/build modes | Выполнен | Vite validation/injection; local test-mode build; production build |
| Конфигурация URL валидируется | Выполнен | Missing, malformed, non-HTTP(S), credentials и query cases fail-fast |
| Trailing slash не создаёт неверный request path | Выполнен | Local env задан с trailing slash; browser API log содержит `/api/group-socials` |
| Local client обращается к local API | Выполнен | Встроенный browser + mock API network log |
| Production bundle не содержит hardcoded `44315` | Выполнен | Exact bundle search не нашёл `localhost:44315` |
| HTTP state architecture не меняется | Выполнен | `serverRoute`, `sendRequest` и все callers сохранены; изменён только источник URL |

Все критерии приёмки `BASE-04` выполнены. Статус задачи установлен `done`. Следующая planned task по backlog — `DATA-02`; она не начиналась в рамках `BASE-04`.
