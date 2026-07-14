# DATA-03 — Ручной SuperAdmin bootstrap

## Цель

Документировать и доказать безопасное ручное создание первого `SuperAdmin` в clean PostgreSQL без seed, постоянного setup tool и сохранения credentials/hash в Git.

## Исходное состояние

- `Admin` хранит `Login`, Identity-compatible hash в `Password` и integer `Type`; `AdminType.SuperAdmin = 1`.
- Login использует `PasswordHasher<Admin>.VerifyHashedPassword` и при успехе возвращает role `SuperAdmin` с HttpOnly JWT cookie.
- Migrations создают `Admins`, но seed/bootstrap отсутствовал.
- `Admin.Login` не имеет unique index, поэтому manual procedure должна защищать concurrency и collision самостоятельно.
- `DEC-008` требует ручную DB insertion и отвергает seed/setup tool.

## Затронутые файлы

- `docs/implementation/13_SUPERADMIN_BOOTSTRAP.md`
- `docs/implementation/evidence/DATA-03.md`
- `docs/implementation/00_INDEX.md`
- `docs/implementation/04_BACKLOG.md`
- `docs/implementation/09_STATE.md`

Production/backend source не менялся. Существующие пользовательские изменения `QA-01`/`API-01` сохранены.

## Решения

- Hash генерируется временным .NET helper вне repository через тот же `PasswordHasher<Admin>`; helper не является tracked setup tool.
- Credentials вводятся интерактивно; password/confirmation не отображаются. Generated SQL передаётся в `psql` только через pipe, без файла, `tee` и query echo.
- SQL использует `SHARE ROW EXCLUSIVE` table lock и повторно проверяет отсутствие `Type = 1` и duplicate login внутри одной transaction.
- Procedure следует текущей `AddBasicAdminRequestValidator` credential policy.
- Login proof выполняется временным helper через реальный `/api/account/login`; JWT/cookie value не печатается.
- Bootstrap намеренно отказывает на БД с существующим SuperAdmin. Rotation/recovery задокументированы как отдельные approved DB/secret operations.

## Выполненные изменения

1. Добавлен secure runbook с prerequisites, target guard, временным hash/SQL helper, controlled transaction, DB assertions, real login proof, cleanup и stop conditions.
2. Задокументированы rotation/recovery boundaries, включая отсутствие JWT revocation при одной смене password.
3. Добавлен этот evidence; после green validation синхронизированы backlog, state и index.

## Проверки

Все DB/API действия выполнялись 13 июля 2026 года только с disposable local PostgreSQL 17. Production/staging/remote resources не использовались. Synthetic local credentials были удалены вместе с container и не включены в repository/evidence.

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| Runbook RED | `test -f docs/implementation/13_SUPERADMIN_BOOTSTRAP.md && rg ...` до создания файла | Файл отсутствовал | 1 | Проверка действительно различала отсутствующий DATA-03 artifact |
| Runbook contract GREEN | `test -f ... && rg` по hasher, transaction guard, login proof и recovery sections | Все обязательные sections найдены | 0 | Runbook содержит заявленный contract |
| Temporary helper restore/build | `dotnet restore` и `dotnet build --configuration Release` для exact helper вне repository | 0 warnings, 0 errors | 0 / 0 | Документированный C# helper компилируется на .NET 9 |
| Runbook/helper identity | Extract `Program.cs`/csproj blocks через `awk`, затем `diff -u` с compiled files | Diff пуст | 0 / 0 | Проверен именно код, опубликованный в runbook |
| Disposable PostgreSQL | `docker run ... postgres:17`; `pg_isready` | Local PostgreSQL принимала connections | 0 | Target изолирован и доступен |
| Canonical migrations | Infrastructure-only `dotnet ef database update` с local `DB_CLOUD` | Применены все 24 migrations | 0 | Clean DB получила current schema отдельной командой |
| Clean Admin baseline | Read-only count `Admins`/`Type = 1` | `0|0` | 0 | Bootstrap начался без Admin/SuperAdmin rows |
| Controlled bootstrap | Exact helper stdout piped в `psql --quiet --set ON_ERROR_STOP=1` | Transaction завершилась без credential/hash output | 0 | Первый SuperAdmin создан manual guarded DB procedure |
| DB shape | Aggregate-only query count/type/hash non-empty/hash differs from login | `1|1|t|t` | 0 | Создана ровно одна `Type = 1` запись с non-plaintext field shape |
| Real API login | API startup с той же local DB; helper вызвал `/api/account/login` | `HTTP 200; role=SuperAdmin; token-cookie=present` | 0 | Application verifier принимает hash; role/cookie формируются |
| Duplicate guard | Повтор exact bootstrap pipeline с другим synthetic login | `SuperAdmin already exists; bootstrap refused` | 3 | Procedure не создаёт второго SuperAdmin |
| Final DB state | Aggregate counts после refused rerun; migration history count | `1|1|t`; `24` | 0 | Failed rerun не изменил account/schema state |
| Canonical solution restore | `dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln --disable-build-servers` | 5 projects restored/up-to-date; 2 existing ImageSharp advisories | 0 | Backend dependencies разрешаются без package changes |
| Solution Release build | `dotnet build ... --no-restore --configuration Release --disable-build-servers` | Build succeeded; 0 errors, 8 existing warnings | 0 | DATA-03 docs/runbook не сопровождаются broken backend baseline |
| Full backend tests | `dotnet test ... --no-restore --no-build --configuration Release --disable-build-servers` с local DB | 11 passed, 0 failed/skipped | 0 | Existing auth regression suite остаётся green |
| Disposable cleanup | `docker stop`; exact-name `docker ps -a` | Container удалён; итоговый список пуст | 0 / 0 | Test DB/account не оставлены запущенными или сохранёнными |
| Temporary cleanup | Удаление только `/tmp/storonnimv-data03-verify` и `/tmp/storonnimv-data03-final`; exact absence checks | Оба каталога отсутствуют; API port не слушается | 0 / 0 | Helper/build artifacts и local API process не оставлены |

