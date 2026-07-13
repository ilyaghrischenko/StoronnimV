# Безопасный workflow копирования content и media

## Назначение и границы

Этот runbook выполняет `DATA-02`: создаёт read-only PostgreSQL backup и Blob file backup, восстанавливает их только в подтверждённые non-production targets, сверяет counts/checksums и проверяет выборочные public media URLs.

Workflow нельзя запускать, пока владелец не закрыл `OPEN-002`: должны быть явно подтверждены source resources, право на чтение, non-production targets и допустимый ACL test Blob containers. Source PostgreSQL и Blob используются только для чтения. Значения connection strings не передаются аргументами команд, не сохраняются в repository и не включаются в evidence/logs.

## Предусловия

1. Известна major version source PostgreSQL и выбран совместимый client image `postgres:<major>`. Более старый `pg_dump` использовать нельзя.
2. `SOURCE_PG_DSN` указывает на разрешённый read-only source, `TARGET_PG_DSN` — на отдельную пустую non-production БД. Используйте libpq URI/conninfo, принимаемый `psql`/`pg_dump`, а не Npgsql semicolon connection string из `DB_CLOUD`.
3. `SOURCE_BLOB_CONNECTION_STRING` разрешает только чтение source containers. `TARGET_BLOB_CONNECTION_STRING` указывает на отдельный development account/emulator.
4. Владелец отдельно подтвердил, можно ли делать development containers публичными. Без public read acceptance criterion для URLs не выполнен.
5. Docker, Azure CLI и `jq` доступны. Команды выполняются из корня repository.
6. Backup directory находится вне repository, имеет достаточно места и защищён от чтения другими local users.

```bash
umask 077
export DATA02_DIR=/absolute/non-repository/path/data-02
export INVENTORY_SQL="$PWD/docs/implementation/sql/DATA_02_INVENTORY.sql"
export REWRITE_SQL="$PWD/docs/implementation/sql/DATA_02_REWRITE_MEDIA_URLS.sql"
export POSTGRES_CLIENT_IMAGE=postgres:17
export AZURE_CONFIG_DIR="$DATA02_DIR/azure-cli"
mkdir -p "$DATA02_DIR" "$AZURE_CONFIG_DIR"
test -n "$SOURCE_PG_DSN"
test -n "$TARGET_PG_DSN"
test -n "$SOURCE_BLOB_CONNECTION_STRING"
test -n "$TARGET_BLOB_CONNECTION_STRING"
```

`postgres:17` выше — пример для подтверждённого PostgreSQL 17 source, а не версия проекта по умолчанию.

## PostgreSQL source inventory и backup

Сначала получите counts-only inventory. `PGOPTIONS` дополнительно запрещает записи source session:

```bash
docker run --rm \
  --env SOURCE_PG_DSN \
  --env PGOPTIONS='-c default_transaction_read_only=on' \
  --mount "type=bind,source=$INVENTORY_SQL,target=/inventory.sql,readonly" \
  "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$SOURCE_PG_DSN" --set ON_ERROR_STOP=1 --csv --file=/inventory.sql' \
  > "$DATA02_DIR/source-db-inventory.csv"
```

Inventory содержит только counts: девять application entities, migration history, media URL references и `Videos.BlobName`. Он не выводит logins, password hashes, content text или URLs.

Создайте custom-format backup и checksum:

```bash
docker run --rm \
  --env SOURCE_PG_DSN \
  --env PGOPTIONS='-c default_transaction_read_only=on' \
  --mount "type=bind,source=$DATA02_DIR,target=/work" \
  "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec pg_dump --dbname="$SOURCE_PG_DSN" --format=custom --no-owner --no-privileges --file=/work/postgresql.dump'

shasum -a 256 "$DATA02_DIR/postgresql.dump" > "$DATA02_DIR/postgresql.dump.sha256"
```

Остановитесь при warning/error `pg_dump`. Не используйте partial dump как backup.

