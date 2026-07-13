# DATA-02 — Local content and media copy evidence

## Цель

По принятому `DEC-017` закрыть локальный `DATA-02` на disposable PostgreSQL 17, Azurite и deterministic test data: создать backup, восстановить его в отдельный target, скопировать media, сверить counts/content metadata/checksums и проверить public media URLs. Реальный production content явно отложен до `OPS-03`/`M5`.

## Решение владельца и границы

- 13 июля 2026 года владелец сообщил, что Azure account намеренно временно ограничен, старая БД удалена и remote state сейчас нужно игнорировать.
- Владелец явно утвердил закрытие `DATA-02` на локальных PostgreSQL, Azurite и тестовых данных; реальные данные отложены.
- Решение зафиксировано как `DEC-017` и заменяет прежний `DEC-006` для `M1`–`M4`.
- Remote DB/Blob не читались и не изменялись в финальном local run. Secrets, connection strings, fixture bytes и backup artifacts не добавлялись в Git.
- `QA-01` и последующие backlog tasks не начинались. Коммит не создавался.

## Затронутые файлы

| Файл | Изменение |
|---|---|
| `docs/implementation/sql/DATA_02_LOCAL_FIXTURE.sql` | Deterministic test corpus без admin credentials |
| `docs/implementation/sql/DATA_02_LOCAL_FIXTURE_ASSERT.sql` | Исполняемые assertions для counts, URLs, video types и blob names |
| `docs/implementation/12_DATA_COPY_WORKFLOW.md` | Воспроизводимый local fixture mode и сохранённый production-safe mode |
| `docs/implementation/evidence/DATA-02.md` | Фактические команды, результаты и acceptance evidence |
| `docs/implementation/00_INDEX.md`–`10_RUNTIME_CONTRACT.md` | Синхронизированы `DEC-017`, status, milestone, validation, traceability, open item и next task |
| `backend/AGENTS.md` | Добавлено требуемое project learning после коррекции владельца |

Application code и configuration не менялись.

## Test corpus

Source получил все 24 существующие EF migrations и следующий минимальный corpus:

| Entity/metric | Ожидается | Получено |
|---|---:|---:|
| `Admins` | 0 | 0 |
| `GroupPages` | 1 | 1 |
| `GroupSocials` | 1 | 1 |
| `Members` | 1 | 1 |
| `MusicPlatforms` | 1 | 1 |
| `NewsItems` | 1 | 1 |
| `Schedules` | 1 | 1 |
| `Socials` | 1 | 1 |
| `Videos` | 4 | 4 |
| Media URL references | 10 | 10 |
| Distinct media URLs | 2 | 2 |

Четыре `Videos` покрывают все enum types `0..3` и используют один `data-02-promotion.mp4`. Шесть photo references используют один `data-02-photo.jpg`. JPEG взят из tracked `frontend/storonnimv.client/src/assets/default-news-photo.jpg`; MP4 сгенерирован `ffmpeg` как реальный односекундный H.264/yuv420p файл.

## Выполненные проверки

Команды не выводили credentials или connection strings.

