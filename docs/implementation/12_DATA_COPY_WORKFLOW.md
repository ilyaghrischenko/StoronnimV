# Безопасный workflow копирования content и media

## Назначение и границы

Этот runbook выполняет `DATA-02`: создаёт read-only PostgreSQL backup и Blob file backup, восстанавливает их только в подтверждённые non-production targets, сверяет counts/checksums и проверяет выборочные public media URLs. По `DEC-017` локальный milestone использует deterministic fixture в disposable PostgreSQL/Azurite; импорт реального production content выполняется отдельно перед `M5`.

Для production mode workflow нельзя запускать, пока владелец не закрыл `OPEN-002`: должны быть явно подтверждены source resources, право на чтение, non-production targets и допустимый ACL test Blob containers. Source PostgreSQL и Blob используются только для чтения. Для local fixture mode source создаётся специально для `DATA-02`, после seed переводится в логически read-only backup stage и не изменяется. Значения connection strings не сохраняются в repository и не включаются в evidence/logs.

## Local fixture mode (`DEC-017`)

Режим предназначен только для `M1`–`M4`. Он создаёт два PostgreSQL 17 containers и два Azurite Blob instances, привязанные к localhost; ни один remote endpoint не используется. Нужны Docker Desktop, .NET 9/local `dotnet-ef`, Azure CLI, `jq`, `ffmpeg` и `openssl`.

Создайте disposable services. Ключи Azurite генерируются на каждый запуск и остаются только в process environment:

```bash
export DATA02_DIR="${TMPDIR:-/tmp}/storonnimv-data02-local"
export POSTGRES_CLIENT_IMAGE=postgres:17
export AZURITE_IMAGE=mcr.microsoft.com/azure-storage/azurite:latest
export SOURCE_PG_CONTAINER=storonnimv-data02-pg-source
export TARGET_PG_CONTAINER=storonnimv-data02-pg-target
export SOURCE_BLOB_CONTAINER=storonnimv-data02-blob-source
export TARGET_BLOB_CONTAINER=storonnimv-data02-blob-target
mkdir -p "$DATA02_DIR/source-azurite" "$DATA02_DIR/target-azurite"

docker run --rm --detach --name "$SOURCE_PG_CONTAINER" \
  --env POSTGRES_USER=data02 --env POSTGRES_PASSWORD=data02-local-only \
  --env POSTGRES_DB=storonnimv_source --publish 127.0.0.1::5432 "$POSTGRES_CLIENT_IMAGE"
docker run --rm --detach --name "$TARGET_PG_CONTAINER" \
  --env POSTGRES_USER=data02 --env POSTGRES_PASSWORD=data02-local-only \
  --env POSTGRES_DB=storonnimv_target --publish 127.0.0.1::5432 "$POSTGRES_CLIENT_IMAGE"

export SOURCE_PG_PORT="$(docker port "$SOURCE_PG_CONTAINER" 5432/tcp | sed 's/.*://')"
export TARGET_PG_PORT="$(docker port "$TARGET_PG_CONTAINER" 5432/tcp | sed 's/.*://')"
export SOURCE_PG_DSN="postgresql://data02:data02-local-only@host.docker.internal:$SOURCE_PG_PORT/storonnimv_source?sslmode=disable"
export TARGET_PG_DSN="postgresql://data02:data02-local-only@host.docker.internal:$TARGET_PG_PORT/storonnimv_target?sslmode=disable"
export SOURCE_NPGSQL="Host=127.0.0.1;Port=$SOURCE_PG_PORT;Database=storonnimv_source;Username=data02;Password=data02-local-only;SSL Mode=Disable"

until docker exec "$SOURCE_PG_CONTAINER" pg_isready --username data02 --dbname storonnimv_source; do sleep 1; done
until docker exec "$TARGET_PG_CONTAINER" pg_isready --username data02 --dbname storonnimv_target; do sleep 1; done

export SOURCE_AZURITE_KEY="$(openssl rand -base64 32)"
export TARGET_AZURITE_KEY="$(openssl rand -base64 32)"
docker run --rm --detach --name "$SOURCE_BLOB_CONTAINER" \
  --env "AZURITE_ACCOUNTS=data02source:$SOURCE_AZURITE_KEY" \
  --publish 127.0.0.1::10000 --volume "$DATA02_DIR/source-azurite:/data" \
  "$AZURITE_IMAGE" azurite-blob --blobHost 0.0.0.0 --blobPort 10000 --location /data
docker run --rm --detach --name "$TARGET_BLOB_CONTAINER" \
  --env "AZURITE_ACCOUNTS=data02target:$TARGET_AZURITE_KEY" \
  --publish 127.0.0.1::10000 --volume "$DATA02_DIR/target-azurite:/data" \
  "$AZURITE_IMAGE" azurite-blob --blobHost 0.0.0.0 --blobPort 10000 --location /data

export SOURCE_BLOB_PORT="$(docker port "$SOURCE_BLOB_CONTAINER" 10000/tcp | sed 's/.*://')"
export TARGET_BLOB_PORT="$(docker port "$TARGET_BLOB_CONTAINER" 10000/tcp | sed 's/.*://')"
export SOURCE_BLOB_CONNECTION_STRING="DefaultEndpointsProtocol=http;AccountName=data02source;AccountKey=$SOURCE_AZURITE_KEY;BlobEndpoint=http://127.0.0.1:$SOURCE_BLOB_PORT/data02source;"
export TARGET_BLOB_CONNECTION_STRING="DefaultEndpointsProtocol=http;AccountName=data02target;AccountKey=$TARGET_AZURITE_KEY;BlobEndpoint=http://127.0.0.1:$TARGET_BLOB_PORT/data02target;"

until docker logs "$SOURCE_BLOB_CONTAINER" 2>&1 | grep -q 'Azurite Blob service is successfully listening'; do sleep 1; done
until docker logs "$TARGET_BLOB_CONTAINER" 2>&1 | grep -q 'Azurite Blob service is successfully listening'; do sleep 1; done
```