Первая EF attempt внутри sandbox собрала project, но получила `SocketException (13): Permission denied` на localhost и exit 1. Та же команда вне sandbox прошла. Изолированный artifacts restore дважды не создал полный assets set; probe build дал `NETSDK1004` и exit 1. Canonical warmed-cache restore затем завершился exit 0. Первая sandboxed solution build закончилась без итогового результата; повтор вне sandbox дал полный exit 0. Эти попытки классифицированы как execution-environment limits, не source failures.

## Невыполненные проверки

- Production/staging bootstrap не выполнялся: production access запрещён и для него нужны approval/backup/secret process.
- Фактическая production rotation/recovery и `TOKEN_KEY` rotation не выполнялись: это отдельная approved operational change, не first bootstrap.
- Browser/frontend login не выполнялся: acceptance доказана real API endpoint; UI auth flow относится к `FEAT-01` после `API-02`.
- CSRF/cookie-origin topology не проверялась: это `API-02`.

## Проблемы вне scope

- `Admin.Login` всё ещё без unique DB index; bootstrap компенсирует это только своим lock/guard. Schema change не требовалась DATA-03.
- Password rotation не отзывает уже выданные JWT; incident response требует отдельной signing-key rotation.
- Basic Admin management services пока не доказывают `AdminType` boundary для SuperAdmin; это `FEAT-09`.
- Restore/build сохраняют существующие ImageSharp `NU1902`/`NU1903` и шесть compiler warnings. Package/code cleanup не относится к DATA-03.

Ни одна проблема не блокирует first manual bootstrap и login proof.

## Итог по критериям приёмки

| Критерий | Итог |
|---|---|
| В clean test DB создан account | Выполнен: baseline `0|0`, после transaction `1|1|t|t` |
| Login возможен | Выполнен: real API вернул `200`, role `SuperAdmin`, token cookie present |
| Procedure повторяема уполномоченным лицом | Выполнен: exact compiled helper и команды задокументированы; secrets вводятся интерактивно вне Git |
| Повтор не создаёт второго SuperAdmin | Выполнен: guarded rerun exit 3, final count остался 1 |
| Rotation/recovery boundaries описаны | Выполнен: manual change, JWT/signing-key и stop boundaries зафиксированы |
| Password/hash не сохранены в Git | Выполнен: tracked artifacts содержат только code/placeholders и aggregate evidence |

Все критерии `DATA-03` выполнены. Статус: `done`. Следующая задача backlog — `API-02`; она не начиналась.
