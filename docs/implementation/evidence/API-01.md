# API-01 — Authentication middleware и policies

## Цель

Сделать authentication pipeline явным и проверить решения для anonymous, Basic Admin и SuperAdmin через реальный ASP.NET Core startup.

## Исходное состояние

- `Program.cs` вызывал `UseAuthorization()`, но не вызывал `UseAuthentication()` явно.
- JWT bearer registration уже принимал token из `Authorization: Bearer` и HttpOnly cookie `Token`.
- `AdminController` требовал bearer authentication scheme; `SuperAdminController` требовал policy `SuperAdminOnly` с role `SuperAdmin`.
- Test project не имел source tests и `ProjectReference` на API.
- Runtime integration matrix показал важное уточнение к старому статическому анализу: в текущем .NET 9 protected endpoints уже могли аутентифицироваться через authorization pipeline. Реальным неподтверждённым требованием оставался явный порядок middleware и repeatable regression coverage.

## Затронутые файлы

- `backend/StoronnimV.Server/StoronnimV.Api/Program.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/StoronnimV.Tests.csproj`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/AuthenticationIntegrationTests.cs`
- `docs/implementation/00_INDEX.md`
- `docs/implementation/04_BACKLOG.md`
- `docs/implementation/09_STATE.md`
- `docs/implementation/evidence/API-01.md`

## Решения

- Добавлен только явный `app.UseAuthentication()` между CORS и `UseAuthorization()`; существующие JWT options, controller attributes и `SuperAdminOnly` policy не менялись, потому что integration matrix подтвердила их корректные решения.
- Integration tests запускают реальный API через `WebApplicationFactory` и disposable PostgreSQL 17 для обязательного Hangfire startup. Только чтение SuperAdmin list заменено test stub, чтобы проверять authorization, а не DB content.
- Test JWT key состоит только из повторяемого test character и не является credential/secret.
- Cookie/CORS/CSRF topology не менялась: это scope `API-02`.

## Выполненные изменения

1. `UseAuthentication()` явно поставлен перед `UseAuthorization()`.
2. Test project получил `Microsoft.AspNetCore.Mvc.Testing` 9.0.4 и reference на API project.
3. Добавлены 11 tests:
   - явный middleware order;
   - anonymous `401`;
   - principal из header и cookie;
   - Basic Admin access к admin endpoint;
   - Basic Admin `403` на SuperAdmin endpoint;
   - SuperAdmin `200` на SuperAdmin endpoint;
   - invalid и expired token `401`;
   - authenticated logout `200`.

## Команды и результаты

### Disposable PostgreSQL

```bash
docker run --detach --rm --name storonnimv-api01-red \
  -e POSTGRES_HOST_AUTH_METHOD=trust \
  -e POSTGRES_DB=storonnimv_api01 \
  -p 127.0.0.1::5432 postgres:17
docker port storonnimv-api01-red 5432/tcp
docker exec storonnimv-api01-red pg_isready -U postgres -d storonnimv_api01
```

Container стартовал на `127.0.0.1:64500`; `pg_isready` подтвердил `accepting connections`. Production DB не использовалась.

### TDD RED

```bash
dotnet test backend/StoronnimV.Server/StoronnimV.Tests/StoronnimV.Tests.csproj \
  --no-restore --configuration Release \
  --artifacts-path /tmp/storonnimv-api01-red/artifacts \
  --disable-build-servers \
  --filter FullyQualifiedName~AuthenticationMiddleware_IsExplicitlyBeforeAuthorization
```

Exit code `1`. Ожидаемая причина: `Program.cs must call app.UseAuthentication().`

### Focused GREEN

```bash
DB_CLOUD='Host=127.0.0.1;Port=64500;Database=storonnimv_api01;Username=postgres' \
dotnet test backend/StoronnimV.Server/StoronnimV.Tests/StoronnimV.Tests.csproj \
  --no-restore --configuration Release \
  --artifacts-path /tmp/storonnimv-api01-red/artifacts \
  --disable-build-servers \
  --filter FullyQualifiedName~AuthenticationIntegrationTests
```

Exit code `0`; `11/11` tests passed.

### Fresh final restore

```bash
DOTNET_CLI_HOME=/tmp/storonnimv-api01-final/dotnet-home \
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-api01-final/nuget-http \
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln \
  --no-cache \
  --artifacts-path /tmp/storonnimv-api01-final/artifacts \
  --disable-build-servers
```

Exit code `0`; все пять projects restored. Сохранены две существующие ImageSharp advisory warnings.

### Fresh final solution build

```bash
DOTNET_CLI_HOME=/tmp/storonnimv-api01-final/dotnet-home \
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-api01-final/nuget-http \
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln \
  --no-restore --configuration Release \
  --artifacts-path /tmp/storonnimv-api01-final/artifacts \
  --disable-build-servers
```

Exit code `0`; `0 errors`, `8 warnings` существующего baseline.

### Fresh final API build

```bash
DOTNET_CLI_HOME=/tmp/storonnimv-api01-final/dotnet-home \
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-api01-final/nuget-http \
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj \
  --no-restore --configuration Release \
  --artifacts-path /tmp/storonnimv-api01-final/artifacts \
  --disable-build-servers
```

Exit code `0`; `0 errors`, две существующие ImageSharp advisory warnings.

### Fresh final full tests

```bash
DOTNET_CLI_HOME=/tmp/storonnimv-api01-final/dotnet-home \
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-api01-final/nuget-http \
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
DB_CLOUD='Host=127.0.0.1;Port=64500;Database=storonnimv_api01;Username=postgres' \
dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln \
  --no-restore --no-build --configuration Release \
  --artifacts-path /tmp/storonnimv-api01-final/artifacts \
  --disable-build-servers
```

Exit code `0`; `11/11` tests passed, `0` failed/skipped.

### Diff и secret checks

```bash
git diff --check
```

Exit code `0`.

Changed-file scan нашёл только test identifiers/values (`token`, generated `SigningKey`, cookie header); credential values, connection passwords и production secrets отсутствуют.

### Cleanup

```bash
docker stop storonnimv-api01-red
docker ps -a --filter name=storonnimv-api01-red --format '{{.Names}}'
```

Exit code `0`; повторная проверка не вернула container name, disposable PostgreSQL удалён.

## Невыполненные проверки

- Browser cookie/CORS/CSRF flow не выполнялся: он относится к `API-02` и явно вне scope `API-01`.
- Production/staging auth не проверялся: production access не разрешён и относится к M5/M6.

## Проблемы вне scope

- Restore/build сохраняют существующие ImageSharp NU1902/NU1903 advisories.
- Solution build сохраняет шесть существующих compiler warnings помимо двух advisories.
- Исправление этих warnings и package upgrade не требуется критериями `API-01` и не выполнялось.

## Итог по критериям приёмки

| Критерий | Результат |
|---|---|
| Authentication middleware имеет явный корректный порядок | Выполнено: wiring test и source подтверждают `UseAuthentication()` перед `UseAuthorization()` |
| Principal формируется из cookie | Выполнено: integration test проверил authenticated name/role и admin `200` |
| Principal формируется из Authorization header | Выполнено: integration test проверил authenticated name/role и admin `200` |
| Unauthorized и forbidden различаются | Выполнено: anonymous/invalid/expired получили `401`, Basic на SuperAdmin endpoint получил `403` |
| Basic/SuperAdmin decisions корректны | Выполнено: Basic admin `200`, Basic SuperAdmin route `403`, SuperAdmin route `200` |

Все критерии `API-01` выполнены. Статус: `done`.