## PostgreSQL non-production restore

Подтвердите, что target пуст. Результат должен быть `0`:

```bash
docker run --rm --env TARGET_PG_DSN "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$TARGET_PG_DSN" --set ON_ERROR_STOP=1 --tuples-only --no-align --command="SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '\''public'\'';"'
```

Если результат не `0`, остановитесь: workflow не очищает и не перезаписывает существующую БД.

Проверьте archive и восстановите его в пустой target:

```bash
docker run --rm \
  --mount "type=bind,source=$DATA02_DIR,target=/work,readonly" \
  "$POSTGRES_CLIENT_IMAGE" \
  pg_restore --list /work/postgresql.dump > "$DATA02_DIR/postgresql.restore-list.txt"

docker run --rm \
  --env TARGET_PG_DSN \
  --mount "type=bind,source=$DATA02_DIR,target=/work,readonly" \
  "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec pg_restore --dbname="$TARGET_PG_DSN" --exit-on-error --no-owner --no-privileges /work/postgresql.dump'
```

Получите target inventory той же SQL-командой и потребуйте точное совпадение:

```bash
docker run --rm \
  --env TARGET_PG_DSN \
  --mount "type=bind,source=$INVENTORY_SQL,target=/inventory.sql,readonly" \
  "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$TARGET_PG_DSN" --set ON_ERROR_STOP=1 --csv --file=/inventory.sql' \
  > "$DATA02_DIR/target-db-inventory.csv"

diff -u "$DATA02_DIR/source-db-inventory.csv" "$DATA02_DIR/target-db-inventory.csv"
```

`diff` должен завершиться exit code 0. `entity.GroupPages` отдельно фиксируется для `OPEN-004`; значение больше `1` не исправляется автоматически.

## Blob source inventory и file backup

Код использует только containers `storonnimv-photo` и `storonnimv-video`. Получите полный inventory с именем, размером и content type, затем скачайте bytes:

```bash
for container in storonnimv-photo storonnimv-video; do
  mkdir -p "$DATA02_DIR/source-blob/$container"

  AZURE_STORAGE_CONNECTION_STRING="$SOURCE_BLOB_CONNECTION_STRING" \
    az storage blob list \
      --container-name "$container" \
      --num-results '*' \
      --query 'sort_by([].{name:name,size:properties.contentLength,contentType:properties.contentSettings.contentType}, &name)' \
      --output json \
      > "$DATA02_DIR/source-$container.json"

  AZURE_STORAGE_CONNECTION_STRING="$SOURCE_BLOB_CONNECTION_STRING" \
    az storage blob download-batch \
      --source "$container" \
      --destination "$DATA02_DIR/source-blob/$container" \
      --no-progress

  (cd "$DATA02_DIR/source-blob/$container" && find . -type f -exec shasum -a 256 {} + | sort) \
    > "$DATA02_DIR/source-$container.sha256"
done
```

Не добавляйте Blob inventory или downloaded media в Git: имена и содержимое могут быть чувствительными.

## Blob non-production restore

Для каждого container создайте новый target container. `TARGET_BLOB_PUBLIC_ACCESS` должен быть отдельно подтверждён как `blob` для public URL acceptance либо `off` для закрытой промежуточной проверки:

```bash
export TARGET_BLOB_PUBLIC_ACCESS=off

for container in storonnimv-photo storonnimv-video; do
  AZURE_STORAGE_CONNECTION_STRING="$TARGET_BLOB_CONNECTION_STRING" \
    az storage container create \
      --name "$container" \
      --public-access "$TARGET_BLOB_PUBLIC_ACCESS" \
      --fail-on-exist \
      --output none

  AZURE_STORAGE_CONNECTION_STRING="$TARGET_BLOB_CONNECTION_STRING" \
    az storage blob upload-batch \
      --destination "$container" \
      --source "$DATA02_DIR/source-blob/$container" \
      --overwrite false \
      --no-progress

  AZURE_STORAGE_CONNECTION_STRING="$TARGET_BLOB_CONNECTION_STRING" \
    az storage blob list \
      --container-name "$container" \
      --num-results '*' \
      --query 'sort_by([].{name:name,size:properties.contentLength,contentType:properties.contentSettings.contentType}, &name)' \
      --output json \
      > "$DATA02_DIR/target-$container.json"
done
```