Примените все migrations к source, затем один раз создайте fixture и проверьте его. Fixture намеренно не содержит admin credentials; повторный seed на непустой source должен завершиться ошибкой.

```bash
dotnet tool restore
dotnet ef database update \
  --project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --startup-project backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj \
  --connection "$SOURCE_NPGSQL"

export FIXTURE_SQL="$PWD/docs/implementation/sql/DATA_02_LOCAL_FIXTURE.sql"
export FIXTURE_ASSERT_SQL="$PWD/docs/implementation/sql/DATA_02_LOCAL_FIXTURE_ASSERT.sql"
export SOURCE_PHOTO_BASE_URL="http://127.0.0.1:$SOURCE_BLOB_PORT/data02source/storonnimv-photo"
export SOURCE_VIDEO_BASE_URL="http://127.0.0.1:$SOURCE_BLOB_PORT/data02source/storonnimv-video"

docker run --rm --env SOURCE_PG_DSN --env SOURCE_PHOTO_BASE_URL --env SOURCE_VIDEO_BASE_URL \
  --mount "type=bind,source=$FIXTURE_SQL,target=/fixture.sql,readonly" "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$SOURCE_PG_DSN" --set ON_ERROR_STOP=1 \
    --set photo_base="$SOURCE_PHOTO_BASE_URL" --set video_base="$SOURCE_VIDEO_BASE_URL" \
    --file=/fixture.sql'
docker run --rm --env SOURCE_PG_DSN --env SOURCE_PHOTO_BASE_URL --env SOURCE_VIDEO_BASE_URL \
  --mount "type=bind,source=$FIXTURE_ASSERT_SQL,target=/fixture-assert.sql,readonly" "$POSTGRES_CLIENT_IMAGE" \
  sh -c 'exec psql --dbname="$SOURCE_PG_DSN" --set ON_ERROR_STOP=1 \
    --set photo_base="$SOURCE_PHOTO_BASE_URL" --set video_base="$SOURCE_VIDEO_BASE_URL" \
    --file=/fixture-assert.sql'
```