| Проверка | Результат | Exit code | Что доказано |
|---|---|---:|---|
| Azurite image | `mcr.microsoft.com/azure-storage/azurite:latest` получен; digest `sha256:647c63a91102a9d8e8000aab803436e1fc85fbb285e7ce830a82ee5d6661cf37` | 0 | Для local run использован официальный emulator image |
| Disposable topology | Два PostgreSQL 17 и два Azurite Blob instances, все ports привязаны только к `127.0.0.1` | 0 | Source и target физически разделены; remote resources не нужны |
| RED: empty DB entity gate | Проверка nonempty fixture на migrated empty source завершилась `DATA-02 fixture entity gate failed` | 1 | Acceptance gate действительно отклоняет пустую schema |
| RED: empty Blob gate | List отсутствующего `storonnimv-photo` вернул `ContainerNotFound` | 3 | Blob gate действительно отклоняет отсутствующий corpus |
| RED: committed assertion SQL | `DATA_02_LOCAL_FIXTURE_ASSERT.sql` на empty source остановился на assertion | 3 | Проверяемый test существовал и падал до seed |
| EF migrations | Все 24 migrations применены к source PostgreSQL | 0 | Fixture создаётся на актуальной schema |
| Fixture seed | Insert counts: `1,1,1,1,1,4,1,1`; persistent assertions прошли | 0 | Все nine entity tables и media reference invariants соответствуют corpus contract |
| Source Blob fixture | `data-02-photo.jpg`: 22 697 bytes, `image/jpeg`; `data-02-promotion.mp4`: 3 666 bytes, `video/mp4` | 0 | Source содержит реальные bytes обоих необходимых media types |
| Source read-only guard | `CREATE TABLE` при `default_transaction_read_only=on` отклонён PostgreSQL | 1 | Backup stage защищён от source mutation |
| Counts-only source inventory | 13 metrics получены через `DATA_02_INVENTORY.sql` | 0 | Source inventory не раскрывает content/URLs/credentials |
| PostgreSQL backup | Custom-format `pg_dump`; archive 19 743 bytes; restore list 67 lines | 0 | Backup создан и читается `pg_restore` |
| Empty target guard | До restore target содержал 0 public tables | 0 | Existing target data не перезаписывались |
| PostgreSQL restore | `pg_restore --exit-on-error --no-owner --no-privileges` завершён без diagnostics | 0 | Backup восстановлен в отдельную empty target DB |
| DB reconciliation | `diff -u` source/target inventories до URL rewrite и counts после rewrite | 0 | Restore и target-only rewrite сохранили entity/media counts |
| Blob backup/copy | Source list/download и target create/upload выполнены; по одному объекту в photo/video containers | 0 | Оба media types скопированы в отдельный target Azurite |
| Blob metadata reconciliation | Source/target name, size и content type inventories совпали | 0 | Copy сохранила object metadata |
| Blob byte reconciliation | Source/target SHA-256 files совпали для обоих containers | 0 | Copy сохранила bytes |
| Target URL rewrite | Updates: `GroupPages=1`, `GroupSocials=1`, `Members=1`, `MusicPlatforms=1`, `NewsItems=1`, `Schedules=1`, `Videos=4` | 0 | Только restored target переключён на target Azurite bases |
| Target fixture assertions | Все assertions повторно прошли с target bases | 0 | Target corpus соответствует тому же data contract |
| Fixture rerun guard | Повторный запуск seed на заполненном source остановился до transaction | 3, ожидаемый | Fixture не дублирует и не перезаписывает существующий corpus |
| Public URL samples | Все семь media fields вернули HTTP 200; photos — `image/jpeg`, video — `video/mp4` | 0 | Каждый используемый media field доступен через public target Blob URL |
| Target video validation | `ffprobe`: format `mov,mp4,m4a,3gp,3g2,mj2`, duration `1.000000` | 0 | Video object является валидным MP4, а не пустой заглушкой |
| Runbook/static checks | Все `bash` blocks прошли `bash -n`; `git diff --check` не выдал diagnostics; targeted secret scan не нашёл совпадений | 0 | Workflow синтаксически валиден, diff не содержит whitespace defects или распознанных secret values |
| Disposable cleanup | Четыре exact containers остановлены и auto-removed; task-owned `/tmp` artifacts удалены; container filter пуст | 0 | Локальный прогон не оставил services, backup или media bytes |

## История устранённого blocker

Предыдущие проверки доказали, что старые PostgreSQL/Azure/public endpoints недоступны (`NXDOMAIN`), а repository/Git history не содержит пригодного real backup. Это не исправляется локальным кодом. После явного решения владельца данное состояние перестало блокировать `DATA-02`: local acceptance переведён на fixture, а выбор источника real production content сохранён в `OPEN-002` с deadline `OPS-03`/`M5`.

Internet Archive probe ранее завершился timeout и не считается ни источником, ни отрицательным доказательством.

## Невыполненные проверки

- Реальный production DB/Blob backup/import не выполнялся по прямому решению владельца. Это отдельный gate `OPEN-002` → `OPS-03`/`M5`, а не критерий локального `DATA-02`.
- Application build/tests/browser smoke не запускались в этой задаче: application code не менялся; public route smoke является следующей отдельной задачей `QA-01`.

## Итог по критериям приёмки

| Критерий `DEC-017` | Итог | Evidence |
|---|---|---|
| Создан local PostgreSQL/Azurite test corpus | Выполнен | 9 entity tables, 10 media references, JPEG и MP4 |
| PostgreSQL backup восстановлен в отдельный target | Выполнен | Dump 19 743 bytes, 67 TOC lines, restore exit 0 |
| Blob media скопированы в отдельный target | Выполнен | 1 photo + 1 video, metadata и SHA-256 совпали |
| Entity/media counts сверены | Выполнен | Source/target inventories и fixture assertions совпали |
| Public target URLs доступны | Выполнен | 7/7 fields вернули HTTP 200 с ожидаемым content type |
| Remote production resources не изменены; secrets не сохранены | Выполнен | Финальный run полностью localhost; secret values отсутствуют в tracked artifacts |

`DATA-02` выполнена и имеет статус `done`. Следующая задача — `QA-01`; она не начиналась.
