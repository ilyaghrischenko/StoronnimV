# Открытые пункты

Эти пункты не блокируют `BASE-01`, но должны быть решены не позднее указанного этапа.

| ID | Вопрос | Ответственный | Крайний этап | Затрагиваемые задачи | Безопасное временное допущение |
|---|---|---|---|---|---|
| OPEN-001 | Какой provider, frontend/API URLs, DNS/TLS и secrets store используются в production? | Владелец | До M5 | `OPS-01`, `OPS-03`, `OPS-04`, `QA-06` | Работать только локально; не создавать production config |
| OPEN-002 | Доступны ли актуальные PostgreSQL и Blob backup, и кто разрешает их чтение? | Владелец | До `DATA-02` | `DATA-02`, `FEAT-03`, `FEAT-04`, `FEAT-05`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `OPS-03` | Использовать минимальные test fixtures; не считать content ready |
| OPEN-003 | Какие точные upload-лимиты и разрешённые форматы нужны для photo/video? | Владелец + реализатор | До `DATA-04` | `DATA-04`, `FEAT-04`, `FEAT-05`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `QA-04` | Сначала измерить существующий corpus; выбрать консервативные configurable limits и подтвердить владельцем |
| OPEN-004 | Есть ли дубликаты GroupPage или некорректные production records? | Реализатор с владельцем | До `FEAT-06` | `DATA-02`, `FEAT-06`, `OPS-03` | Не удалять автоматически; сформировать report и выбрать canonical row |
| OPEN-005 | Кто и каким защищённым каналом вносит первого SuperAdmin и хранит credentials? | Владелец | До `DATA-03` | `DATA-03`, `FEAT-01`, `DOC-01` | Процедура описывает роли, но не содержит credentials |
| OPEN-006 | Какая стратегия runtime logging нужна после удаления tracked logs? | Владелец | После первого релиза или до M5, если hosting требует | `CLEAN-01`, `OPS-01`, `DOC-01` | Console/runtime platform logs без tracked files; не добавлять новый sink |
| OPEN-007 | Кто формально принимает release candidate? | Владелец | До M6 | `QA-07` | Принимающей стороной считается владелец проекта |
| OPEN-008 | Какие реальные изображения используются для трёх Video category tiles? | Владелец | До `FEAT-08` | `DATA-02`, `FEAT-08`, `MOB-03` | Выбрать из подтверждённого existing Blob corpus; не использовать Bing placeholder |
