# BASE-02 — Clean backend build evidence

## Цель

Доказать clean restore/build backend solution и startup API на поддерживаемой ОС, устранить только фактические compile/portability blockers и подтвердить отсутствие machine-specific references без обновления packages или изменения runtime behavior.

## Исходное состояние

- Зависимость `BASE-01` имеет статус `done`.
- До текущей повторной проверки `BASE-02` уже имела статус `done`, а commit `56fac80` удалил из `StoronnimV.Infrastructure.csproj` Windows-only `HintPath` к `Microsoft.Extensions.Configuration.dll`.
- Удалённый assembly reference дублировал существующий переносимый `PackageReference` `Microsoft.Extensions.Configuration` версии 9.0.0. Package versions, target framework и project graph не менялись.
- Единственное исходное незакоммиченное изменение — `backend/.DS_Store`; оно принадлежит пользователю и не изменялось в рамках задачи.

## Затронутые файлы

| Файл | Изменение |
|---|---|
| `docs/implementation/evidence/BASE-02.md` | Канонический evidence с результатами повторной проверки |
| Предыдущий evidence-файл | Удалён после переноса сведений в требуемый `BASE-02.md` |
| `docs/implementation/00_INDEX.md` | Ссылка обновлена на канонический evidence |
| `docs/implementation/04_BACKLOG.md` | Исправлена устаревшая итоговая строка о следующей незаблокированной задаче; статус BASE-02 оставлен `done` |
| `docs/implementation/09_STATE.md` | Ссылка и фактические команды проверки синхронизированы |
| `docs/implementation/10_RUNTIME_CONTRACT.md` | Ссылки обновлены на канонический evidence |

Продуктовый код в текущем запуске не менялся. Фактическое исправление portability уже присутствует в `backend/StoronnimV.Server/StoronnimV.Infrastructure/StoronnimV.Infrastructure.csproj`: удалён только дублирующий Windows-specific `<Reference>`/`<HintPath>`.

## Принятые решения

- Использовать новые изолированные `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH` и artifacts path под `/tmp`, чтобы не полагаться на repository `bin`/`obj`.
- После подтверждённого зависания restore/build в sandbox повторить обязательные команды с разрешённым network/process access; исходный код при этом не менялся.
- Не обновлять `SixLabors.ImageSharp` 3.1.6: это известная проблема безопасности вне compile/portability scope BASE-02, а backlog прямо запрещает package update без доказанной необходимости.
- Не исправлять существующие compiler warnings и пустой test project: они не блокируют критерии BASE-02 и относятся к отдельным backlog-задачам.

## Выполненные изменения

1. Повторно подтверждён clean restore всего solution в пустой package cache.
2. Повторно подтверждены Release build всего solution и отдельного startup API.
3. Подтверждено отсутствие `HintPath`, абсолютных user paths, external MSBuild imports и OS-specific conditions в backend solution/project metadata.
4. Выполнен относящийся backend test gate; runner завершился успешно, но тестов в проекте нет.
5. Исторический evidence перенесён в требуемый файл `BASE-02.md`; ссылки синхронизированы.

## Среда проверки

- Дата: 13 июля 2026 года.
- OS: macOS 26.5 (`Darwin`, arm64).
- .NET SDK: 9.0.203; это характеристика машины проверки, а не project pin.
- Solution: `backend/StoronnimV.Server/StoronnimV.Server.sln`.
- Startup API: `backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj`.
- Configuration: `Release`.
- Изоляция: перед финальным циклом подтверждено отсутствие `/tmp/storonnimv-base02-final-019f58b4`; затем созданы отдельные `{cli-home,packages,http-cache,artifacts}`.

## Выполненные команды и результаты

Для restore/build/test использовались одинаковые environment variables:

