# Явный workflow EF migrations

## Назначение и границы

Этот runbook создаёт или обновляет PostgreSQL schema отдельной EF Core командой. API startup migrations не применяет. Workflow подтверждён на пустой локальной PostgreSQL; production и существующие данные не затрагивались.

Не выполняйте команду для production или другой общей БД без явного разрешения владельца, актуального backup и подтверждённого rollback plan. `DB_CLOUD` является секретом: задавайте его через process environment или approved secret manager, не вставляйте значение в Git, документацию или logs.

## Предусловия

1. Установлен .NET 9 SDK.
2. Target PostgreSQL подтверждён как отдельная non-production БД. Для непустой БД дополнительно подтверждены backup и право на изменение.
3. В текущем process environment задан `DB_CLOUD` с Npgsql connection string выбранного target. Infrastructure design-time factory намеренно не загружает API `.env` и не требует JWT, CORS, Blob или Hangfire configuration.
4. Команды выполняются из корня repository.

## Подготовка tooling и build

```bash
dotnet tool restore
dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln
dotnet build backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj --no-restore --configuration Release --disable-build-servers
```

Локальный manifest фиксирует `dotnet-ef` 9.0.7. Перед изменением schema можно проверить доступные migrations без подключения:

```bash
dotnet ef migrations list \
  --project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --startup-project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --context StoronnimVContext \
  --no-connect
```

## Применение migrations

```bash
dotnet ef database update \
  --project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --startup-project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --context StoronnimVContext
```

Ожидаемый результат для пустой БД: применены все 26 migrations от `20241125211724_Initial` до `20260717233000_EnforceAdminLoginUniqueness`, команда завершается с exit code 0. `20260715012000_EnforceGroupPageSingleton` создаёт unique singleton index; если в `GroupPages` уже больше одной строки, она завершается явной ошибкой до изменения schema и не удаляет данные. Последняя migration создаёт unique `Admins.Login` index; при существующих duplicate logins она также останавливается до изменения schema и не удаляет данные.

Повторите ту же команду без изменения `DB_CLOUD`. Ожидаемый результат: `No migrations were applied. The database is already up to date.` и exit code 0.

## Проверка schema

Проверьте соответствие текущей модели последней migration:

```bash
dotnet ef migrations has-pending-model-changes \
  --project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --startup-project backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj \
  --context StoronnimVContext
```

Ожидаемый результат: `No changes have been made to the model since the last migration.` и exit code 0.

Через `psql` или эквивалентный read-only PostgreSQL client проверьте:

```sql
SELECT "MigrationId"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;
```

Ожидаются 26 строк history и таблицы `Admins`, `GroupPages`, `GroupSocials`, `Members`, `MusicPlatforms`, `NewsItems`, `Schedules`, `Socials`, `Videos`, а также `__EFMigrationsHistory`.

## Остановка при несоответствии

Не исправляйте target вручную и не удаляйте schema. Остановитесь, сохраните обезличенную ошибку и проверьте target, backup, migration history, выбранные `--project`/`--startup-project` и `DB_CLOUD`. Не используйте API project как startup project для EF: migration workflow специально изолирован от application startup и его внешних зависимостей.