`upload-batch` может определить content type иначе, чем source. Сравните inventory; несовпадение name, size или content type блокирует принятие и должно быть исправлено на target без изменения source:

```bash
for container in storonnimv-photo storonnimv-video; do
  diff -u \
    <(jq -S . "$DATA02_DIR/source-$container.json") \
    <(jq -S . "$DATA02_DIR/target-$container.json")
done
```

Для byte-level проверки скачайте target в отдельный каталог, вычислите checksums той же командой и сравните `source-*.sha256` с target checksum files.

## URL sampling и итоговая сверка

После успешной Blob сверки замените source container bases только в restored non-production DB. Значения задаются без завершающего `/`; script останавливается при пустых/одинаковых bases и меняет только URLs с точным source prefix:

```bash
export SOURCE_PHOTO_BASE_URL=https://source.example/storonnimv-photo
export TARGET_PHOTO_BASE_URL=https://target.example/storonnimv-photo
export SOURCE_VIDEO_BASE_URL=https://source.example/storonnimv-video
export TARGET_VIDEO_BASE_URL=https://target.example/storonnimv-video

docker run --rm \
  --env TARGET_PG_DSN \
  --env SOURCE_PHOTO_BASE_URL \
  --env TARGET_PHOTO_BASE_URL \
  --env SOURCE_VIDEO_BASE_URL \
  --env TARGET_VIDEO_BASE_URL \
  --mount "type=bind,source=$REWRITE_SQL,target=/rewrite.sql,readonly" \
  "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$TARGET_PG_DSN" --set ON_ERROR_STOP=1 \
    --set source_photo_base="$SOURCE_PHOTO_BASE_URL" \
    --set target_photo_base="$TARGET_PHOTO_BASE_URL" \
    --set source_video_base="$SOURCE_VIDEO_BASE_URL" \
    --set target_video_base="$TARGET_VIDEO_BASE_URL" \
    --file=/rewrite.sql'
```

Команда выше намеренно не получает source DB connection и не может изменить source data. Просмотрите row counts каждого `UPDATE`; неожиданный `0` или count, не согласованный с inventory, блокирует принятие.

Затем выберите минимум по одному существующему DB reference для каждого используемого media field: `GroupPages.PhotoUrl`, `GroupSocials.PhotoUrl`, `Members.PhotoUrl`, `MusicPlatforms.BgImageUrl`, `NewsItems.Photo`, `Schedules.Photo`, `Videos.Url`. Не публикуйте сами URLs в evidence.

Для каждого sample выполните `curl --head --fail --location "$SAMPLE_URL"`. При невозможности `HEAD` используйте bounded GET: `curl --fail --location --range 0-0 --output /dev/null "$SAMPLE_URL"`. Запишите только field, HTTP status и content type. Все samples должны быть доступны из test environment; при `TARGET_BLOB_PUBLIC_ACCESS=off` этот критерий ожидаемо остаётся невыполненным.

Acceptance требует одновременно:

- source/target DB inventories совпадают;
- source/target Blob name, size, content type и byte checksums совпадают;
- DB media reference counts объяснимо согласованы с Blob inventory, включая external/default/unused blobs;
- sampled target URLs доступны;
- source resources не получили mutations;
- backup path, inventories и logs не добавлены в Git и не содержат credentials.

## Очистка

Не удаляйте backup до принятия evidence. После принятия удаляйте только явно подтверждённые disposable targets и local backup directory; source DB/Blob никогда не очищаются этим workflow.