```text
DOTNET_CLI_HOME=/tmp/storonnimv-base02-final-019f58b4/cli-home
NUGET_PACKAGES=/tmp/storonnimv-base02-final-019f58b4/packages
NUGET_HTTP_CACHE_PATH=/tmp/storonnimv-base02-final-019f58b4/http-cache
DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=true
```

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Toolchain | `dotnet --info` | .NET SDK 9.0.203, Darwin arm64 | 0 | На машине доступен требуемый .NET 9 SDK |
| Clean solution restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --no-cache --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers` | 0 errors, 2 `SixLabors.ImageSharp` vulnerability warnings | 0 | Весь solution восстанавливается в заранее отсутствовавшем package cache |
| Solution Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers` | Build succeeded; 0 errors, 8 warnings | 0 | Все пять solution projects компилируются |
| Startup API Release build | `dotnet build backend/StoronnimV.Server/StoronnimV.Api/StoronnimV.Api.csproj --no-restore --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers` | Build succeeded; 0 errors, 2 warnings | 0 | Startup API и его project graph компилируются отдельно |
| Backend test gate | `dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln --no-restore --no-build --configuration Release --artifacts-path /tmp/storonnimv-base02-final-019f58b4/artifacts --disable-build-servers` | Runner успешно обработал test assembly; доступных тестов нет | 0 | Test command технически выполняется, но behavioral coverage отсутствует |
| Portability scan | `rg` по backend `.csproj`/`.props`/`.targets`/`.sln` для `HintPath`, абсолютных user paths, external imports и OS conditions | Совпадений нет | 1 | Backend project metadata не содержит проверяемых machine-specific references |
| Whitespace validation | `git diff --check` и trailing-whitespace scan нового evidence | Ошибок нет | 0 | Итоговые изменения не содержат whitespace errors |
| Secret scan | Regex scan итогового diff и нового evidence для private keys, common access/API key formats и присвоенных secret/password values | Совпадений нет | 0 | Изменённые файлы не содержат распознанных secrets |
| Итоговый scope review | `git status --short`, полный diff и чтение нового evidence | Только документы BASE-02; отдельный исходный `backend/.DS_Store` сохранён | 0 | Новых build artifacts и несвязанных изменений задачи в repository нет |

### Диагностические попытки

- Restore в sandbox не выдавал прогресса из-за недоступного NuGet network access; финальная обязательная проверка повторена с разрешённым сетевым доступом и завершилась exit 0.
- Первый sandboxed solution build не выдавал MSBuild output более 90 секунд и был остановлен (`exit 143`); финальный build вне sandbox завершился exit 0 за 4,44 секунды.
- Первая отдельная API build попытка использовала другой artifacts path и закономерно завершилась `NETSDK1004` (`exit 1`): assets существовали в основном artifacts path. Команда исправлена без изменения кода и повторена с тем же path, что restore; финальный результат — exit 0.

## Warnings и проблемы вне scope

- `SixLabors.ImageSharp` 3.1.6: `NU1902` и `NU1903`. Это подтверждённая security/dependency проблема, но не compile/portability blocker BASE-02; обновление package не выполнялось.
- Solution build: два существующих `CS8981`, один `CS8602`, два `CS8618` и один `CS8629`; вместе с двумя NuGet warnings — 8 warnings, 0 errors. Они не исправлялись.
- `StoronnimV.Tests` содержит test assembly, но доступных тестов нет. Это не блокирует build acceptance, но означает отсутствие behavioral regression coverage.
- Устаревшая итоговая строка `04_BACKLOG.md`, называвшая `BASE-01` первой незаблокированной задачей, была фактически неверной после завершения BASE-01/BASE-02 и синхронизирована с `DATA-01`.

## Невыполненные проверки

- API startup, PostgreSQL, Azure Blob Storage, migrations, `/health`, OpenAPI/Swagger и runtime behavior не запускались: они находятся вне scope BASE-02 и относятся к `DATA-01`/`BASE-03`.
- Frontend build/lint/browser checks не запускались: frontend не затронут задачей.
- Production/staging checks не запускались: доступ к production resources запрещён и для BASE-02 не требуется.

## Итог по критериям приёмки

| Критерий | Итог | Evidence |
|---|---|---|
| Solution собирается | Выполнен | Clean restore и solution Release build завершились exit 0, 0 errors |
| Startup API собирается отдельно | Выполнен | API Release build завершился exit 0, 0 errors |
| Нет machine-specific references | Выполнен | Windows-only `HintPath` удалён ранее; повторный metadata scan не нашёл совпадений |
| Packages не обновлены без необходимости | Выполнен | Package versions не менялись |
| Изменения ограничены BASE-02 | Выполнен | Текущий diff содержит только evidence и синхронизацию ссылок; пользовательский `.DS_Store` сохранён |

Все критерии приёмки BASE-02 выполнены. Статус задачи остаётся `done`. Следующая незаблокированная задача по актуальному состоянию — `DATA-01`; она не начиналась.
