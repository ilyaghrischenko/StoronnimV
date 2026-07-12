# BASE-02 — Clean backend build evidence

## Контекст

- Дата: 13 июля 2026 года.
- OS: macOS 26.5 (`Darwin`, arm64).
- .NET SDK: 9.0.203; это evidence машины проверки, а не project pin.
- Solution: `backend/StoronnimV.Server/StoronnimV.Server.sln`.
- Startup API project: `backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj`.
- Configuration: `Release`.
- Изоляция: отдельные пустые `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH` и `--artifacts-path` под `/tmp`; repository `bin`/`obj` не использовались.

## Исходное состояние

Первый restore solution в новом package cache завершился успешно (exit code 0) с двумя warnings `NU1902`/`NU1903` для существующей версии `SixLabors.ImageSharp` 3.1.6. Первый solution Release build и отдельный API Release build также завершились успешно: 0 errors, соответственно 8 и 2 warnings.

Несмотря на успешную сборку, активный `StoronnimV.Infrastructure.csproj` содержал machine-specific assembly reference:

`C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\9.0.0\Microsoft.Extensions.Configuration.dll`.

Этот reference дублировал существующий переносимый `PackageReference` `Microsoft.Extensions.Configuration` версии 9.0.0. Других `HintPath`, абсолютных пользовательских путей, внешних MSBuild imports, OS conditions или file references вне репозитория в backend project graph не найдено.

## Исправления

| Файл | Причина | Минимальное изменение | Граница scope |
|---|---|---|---|
| `backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj` | Активный Windows-only абсолютный `HintPath` нарушал переносимость | Удалён только дублирующий `<Reference>`/`<HintPath>`; существующий `PackageReference` 9.0.0 сохранён | Runtime behavior, target framework, project graph и package versions не менялись |

## Финальная проверка

Перед финальным restore создан новый, не использовавшийся в первом цикле каталог `/tmp/storonnimv-base02-final`. Команды ниже выполнялись с environment variables:

```text
DOTNET_CLI_HOME=/tmp/storonnimv-base02-final/cli-home
NUGET_PACKAGES=/tmp/storonnimv-base02-final/packages
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-base02-final/http-cache
DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=true
```

| Проверка | Команда | Результат | Exit code | Errors | Warnings |
|---|---|---|---:|---:|---:|
| Clean solution restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --artifacts-path /tmp/storonnimv-base02-final/artifacts --disable-build-servers` | Успешно | 0 | 0 | 2 |
| Solution Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final/artifacts --disable-build-servers` | Успешно | 0 | 0 | 8 |
| API Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final/artifacts --disable-build-servers` | Успешно | 0 | 0 | 2 |
| Machine-specific project reference scan | `rg` по backend `.csproj`/`.props`/`.targets`/`.sln` для `HintPath`, абсолютных Windows/macOS/Linux user paths и external imports | Совпадений нет | 1 | 0 | 0 |
| Whitespace validation | `git diff --check` | Успешно | 0 | 0 | 0 |

Solution warnings: два NuGet vulnerability warning для `SixLabors.ImageSharp` 3.1.6, два `CS8981` для существующей migration `migr` и четыре nullable warnings (`CS8602`, два `CS8618`, `CS8629`; `CS8618` возникает для двух properties). Они не являются build/portability blockers и не исправлялись. API build повторяет только два NuGet warnings. Package versions не изменялись.

## Ограничения

Не проверялись и не запускались API startup, PostgreSQL, Azure Blob Storage, migrations, `/health`, OpenAPI/Swagger, tests и runtime behavior. `DATA-01` и `BASE-03` не начинались.
