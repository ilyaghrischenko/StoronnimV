# Стратегия завершения

## 1. Исходное состояние

Frontend type-check и production bundle ранее проходили, lint не проходит. Backend build/run не доказан. Публичные и административные поверхности широки, но production API URL, auth pipeline, часть request contracts, mobile layout, media consistency, Hangfire job, tests и deployment workflow незавершены. Существующие данные и внешние ресурсы runtime не проверены.

## 2. Определение готового продукта

Продукт готов, когда все сценарии из [01_REQUIREMENTS.md](01_REQUIREMENTS.md) работают на согласованных desktop/mobile размерах, frontend/backend/data контракты проверены, локальный запуск воспроизводим, production выпуск отрепетирован, P0/P1 отсутствуют, а владелец проходит release checklist без блокирующих проблем.

## 3. Основные направления

- базовая сборка, конфигурация и локальные зависимости;
- безопасное восстановление реального контента;
- auth, authorization и единые API-контракты;
- вертикальное завершение public/admin features;
- целостность media и background processing;
- responsive public/admin UI и accessibility;
- автоматические, интеграционные и ручные проверки;
- CI, migration rehearsal, deployment и release audit.

## 4. Порядок этапов

| Этап | Цель | Входные условия | Результаты | Модули | Зависимости | Риски | Критерий завершения | Backlog |
|---|---|---|---|---|---|---|---|---|
| M1 Reproducible local foundation | Получить доказанный локальный vertical read flow | Repo и toolchain | Backend/frontend build, schema, non-production content copy, browser smoke | Все, DB, Blob | Нет | Внешние данные недоступны; portability | Новый разработчик повторяет запуск по документу | `BASE-01`, `BASE-02`, `DATA-01`, `DATA-02`, `BASE-03`, `BASE-04`, `QA-01` |
| M2 Functional desktop | Завершить auth и обязательные public/admin flows | M1 | Проверенный desktop public CRUD, SuperAdmin, media, Hangfire | FE, API, DB, Blob | M1 | Cookie/CSRF, binding, data loss | Все desktop acceptance scenarios проходят | `API-01`, `DATA-03`, `API-02`, `FEAT-01`, `FEAT-02`, `API-03`, `DATA-04`, `FEAT-03`, `FEAT-04`, `FEAT-05`, `API-04`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `FEAT-09`, `FEAT-10` |
| M3 Functional mobile | Сделать весь подтверждённый scope usable на 320–1024 px | M2 и visual baseline | Responsive layout, pages, media, modals, admin | Frontend | M2 | CSS/SCSS drift, table/forms density | Device matrix проходит без overflow/blockers | `MOB-01`, `MOB-02`, `MOB-03`, `MOB-04`, `MOB-05`, `MOB-06` |
| M4 Integrated quality | Получить regression protection и release evidence | M3 | Tests, E2E, lint, audits | Tests и все runtime модули | M3 | Flaky external services | Quality gates повторяемы и green | `QA-02`, `QA-03`, `QA-04`, `QA-05` |
| M5 Deployment readiness | Подготовить выбранное production окружение и выпуск | M4, решение hosting/access | CI, env contract, backup/migration/rollback, deploy, docs/cleanup | Ops и все модули | M4, external access | Data loss, secrets, topology | Rehearsal пройден и production deploy успешен | `OPS-01`, `OPS-02`, `OPS-03`, `CLEAN-01`, `DOC-01`, `OPS-04` |
| M6 Release candidate | Подтвердить реальную готовность | M5 | Production smoke и owner acceptance | Все | M5 | Real content/integration drift | Нет P0/P1; checklist подписан | `QA-06`, `QA-07` |

## 5. Зависимости и критический путь

`BASE-01 → BASE-02 → DATA-01 → BASE-03 → BASE-04 → QA-01 → API-01 → API-02 → FEAT-01 → API-03 → DATA-04 → feature CRUD → MOB-01 → MOB-02/MOB-03/MOB-04/MOB-05 → MOB-06 → QA-04 → OPS-03 → OPS-04 → QA-06 → QA-07`.

