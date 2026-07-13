# DATA-02 — Safe content and media copy evidence

## Цель

Получить backup существующих PostgreSQL и Azure Blob данных, составить inventory, восстановить данные в non-production окружении, сверить количество сущностей/media и проверить выборочные публичные media URLs без изменения production ресурсов.

## Исходное состояние

- `DATA-02` существует в backlog со статусом `planned`; её единственная зависимость `DATA-01` имеет статус `done`.
- Рабочее дерево до проверки было чистым.
- `OPEN-002` не закрыт: владелец не подтвердил доступность актуальных PostgreSQL/Blob backup и не зафиксировал разрешение на их чтение.
- В repository нет PostgreSQL backup, Blob export или inventory существующего content/media.
- Игнорируемый API `.env` содержит настроенные local и non-local connection settings, но их значения не выводились и не копировались. Non-local targets не использовались.

## Затронутые файлы

| Файл | Изменение |
|---|---|
| `docs/implementation/evidence/DATA-02.md` | Зафиксированы исходное состояние, безопасные диагностические проверки и доказанный внешний blocker |
| `docs/implementation/12_DATA_COPY_WORKFLOW.md` | Добавлен source-read-only PostgreSQL/Blob backup, non-production restore, inventory и URL-sampling workflow |
| `docs/implementation/sql/DATA_02_INVENTORY.sql` | Добавлен counts-only inventory девяти entities, migration history и media references |
| `docs/implementation/sql/DATA_02_REWRITE_MEDIA_URLS.sql` | Добавлен prefix-guarded rewrite media URLs только в restored target DB |

Backlog, state, application code и configuration не менялись. Коммит не создавался.

## Принятые решения

- Не подключаться к `DB_CLOUD` и `BLOB_STORAGE`: targets классифицированы как non-local или непроверенные, а обязательное разрешение владельца из `OPEN-002` отсутствует.
- Не использовать минимальные fixtures как доказательство завершения. Requirements разрешают их только как fallback для локальной разработки, а `OPEN-002` прямо запрещает считать при этом content ready.
- Добавить минимальный workflow только для подтверждённых стандартных interfaces: PostgreSQL custom-format `pg_dump`/`pg_restore`, Azure CLI Blob list/download/upload и counts-only SQL. PostgreSQL часть проверена на disposable migrated DB; Azure часть ограничена offline CLI contract validation до появления разрешённого source/dev target.
- Использовать отдельные libpq DSNs `SOURCE_PG_DSN`/`TARGET_PG_DSN` вместо application `DB_CLOUD`: `psql`, `pg_dump` и `pg_restore` не принимают Npgsql semicolon connection strings.
- Не выполнять destructive cleanup/overwrite: source sessions read-only, target DB обязана быть пустой, target Blob containers создаются с `--fail-on-exist`, uploads используют `--overwrite false`.
- Оставить `DATA-02` в статусе `planned` и не обновлять `09_STATE.md`: обязательные критерии приёмки не выполнены.

## Выполненные изменения

1. Добавлен counts-only SQL inventory без logins, hashes, content text и URL values.
2. Добавлен runbook, который разделяет source/target credentials, не передаёт их через Git/arguments, требует явную authorization/ACL gate и сохраняет artifacts вне repository.
3. PostgreSQL workflow выполнен на disposable PostgreSQL 17: применены 24 migrations, создан custom-format backup, восстановлен пустой target, source/target inventories совпали.
4. Target-only URL rewrite проверен на семи disposable rows, покрывающих все media URL fields: все семь source prefixes заменены, unrelated social URL сохранён; одинаковые source/target bases отклоняются до transaction.
5. Azure CLI 2.79.0 локально подтвердил наличие используемых Blob list/download/upload/container commands и options. Реальные Blob operations не выполнялись.

## Выполненные команды и результаты

