# Открытые вопросы владельцу

Здесь нет вопросов, которые можно решить дальнейшим чтением repository.

## 1. Какой environment сейчас считается рабочим?

- **Почему возник:** repository содержит production env name, localhost в source, Dockerfile и static routes, но нет deployment pipeline.
- **Проверено:** `GlobalContext.tsx`, `.env.production` (только имя/назначение), Dockerfile, `staticwebapp.config.json`, Git history.
- **Почему важно:** определяет реальные API origin, CORS/cookie domain и возможность runtime проверки.
- **Вероятные варианты:** текущий Azure deployment; старый deployment; только local; environment больше не существует.
- **Нужно уточнить:** URLs и доступность ресурсов без передачи секретов в чат/документацию.

## 2. Что является источником истины для дизайна и стилей?

- **Почему:** `style.css`, SCSS partials и `dist` расходятся; pipeline SCSS не найден.
- **Проверено:** `index.html`, `style.scss`, `style.css`, `Header.scss`, `dist`.
- **Почему важно:** нельзя планировать mobile/UI backlog, не выбрав актуальный дизайн.
- **Варианты:** runtime CSS; SCSS как новая незакомпилированная версия; deployed UI; внешний макет.
- **Нужно уточнить:** какой snapshot владелец принимает как актуальный.

## 3. Нужна полноценная mobile-версия или временная заглушка?

- **Почему:** `ResolutionWrapper` показывает «в разработке», но отключён; README заявляет full responsive.
- **Проверено:** `App.tsx`, `ResolutionWrapper.tsx`, `MobileInDeveloping.tsx`, responsive styles.
- **Почему важно:** это два принципиально разных product decisions.
- **Варианты:** полноценный responsive UI; временная блокирующая страница; public mobile only без admin.
- **Нужно уточнить:** целевые devices и необходимость mobile admin.

## 4. Какая модель управления GroupPage ожидается?

- **Почему:** backend допускает много GroupPage rows, UI читает одну и не имеет create/delete flows.
- **Проверено:** GroupPage controller/service/repository, forms/routes.
- **Почему важно:** нужен invariant singleton или список/версии.
- **Варианты:** ровно одна запись; история версий; несколько групп/страниц.
- **Нужно уточнить:** product intent.

## 5. Как должен создаваться первый SuperAdmin?

- **Почему:** seed/bootstrap/setup command не найден, API требует SuperAdmin role.
- **Проверено:** migrations, entities, services, startup, tests, README.
- **Почему важно:** без bootstrap административная система может быть недоступна с чистой DB.
- **Варианты:** manual DB row; private migration/seed; уже существующий cloud account; отдельный tool вне repo.
- **Нужно уточнить:** фактический безопасный процесс.

## 6. Должен ли Hangfire dashboard быть публично доступен?

- **Почему:** route подключён без authorization filter.
- **Проверено:** `Program.cs`, auth configuration.
- **Почему важно:** определяет, дефект ли это deployment-exposure или route закрывает внешняя инфраструктура.
- **Варианты:** public intentionally; protected by reverse proxy; должен быть app-auth protected; disabled in production.
- **Нужно уточнить:** network/deployment policy.

## 7. Какие content/admin сценарии реально считались завершёнными?

- **Почему:** UI и API широкие, но body contracts расходятся; `/developers` и media tiles — placeholders.
- **Проверено:** все routes, forms, controllers, DTO, Git history.
- **Почему важно:** помогает отличить abandoned experiment от regression.
- **Варианты:** public read only; весь CRUD; только часть content types.
- **Нужно уточнить:** последний известный рабочий сценарий и ожидаемые acceptance criteria.

## 8. Можно ли считать текущие historical logs допустимыми для хранения?

- **Почему:** 35 log files закоммичены; содержимое не анализировалось во избежание распространения данных.
- **Проверено:** tracked file inventory и logging configuration.
- **Почему важно:** privacy/secret retention и repository hygiene.
- **Варианты:** synthetic/safe logs; реальные production/dev logs; уже устаревшие данные.
- **Нужно уточнить:** владелец должен провести authorized data/secret review.
