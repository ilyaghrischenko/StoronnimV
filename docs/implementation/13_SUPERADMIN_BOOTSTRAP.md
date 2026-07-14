# Ручной bootstrap первого SuperAdmin

## Назначение и границы

Runbook вручную добавляет первый `SuperAdmin` в уже подготовленную PostgreSQL schema. Он не является seed или постоянным setup tool: helper создаётся во временном каталоге вне Git, генерирует hash через тот же `PasswordHasher<Admin>`, передаёт SQL напрямую в `psql` и не печатает password/hash.

Процедура предназначена для уполномоченного оператора. Для production сначала нужны явное разрешение владельца, актуальный backup, подтверждённый target и approved secret-handling process. Не сохраняйте DB connection string, login, password, hash, generated SQL или JWT в repository, shell history, tickets, screenshots и logs.

## Предусловия

1. Установлены .NET 9 SDK, Docker и PostgreSQL client либо локально доступен `postgres:17` image.
2. Migrations применены отдельной командой из [11_MIGRATION_WORKFLOW.md](11_MIGRATION_WORKFLOW.md). API startup не изменяет schema.
3. `DATA03_PG_DSN` содержит libpq URI/conninfo выбранной БД и задан только в process environment или approved secret manager.
4. В таблице `Admins` нет `SuperAdmin`; выбранный login также отсутствует. Procedure намеренно прекращается при любом из этих условий.
5. Команды выполняются из корня repository. Для shared/production target окно изменения и backup подтверждены до table lock.

Проверьте target без чтения credential fields:

```bash
docker run --rm --env DATA03_PG_DSN postgres:17 \
  sh -c 'exec psql --dbname="$DATA03_PG_DSN" --no-psqlrc --set ON_ERROR_STOP=1 --command="$1"' sh \
  'SELECT current_database(), current_user; SELECT "Type", COUNT(*) FROM "Admins" GROUP BY "Type" ORDER BY "Type";'
```

Значения `AdminType` в текущем коде и EF schema: `0` — `Basic`, `1` — `SuperAdmin`. Не продолжайте, если schema отличается или запрос возвращает строку с `Type = 1`.

## Временный helper

Создайте private temporary directory. Исходник helper не содержит credentials; build/output должны оставаться вне repository.

```bash
export DATA03_DIR="$(mktemp -d "${TMPDIR:-/tmp}/storonnimv-data03.XXXXXX")"
chmod 700 "$DATA03_DIR"

cat > "$DATA03_DIR/Data03.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <ProjectReference Include="$PWD/backend/StoronnimV.Server/StoronnimV.Domain/StoronnimV.Domain.csproj" />
  </ItemGroup>
</Project>
EOF

cat > "$DATA03_DIR/Program.cs" <<'EOF'
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;

static string ReadSecret(string prompt)
{
    Console.Error.Write(prompt);
    var value = new StringBuilder();

    while (true)
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter) break;
        if (key.Key == ConsoleKey.Backspace && value.Length > 0)
        {
            value.Length--;
            continue;
        }
        if (!char.IsControl(key.KeyChar)) value.Append(key.KeyChar);
    }

    Console.Error.WriteLine();
    return value.ToString();
}

static (string Login, string Password) ReadCredentials(bool enforceCurrentPolicy)
{
    Console.Error.Write("Login: ");
    string login = Console.ReadLine()?.Trim() ?? string.Empty;
    string password = ReadSecret("Password: ");

    if (!enforceCurrentPolicy) return (login, password);

    string confirmation = ReadSecret("Confirm password: ");
    bool validLogin = login.Length >= 4
        && Regex.IsMatch(login, "[A-Za-z]")
        && Regex.IsMatch(login, "[0-9]");
    bool validPassword = password.Length is >= 10 and <= 15
        && Regex.Matches(password, "[A-Za-z]").Count >= 5
        && Regex.Matches(password, "[A-Z]").Count >= 3
        && Regex.Matches(password, @"\d").Count >= 5;

    if (!validLogin || !validPassword || password != confirmation)
    {
        throw new InvalidOperationException(
            "Credentials do not match current AddBasicAdminRequestValidator policy or confirmation.");
    }

    return (login, password);
}

if (args is ["login", var apiBase])
{
    var credentials = ReadCredentials(enforceCurrentPolicy: false);
    using var client = new HttpClient { BaseAddress = new Uri(apiBase.TrimEnd('/') + "/") };
    using HttpResponseMessage response = await client.PostAsJsonAsync(
        "api/account/login", new { credentials.Login, credentials.Password });
    string role = (await response.Content.ReadAsStringAsync()).Trim().Trim('"');
    bool tokenCookie = response.Headers.TryGetValues("Set-Cookie", out var cookies)
        && cookies.Any(cookie => cookie.StartsWith("Token=", StringComparison.Ordinal));

    Console.WriteLine(
        $"HTTP {(int)response.StatusCode}; role={role}; token-cookie={(tokenCookie ? "present" : "missing")}");
    return response.IsSuccessStatusCode && role == "SuperAdmin" && tokenCookie ? 0 : 1;
}

var bootstrap = ReadCredentials(enforceCurrentPolicy: true);
var admin = new Admin
{
    Login = bootstrap.Login,
    Password = string.Empty,
    Type = AdminType.SuperAdmin
};
string hash = new PasswordHasher<Admin>().HashPassword(admin, bootstrap.Password);
string loginBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(bootstrap.Login));
string hashBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(hash));

Console.WriteLine("BEGIN;");
Console.WriteLine("LOCK TABLE \"Admins\" IN SHARE ROW EXCLUSIVE MODE;");
Console.WriteLine("DO $bootstrap$");
Console.WriteLine("DECLARE");
Console.WriteLine($"  bootstrap_login text := convert_from(decode('{loginBase64}', 'base64'), 'UTF8');");
Console.WriteLine($"  bootstrap_hash text := convert_from(decode('{hashBase64}', 'base64'), 'UTF8');");
Console.WriteLine("BEGIN");
Console.WriteLine("  IF EXISTS (SELECT 1 FROM \"Admins\" WHERE \"Type\" = 1) THEN");
Console.WriteLine("    RAISE EXCEPTION 'SuperAdmin already exists; bootstrap refused';");
Console.WriteLine("  END IF;");
Console.WriteLine("  IF EXISTS (SELECT 1 FROM \"Admins\" WHERE \"Login\" = bootstrap_login) THEN");
Console.WriteLine("    RAISE EXCEPTION 'Admin login already exists; bootstrap refused';");
Console.WriteLine("  END IF;");
Console.WriteLine("  INSERT INTO \"Admins\" (\"Login\", \"Password\", \"Type\", \"CreatedAt\", \"UpdatedAt\")");
Console.WriteLine("  VALUES (bootstrap_login, bootstrap_hash, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");
Console.WriteLine("END");
Console.WriteLine("$bootstrap$;");
Console.WriteLine("COMMIT;");
return 0;
EOF

dotnet restore "$DATA03_DIR/Data03.csproj" --disable-build-servers
dotnet build "$DATA03_DIR/Data03.csproj" --no-restore --configuration Release --disable-build-servers
```