Команды не выводили значения connection strings, credentials, account names или tokens.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Исходный working tree | `git status --short` | Вывод отсутствовал | 0 | До DATA-02 не было пользовательских или task changes |
| Наличие задачи и dependency | Поиск строк `DATA-01`/`DATA-02` в `04_BACKLOG.md` и чтение `09_STATE.md` | `DATA-01` — `done`; `DATA-02` — `planned` | 0 | Dependency завершена, задачу можно начинать только после data-access gate |
| Authorization gate | Чтение `08_OPEN_ITEMS.md` | `OPEN-002` назначен владельцу и должен быть решён до `DATA-02` | 0 | Разрешение на чтение backup не зафиксировано в project sources |
| Backup artifact search | Поиск `*.sql`, `*.dump`, `*.backup`, `*.bak`, `*.tar`, `*.zip` и файлов с `backup`/`restore`/`inventory` в repository | Backup/export существующих данных не найден | 0 | В workspace нет входного PostgreSQL/Blob backup для restore rehearsal |
| Environment classification | Проверка только имён и класса значений в ignored API `.env` | `DB_LOCAL_ILYA`/`DB_LOCAL_DIMA` — local; `DB_CLOUD`/`BLOB_STORAGE` — non-local или непроверенные | 0 | Cloud settings нельзя безопасно считать development targets; secret values не раскрыты |
| Local PostgreSQL reachability | TCP probe configured local ports на `127.0.0.1` | Оба local endpoints недоступны | 0 для сценария; оба probes вернули 1 | Готовая local source/copy DB не запущена |
| Local container inventory | `docker ps --format ...` | Список запущенных containers пуст | 0 | Нет запущенного disposable PostgreSQL или Azurite container для DATA-02 |
| Inventory SQL validation | `DATA_02_INVENTORY.sql` на migrated disposable PostgreSQL 17 | 13 metrics; 12 entity/media metrics равны 0, migration history равна 24 | 0 | Table/column names и counts-only query соответствуют текущей migrated schema |
| Source read-only guard | `CREATE TABLE` через session с `PGOPTIONS=-c default_transaction_read_only=on` | PostgreSQL отклонил DDL: read-only transaction | 1 | Документированный source session guard блокирует mutation |
| PostgreSQL backup | `pg_dump --format=custom --no-owner --no-privileges` с read-only `PGOPTIONS` | Создан readable archive: PostgreSQL 17.8, 56 TOC entries | 0 | Source-safe custom-format backup command работает |
| Empty-target guard | Counts query по `information_schema.tables` до restore | `0` public tables | 0 | Restore validation началась на пустом disposable target |
| PostgreSQL restore | `pg_restore --exit-on-error --no-owner --no-privileges` | Archive восстановлен без diagnostics | 0 | Runbook восстанавливает schema/data в empty non-production target |
| Entity count reconciliation | Один SQL inventory на source и target, затем `diff -u` | Различий нет | 0 | Restore сохранил все проверяемые counts и migration history в disposable rehearsal |
| Target URL rewrite | `DATA_02_REWRITE_MEDIA_URLS.sql` на семи rows по всем media URL fields | Каждый `UPDATE` затронул 1 row; source-prefix count `0`, target-prefix count `7`; unrelated social URL не изменён | 0 | Restored DB может быть переключена на test Blob hosts без source DB access или broad URL replacement |
| URL rewrite guard | Тот же script с одинаковыми source/target photo bases | Script остановился до `BEGIN` | 3 | Ошибочная/no-op prefix configuration не изменяет target data |
| Runbook shell syntax | Извлечение всех `bash` blocks и `bash -n` | Syntax errors отсутствуют | 0 | Документированные shell blocks синтаксически согласованы |
| Azure CLI contract | `az version` и `--help` для `blob list`, `download-batch`, `upload-batch`, `container create` | Azure CLI 2.79.0; все команды/options разрешаются | 0 | Документированные Azure command names/options существуют в установленном CLI; network behavior не доказано |
| Disposable cleanup | `docker stop`, `docker ps --all` по exact harness names и удаление task-owned `/tmp` artifacts | Оба containers остановлены и auto-removed; итоговые списки пусты; harness files удалены | 0 | Временные PostgreSQL source/target и локальные rehearsal artifacts не оставлены |
| Third blocked audit | Повторный поиск backup/export/authorization, TCP probes local DB endpoints и `docker ps` | Новых artifacts/authorization нет; оба endpoints недоступны; containers отсутствуют | 0 | `OPEN-002` остаётся тем же внешним blocker после трёх последовательных goal turns |
| Tracked diff whitespace | `git diff --check` | Ошибок нет | 0 | Existing tracked files не изменены и не содержат task whitespace defects |
| New-file whitespace | `git diff --no-index --check` и trailing-whitespace scan для четырёх новых файлов | Диагностика отсутствует; scans не нашли whitespace defects | 1 из-за ожидаемого new-file diff / 1 из-за отсутствия совпадений | Новые untracked DATA-02 artifacts не содержат whitespace defects |
| Secret scan | Поиск private-key headers, assigned connection passwords/account keys и PostgreSQL credential URLs в четырёх новых файлах | Совпадений нет | 1 | DATA-02 artifacts не содержат распознанных secret values |
| Итоговый scope/status | Полный no-index diff и `git status --short --untracked-files=all` | Изменения ограничены четырьмя DATA-02 artifacts; backlog/state не изменены | 1 для ожидаемых new-file diffs / 0 | Следующая задача и commit не начаты |

