# Требования

## Цель продукта

Логически завершённый StoronnimV — украиноязычный мультимедийный сайт-визитка группы, где посетитель без блокирующих ошибок просматривает актуальные новости, афишу, музыку, состав группы, видео и социальные ссылки, а авторизованные администраторы управляют этим контентом на телефоне, планшете и desktop. Проект воспроизводимо запускается локально, имеет проверяемые контракты frontend/backend/data и подготовлен к последующему production deployment.

## Жёсткие ограничения

- Сохраняется существующий стек: React/TypeScript/Vite, ASP.NET Core/.NET 9, PostgreSQL, Azure Blob Storage и Hangfire. Замена архитектуры не входит в завершение проекта.
- Исходный desktop-визуал определяется подключённым `src/styles/style.css`; SCSS используется только после проверки визуальной эквивалентности.
- Полноценная mobile/tablet адаптация обязательна для публичной и административной частей.
- Migrations выполняются отдельной явной командой; startup не изменяет schema автоматически.
- Первый SuperAdmin создаётся вручную в БД по документированной безопасной процедуре.
- `GroupPage` содержит ровно одну логическую запись.
- Hangfire dashboard не доступен в production.
- Существующие production данные и Blob media нельзя изменять до backup и инвентаризации.
- Секреты не хранятся в Git и не копируются в документацию.
- Новая отдельная admin dashboard, смена стека и редизайн desktop не входят в объём.

## Приоритеты

1. Получить чистую сборку и воспроизводимый локальный запуск.
2. Подготовить воспроизводимый локальный test corpus в PostgreSQL/Azurite; реальные production data/media проверять позднее при deployment readiness.
3. Завершить auth и API-контракты, затем обязательные вертикальные CRUD-сценарии.
4. Завершить desktop-функциональность до массовой мобильной адаптации страниц.
5. Выполнить полную public/admin mobile адаптацию.
6. Создать регрессионные проверки, затем готовить deployment.

## Предпочтения

- Сохранять существующую архитектуру и визуальную идентичность.
- Делать задачи небольшими вертикальными сценариями.
- Упрощать mobile-композицию вместо буквального сжатия SVG-frame.
- Использовать автоматические проверки для стабильных контрактов и ручные визуальные проверки для layout/content.

## Резервные сценарии

- Для локальных milestones используется минимальный согласованный test corpus в PostgreSQL/Azurite. Источник и перенос реального production content определяются отдельно в M5 и не блокируют локальную разработку.
- Если production hosting ещё не выбран, milestones до интеграционно проверенной локальной версии выполняются, а deployment остаётся заблокированным external item.
- Если Azure Blob нельзя безопасно использовать локально, применяется отдельный development container/Azurite с теми же контрактами; production media не изменяются.
- Если SVG-frame не помещается на узком экране, mobile использует самостоятельный одноколоночный layout.

## Обязательные пользовательские сценарии

### Посетитель

- Навигация между Home, Schedule, News, Music, Group и Video без full-page ошибок и horizontal overflow.
- Home показывает ближайшее выступление, шесть новостей и promotion video либо различимые loading/empty/error состояния.
- News и Schedule поддерживают pagination и detail modal; Schedule показывает корректные дату, статус, место и карту.
- Group показывает единственное описание группы, участников и их социальные ссылки.
- Music показывает управляемые ссылки платформ и Spotify embed.
- Video показывает три категории, списки, pagination и реальные согласованные category images.
- Footer показывает социальные ссылки группы.
- `/developers` открывается как пустая стабильная статическая страница.
- Неизвестные и прямые SPA URLs обрабатываются предсказуемо.

### Basic Admin

- Login/logout и server-side проверка сессии.
- Create/read/update/delete для news, schedules, videos, group singleton, members/socials, music platforms и group socials.
- Upload/replace/delete media с валидацией и без необъяснимого рассогласования DB/Blob.
- Все формы доступны и пригодны на mobile, tablet и desktop.

### SuperAdmin

- Все Basic Admin возможности.
- List/create/edit/delete только Basic Admin accounts.
- Защита от изменения или удаления SuperAdmin через Basic Admin endpoints.
- Route guard не доверяет одной строке роли в `sessionStorage`.

### Фоновый и операционный сценарий

- Daily Hangfire job дожидается обновления всех просроченных Schedule records и повторно выполняется безопасно.
- `/health` отражает доступность API и PostgreSQL.
- Hangfire dashboard отключён в production.

## Исключённый функционал

- Analytics и cookie-consent, пока не появится отдельное требование.
- Магазин, продажа билетов, поиск, contact/booking формы и мультиязычность.
- Наполнение `/developers`; страница намеренно остаётся пустой.
- Новая отдельная admin dashboard.
- Автоматическое применение EF migrations при startup.
- Публичный production Hangfire dashboard.
- Необязательный архитектурный рефакторинг и очистка всего технического долга.
- Определение новой долговременной стратегии файлового логирования.

## Нефункциональные требования

### Устройства и layout

- Обязательные ширины проверки: 320, 375, 768, 1024 и 1440 px.
- Нет горизонтального overflow на основных страницах.
- Navigation, media, modals, forms и admin tables остаются usable при touch input.
- Проверяются landscape и portrait там, где layout существенно меняется.

### Браузеры

Актуальные стабильные Chrome, Safari, Firefox и Edge. Точные версии фиксируются в момент QA, а не в plan snapshot.

### Доступность

- Семантические интерактивные элементы, keyboard navigation и видимый focus.
- Доступные имена controls и содержательные `alt` для контентных изображений.
- Modal имеет dialog semantics, focus management, Escape и возврат focus.
- Hover-only действие имеет touch/keyboard эквивалент.
- Уважается `prefers-reduced-motion` для необязательной анимации.

### Производительность

- Responsive media sizing, разумный preload/lazy loading и отсутствие ненужной загрузки тяжёлого video на mobile.
- Production bundle не содержит localhost API URL.
- Performance проверяется на основных публичных маршрутах после подключения реального контента.

### Безопасность

- Authentication/authorization проверяются сервером.
- Credentialed cookie flow имеет согласованные HTTPS, CORS, SameSite/domain и CSRF-защиту.
- Uploads имеют whitelist форматов, size limits и проверку типа/signature.
- Hangfire dashboard закрыт в production; ошибки не раскрывают внутренние детали.
- Basic Admin operations не могут затронуть SuperAdmin account.

### SEO

- Корректные `lang`, canonical, title, description и social metadata для публичных страниц.
- Direct links и 404 работают на целевом static hosting.
- Analytics не требуется.

### Deployment и поддерживаемость

- Local startup документирован и воспроизводим.
- Production migrations выполняются отдельным контролируемым шагом с backup/rollback.
- Конфигурация задаётся environment variables с едиными именами.
- Build/test/deployment gates автоматизируются до выпуска.
- Исторические tracked logs удаляются; решение о будущих sinks принимается отдельно.
