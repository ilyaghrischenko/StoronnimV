# План завершения StoronnimV

**Дата подготовки:** 12 июля 2026 года
**Состояние:** планирование завершено; `BASE-01`, `BASE-02`, `DATA-01`, `BASE-03` и `BASE-04` выполнены.

## Назначение

Каталог содержит утверждённый план доведения существующего сайта музыкальной группы до проверяемого завершения и evidence выполненных задач.

## Документы и порядок чтения

1. [01_REQUIREMENTS.md](01_REQUIREMENTS.md) — утверждённый объём и ограничения.
2. [02_DECISIONS.md](02_DECISIONS.md) — журнал решений владельца и технических допущений.
3. [03_IMPLEMENTATION_PLAN.md](03_IMPLEMENTATION_PLAN.md) — этапы, зависимости и критический путь.
4. [04_BACKLOG.md](04_BACKLOG.md) — implementation-ready задачи.
5. [05_MILESTONES.md](05_MILESTONES.md) — демонстрируемые этапы выпуска.
6. [06_VALIDATION_PLAN.md](06_VALIDATION_PLAN.md) — автоматическая и ручная проверка.
7. [07_TRACEABILITY.md](07_TRACEABILITY.md) — связь анализа, требований и backlog.
8. [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md) — неблокирующие внешние решения.
9. [09_STATE.md](09_STATE.md) — компактное состояние для следующих сеансов Codex.
10. [10_RUNTIME_CONTRACT.md](10_RUNTIME_CONTRACT.md) — канонические runtime requirements, configuration names и безопасная последовательность подготовки локального окружения.
11. [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md) — явная безопасная команда EF migrations и schema inspection.
12. [evidence/BASE-02.md](evidence/BASE-02.md) — фактические команды и результаты clean backend restore/build.
13. [evidence/DATA-01.md](evidence/DATA-01.md) — фактические команды и результаты migration workflow на пустой PostgreSQL.
14. [evidence/BASE-03.md](evidence/BASE-03.md) — фактические команды и результаты local API startup, health и Development OpenAPI.
15. [evidence/BASE-04.md](evidence/BASE-04.md) — фактические команды и результаты frontend environment API URL, production bundle и browser network inspection.

## Подтверждено владельцем

- Сохраняются публичные страницы Home, Schedule, News, Music, Group и Video.
- `/developers` сохраняется как пустая статическая страница.
- Полный content admin и SuperAdmin входят в релиз и работают на mobile, tablet и desktop.
- Публичная мобильная версия полноценная; разрешён отдельный упрощённый layout без обязательного сохранения SVG-рамки.
- Визуальный baseline — текущий runtime `style.css` и фактический desktop UI.
- Источник контента — существующие PostgreSQL и Azure Blob данные после безопасной инвентаризации и backup.
- `GroupPage` — singleton.
- Первый SuperAdmin вручную вносится в подготовленную БД.
- Migrations применяются явной командой, не автоматически при startup.
- Hangfire dashboard отключён в production.
- Analytics в текущий релиз не входит.
- Для uploads обязательны лимиты и проверки файлов.
- Закоммиченные исторические logs должны быть удалены; новая стратегия логирования пока не определяется.
- Сначала восстанавливается локальное окружение; production deployment остаётся поздним этапом.

## Остаётся открытым

Открытые пункты не блокируют первый milestone: production hosting/topology и доступы, точные upload-лимиты, фактическая доступность backup существующих данных, будущая стратегия логирования и явное подтверждение принимающего лица. Они перечислены в [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md) с крайним этапом решения.