`DATA-02` идёт после schema и блокирует проверки реального контента. `DATA-03` блокирует SuperAdmin. `API-04` зависит от Schedule contract. Deployment не блокирует локальные milestones, но блокирует логическое завершение проекта.

## 6. Допустимая параллельная работа

- После `API-03` независимые feature-сценарии можно выполнять параллельно, но media features ждут `DATA-04`.
- После `MOB-01` группы публичных страниц `MOB-02` и `MOB-03` можно разделить между исполнителями; `MOB-04` проверяет общие элементы после них.
- Backend tests и frontend tests можно развивать параллельно после стабилизации соответствующих контрактов.
- `OPS-01` можно исследовать во время M4, но production changes начинаются только после quality gate.

## 7. Точки ревью

1. После clean backend build: подтвердить toolchain и отсутствие скрытой portability-блокировки.
2. После data restore: владелец подтверждает, что контент и media соответствуют ожидаемым.
3. После auth vertical: security review cookie/CORS/CSRF и SuperAdmin policy.
4. После первого DB-only и первого media CRUD: утвердить повторяемый contract/compensation pattern.
5. После desktop demo: проверить весь объём до responsive работ.
6. После mobile cross-device audit: утвердить layout на пяти ширинах.
7. Перед migration rehearsal: подтвердить backup, target environment и rollback authority.
8. После production smoke: финальное принятие владельцем.

## 8. Стратегия мобильной адаптации

Сначала `MOB-01` создаёт единый responsive foundation на базе runtime CSS: убирает desktop minimum width, вводит layout/nav breakpoints и определяет CSS source workflow. Затем страницы делятся по группам, общие media/modal/state patterns проверяются отдельно, admin получает самостоятельную mobile table/form strategy, а `MOB-06` закрывает accessibility, touch, reduced motion и cross-device regressions. SVG-frame разрешено скрывать или упрощать на narrow screens.

## 9. Стратегия тестирования

- Unit/service tests: auth, role boundaries, date/status logic, singleton, validation и compensation decisions.
- Integration tests: PostgreSQL, request binding, migrations, cookie auth, media adapter и Hangfire.
- Frontend tests: contexts/forms/guards/states и responsive interaction behavior.
- E2E: visitor, Basic Admin, SuperAdmin и direct/deep-link flows.
- Manual visual: реальные данные на 320, 375, 768, 1024 и 1440 px в согласованных browsers.

## 10. Стратегия deployment

Сначала выбрать topology и target provider, затем задокументировать secrets/origins, создать CI gates, сделать backup и rehearsal отдельной migration command, проверить rollback и только затем deploy. Startup не применяет migrations. Hangfire dashboard production-конфигурацией отключён. Production data mutations до backup запрещены.

## 11. Финальный аудит

Аудит сверяет requirements, decisions, backlog IDs, runtime behavior, content, accessibility, security, SEO, performance, deployment и документацию. Все отклонения классифицируются как P0–P3; открытые P0/P1 блокируют выпуск.

## 12. Критерии завершения проекта

Проект завершён только когда:

1. Все обязательные visitor, Basic Admin, SuperAdmin и background сценарии работают.
2. Frontend/backend/data согласованы и воспроизводимо запускаются.
3. Критические defects устранены, а loading/error/empty states различимы.
4. Все обязательные страницы и формы адаптированы под mobile/tablet/desktop.
5. Build, automated tests, E2E и ручная device/browser matrix пройдены.
6. Формы, media, navigation, roles и APIs соответствуют решениям владельца.
7. Deployment выполнен с backup/migration/rollback evidence.
8. Нет открытых P0/P1, документация соответствует коду.
9. Владелец может пройти release checklist без блокирующих проблем.

Выполнение backlog доказывает готовность в утверждённом объёме, но не обещает отсутствие любых будущих дефектов.