Первый sandboxed `docker ps` не получил доступ к local Docker socket и завершился exit 1. Read-only команда была повторена с разрешённым доступом и завершилась exit 0; containers отсутствовали.

Первая sandboxed EF migration попытка не получила loopback access и завершилась exit 1; команда была повторена с разрешённым local-network access и завершилась exit 0. Первая rehearsal-команда передала Npgsql semicolon connection string в `psql` и завершилась exit 2 с `invalid connection option "Host"`; runbook исправлен на libpq DSNs, после чего inventory/backup/restore завершились exit 0.

## Невыполненные проверки

- Backup существующей PostgreSQL не создан и restore rehearsal на реальном content не выполнен: отсутствуют разрешённый source и разрешение на чтение. Выполнена только disposable tooling rehearsal.
- Entity counts реального content не получены и не сверены: disposable empty-schema rehearsal проверяет tooling, но не acceptance на существующих данных.
- Blob inventory, copy в development container и sampled blob checks не выполнены: нет разрешённого source/export и доступного development copy.
- Публичные URLs в test environment не проверены: test content/media environment не существует.
- Backup restore rehearsal из `06_VALIDATION_PLAN.md` на существующих данных не выполнен по той же причине; disposable rehearsal не заменяет этот gate.
- Проверки затронутых application modules не запускались: application code не менялся.

## Проблемы вне scope

- `OPEN-003`, `OPEN-004` и `OPEN-008` остаются открытыми. Они могут быть исследованы после получения corpus, но не устранялись в DATA-02.
- Blob adapter создаёт containers и выполняет upload/delete, но отдельного read-only inventory/export workflow в application code нет. Это наблюдение не является текущим blocker: способ backup должен определяться фактически предоставленным Azure backup/export и разрешением владельца.

## Доказанный blocker и требуемое решение владельца

Для продолжения требуется один безопасный входной вариант:

1. Пути к актуальным PostgreSQL backup и Blob export/inventory, подтверждение что они не содержат недопустимые secrets, и явное разрешение использовать их для DATA-02; либо
2. Явное разрешение на read-only доступ к указанным PostgreSQL/Azure Blob source resources, а также подтверждённые non-production PostgreSQL и Blob/Azurite targets для restore/copy.

Connection strings и credentials должны передаваться вне Git и не включаться в evidence или logs.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Сделан backup существующих DB/Blob данных | Не выполнен — blocked | PostgreSQL tooling проверен только на disposable DB; реальный backup/export отсутствует; разрешение владельца не зафиксировано |
| Выполнены inventory и non-production restore | Не выполнен — blocked | Нет разрешённого source и доступного test target |
| Количество сущностей/media сверено | Не выполнен — blocked | Query/reconciliation проверены на disposable schema; real source/copy data недоступны |
| Публичные URLs доступны в test environment | Не выполнен — blocked | Test content/media environment не создано |
| Production data не изменены; secrets не скопированы | Выполнен | Non-local settings не использовались; secret values не выводились и не добавлялись в Git |

Все критерии приёмки DATA-02 не выполнены. Задача остаётся `planned`; `09_STATE.md` не обновлялся до принятия задачи; следующая backlog-задача не начиналась.