Helper применяет текущую policy `AddBasicAdminRequestValidator`: login минимум 4 символа с ASCII letter и digit; password 10–15 символов, минимум 5 ASCII letters, 3 uppercase ASCII letters и 5 digits. Credentials вводятся интерактивно; password и confirmation не отображаются.

## Controlled bootstrap

Следующая pipeline генерирует SQL в памяти, передаёт его прямо в `psql` и прекращается при первой SQL error. Не добавляйте `tee`, shell tracing (`set -x`) или redirection в файл.

```bash
set -o pipefail
dotnet run --project "$DATA03_DIR/Data03.csproj" --no-build --configuration Release \
  | docker run --rm --interactive --env DATA03_PG_DSN postgres:17 \
      sh -c 'exec psql --dbname="$DATA03_PG_DSN" --no-psqlrc --quiet --set ON_ERROR_STOP=1'
```

Transaction берёт короткий table lock, повторно проверяет отсутствие любого `Type = 1` и collision login, затем вставляет ровно одну запись. Повторный запуск на той же БД обязан завершиться non-zero с `SuperAdmin already exists; bootstrap refused`; это guard, не ошибка для обхода.

Проверьте результат, не выбирая `Login` или `Password`:

```bash
docker run --rm --env DATA03_PG_DSN postgres:17 \
  sh -c 'exec psql --dbname="$DATA03_PG_DSN" --no-psqlrc --tuples-only --no-align --set ON_ERROR_STOP=1 --command="$1"' sh \
  'SELECT COUNT(*), COUNT(*) FILTER (WHERE "Type" = 1), bool_and(length("Password") > 0) FROM "Admins";'
```

Для clean DB ожидается `1|1|t`.

## Login proof

Запустите API с этим же target и штатными environment variables из [10_RUNTIME_CONTRACT.md](10_RUNTIME_CONTRACT.md). Для disposable local environment допустим HTTP loopback; shared/staging/production login выполняйте только через approved HTTPS endpoint.

```bash
dotnet run --project "$DATA03_DIR/Data03.csproj" --no-build --configuration Release \
  -- login "${DATA03_API_URL:?set approved API base URL}"
```

Введите те же credentials. Ожидается `HTTP 200; role=SuperAdmin; token-cookie=present` и exit code 0. Helper не печатает JWT/cookie value. После proof очистите browser/client session, unset `DATA03_PG_DSN`/`DATA03_API_URL` и удалите весь `$DATA03_DIR`.

## Rotation и recovery boundaries

- Этот bootstrap выполняется только один раз для БД без SuperAdmin. Не удаляйте существующую запись и не меняйте `Type`, чтобы повторно использовать процедуру.
- Плановая смена login/password и recovery требуют отдельного одобренного DB change с backup, точным `Id`/`Type = 1` predicate, проверкой ровно одной изменённой строки и повторным login proof. Generated hash нельзя сохранять в runbook или ticket.
- Не используйте Basic Admin management endpoints для SuperAdmin rotation/recovery: их защита `AdminType` проверяется отдельной задачей `FEAT-09`.
- Смена password не отзывает уже выпущенные JWT. При подозрении на компрометацию требуется также approved rotation `TOKEN_KEY` через secret manager и restart API; это завершает все текущие admin sessions и относится к operational incident procedure, не к bootstrap.
- При неизвестном target, несовпадении schema, отсутствии backup/доступа, существующем SuperAdmin, duplicate login или failed login остановитесь. Не исправляйте row/hash вручную и не создавайте второй SuperAdmin; передайте обезличенную диагностику владельцу.
