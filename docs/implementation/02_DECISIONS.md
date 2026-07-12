# Журнал решений

| ID | Решение | Причина | Альтернативы | Компромисс | Источник | Статус |
|---|---|---|---|---|---|---|
| DEC-001 | Сохранить шесть публичных разделов и пустую статическую `/developers` | Явный ответ владельца | Удалить route; наполнить страницу | Сохраняется намеренная пустая страница | Ответ 1B | accepted |
| DEC-002 | Полный content admin и SuperAdmin входят в релиз на mobile/tablet/desktop | Контент должен управляться на всех версиях сайта | Public-only; desktop-only admin; частичный CRUD | Максимальный объём auth, security, forms и QA | Ответ 2A | accepted |
| DEC-003 | Сначала завершить локальное окружение; deployment выполнять позднее | Production target пока не подтверждён | Сразу восстанавливать существующий deployment; создавать новый | Локальная работа не блокируется, но релиз требует отдельного решения | Ответ 3C | accepted |
| DEC-004 | Mobile использует упрощённый layout без обязательного сохранения SVG-frame | Текущая frame geometry имеет desktop minimum width | Сохранять frame; отдельный макет | Mobile может визуально отличаться компоновкой | Ответ 4A | accepted |
| DEC-005 | Desktop baseline — текущий подключённый `style.css` | Владелец выбрал вариант A после разъяснения | SCSS; deployed snapshot; внешний макет | Новые SCSS-изменения не принимаются без сравнения | Последний ответ владельца | accepted |
| DEC-006 | Использовать существующие PostgreSQL и Azure Blob данные | Сохранение реального контента | Пустая БД; тестовые данные | Нужны backup, inventory и безопасная non-production копия | Ответ 5A | accepted |
| DEC-007 | `GroupPage` — singleton | Frontend читает одну страницу группы | История/версии; несколько групп | Требуется DB/application invariant | Ответ 6a | accepted |
| DEC-008 | Первый SuperAdmin вручную добавляется в БД | Явное решение владельца | Seed; setup tool; уже существующий account | Нужна безопасная документированная процедура, credentials вне Git | Ответ 6b | accepted |
| DEC-009 | EF migrations применяются отдельной командой | Владелец предварительно выбрал рекомендуемый вариант | Startup migrations; внешнее ручное создание schema | Deployment требует отдельного контролируемого шага | Ответ 2A после уточнения | accepted |
| DEC-010 | Hangfire dashboard отключается в production | Снижение operational exposure | Application auth; reverse proxy protection | В production нет dashboard UI | Ответ 6d | accepted |
| DEC-011 | Принят предложенный cross-device/browser/QA baseline | Владелец не задаёт собственные числа и принял предложение | Иная device matrix или quality gate | Точные browser versions фиксируются во время QA | Ответ 3 «да» | accepted |
| DEC-012 | Analytics не входит в релиз | Нет текущего требования | Добавить analytics/consent | Нет продуктовой телеметрии первого релиза | Ответ 8 | accepted |
| DEC-013 | Uploads получают лимиты и проверки | Владелец подтвердил необходимость | Оставить внешнее ограничение storage | Точные значения ещё должны быть подтверждены | Ответ 9 | accepted, values open |
| DEC-014 | Tracked historical logs удаляются | Владелец хочет отдельно решить будущую стратегию | Сохранить; архивировать | Новая стратегия sink не входит в текущий scope | Ответ 10 | accepted |
| DEC-015 | Принимающей стороной временно считается владелец проекта | Иной принимающий не указан | Назначить отдельного reviewer | Требует простого подтверждения до RC | Плановое допущение | assumed |
| DEC-016 | Existing architecture сохраняется; отдельная admin dashboard не создаётся | Запрос — завершить существующий проект | Перепроектировать frontend/backend | Меньше scope, но встроенные admin controls остаются сложнее mobile | Анализ + минимизация изменений | accepted technical constraint |

## Решения, которые можно пересмотреть позднее

- DEC-003 после выбора production hosting.
- DEC-010, если появится отдельная защищённая operational network policy.
- DEC-012 отдельным post-release решением.
- DEC-013 только в части численных лимитов, не в части обязательности validation.
- DEC-015 до начала финального release-candidate аудита.