Для media fixture используйте существующий tracked JPEG и сгенерированный реальный MP4. Создайте source containers с public `blob` ACL и загрузите файлы с явными content types, затем продолжите общими разделами inventory/backup/restore ниже.

```bash
cp frontend/storonnimv.client/src/assets/default-news-photo.jpg "$DATA02_DIR/data-02-photo.jpg"
ffmpeg -hide_banner -loglevel error -f lavfi -i color=c=black:s=320x180:d=1 \
  -c:v libx264 -pix_fmt yuv420p -movflags +faststart -y "$DATA02_DIR/data-02-promotion.mp4"

for container in storonnimv-photo storonnimv-video; do
  AZURE_STORAGE_CONNECTION_STRING="$SOURCE_BLOB_CONNECTION_STRING" \
    az storage container create --name "$container" --public-access blob --output none
done
AZURE_STORAGE_CONNECTION_STRING="$SOURCE_BLOB_CONNECTION_STRING" az storage blob upload \
  --container-name storonnimv-photo --name data-02-photo.jpg \
  --file "$DATA02_DIR/data-02-photo.jpg" --content-type image/jpeg --overwrite false --output none
AZURE_STORAGE_CONNECTION_STRING="$SOURCE_BLOB_CONNECTION_STRING" az storage blob upload \
  --container-name storonnimv-video --name data-02-promotion.mp4 \
  --file "$DATA02_DIR/data-02-promotion.mp4" --content-type video/mp4 --overwrite false --output none
```

В local mode задайте `TARGET_BLOB_PUBLIC_ACCESS=blob`. После target URL rewrite повторите `DATA_02_LOCAL_FIXTURE_ASSERT.sql` с target bases и потребуйте HTTP 200 для всех семи media fields. Реальные DB/Blob данные этим режимом не имитируются и не считаются импортированными.

## Предусловия

1. Известна major version source PostgreSQL и выбран совместимый client image `postgres:<major>`. Более старый `pg_dump` использовать нельзя.
2. `SOURCE_PG_DSN` указывает на разрешённый read-only source, `TARGET_PG_DSN` — на отдельную пустую non-production БД. Используйте libpq URI/conninfo, принимаемый `psql`/`pg_dump`, а не Npgsql semicolon connection string из `DB_CLOUD`.
3. `SOURCE_BLOB_CONNECTION_STRING` разрешает только чтение source containers. `TARGET_BLOB_CONNECTION_STRING` указывает на отдельный development account/emulator.
4. Владелец отдельно подтвердил, можно ли делать development containers публичными. Без public read acceptance criterion для URLs не выполнен.
5. Docker, Azure CLI и `jq` доступны. Для local fixture mode дополнительно нужны .NET 9/local `dotnet-ef`, `ffmpeg` и `openssl`. Команды выполняются из корня repository.
6. Backup directory находится вне repository, имеет достаточно места и защищён от чтения другими local users.

```bash
umask 077
export DATA02_DIR="${DATA02_DIR:?Set DATA02_DIR to an absolute non-repository path}"
export INVENTORY_SQL="$PWD/docs/implementation/sql/DATA_02_INVENTORY.sql"
export REWRITE_SQL="$PWD/docs/implementation/sql/DATA_02_REWRITE_MEDIA_URLS.sql"
export POSTGRES_CLIENT_IMAGE="${POSTGRES_CLIENT_IMAGE:-postgres:17}"
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
export TARGET_BLOB_PUBLIC_ACCESS="${TARGET_BLOB_PUBLIC_ACCESS:-off}"

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

Не удаляйте backup до принятия evidence. После принятия local fixture evidence остановите только четыре exact disposable containers из local section и удалите только `DATA02_DIR`. В production mode source DB/Blob никогда не очищаются этим workflow.
