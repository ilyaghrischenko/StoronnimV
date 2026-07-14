# API-02 — Cookie/CORS/CSRF auth topology

## Цель

Сделать browser cookie-auth mutations безопасными и воспроизводимыми: exact-origin credentialed CORS, согласованные local/future cookie flags, antiforgery token transport и отказ mutation без token. Production provider не выбирался.

## Исходное состояние

- JWT находился в `HttpOnly` cookie `Token`; frontend уже ставил `withCredentials: true`.
- Base cookie имел `Secure=true`, `SameSite=None`; HTTP local browser не имел отдельного development override.
- Login читал optional `DOMAIN`, logout требовал `DOMAIN`, поэтому host-only local contract расходился между endpoints.
- CORS принимал одну необработанную строку `CLIENT_URL` с credentials и any headers/methods.
- Unsafe cookie-auth endpoints не имели antiforgery/CSRF validation.
- `GlobalContext.sendRequest` не получал и не передавал CSRF token.

## Затронутые файлы

- Backend composition/config: `Program.cs`, `WebApplicationBuilderExtensions.cs`, `appsettings*.json`, `.env.example`.
- Backend HTTP/auth: `AccountController.cs`, `AdminController.cs`, `AccountControllerService.cs`, `Middlewares/AntiforgeryMiddleware.cs`.
- Frontend transport: `GlobalContext.tsx`.
- Tests: `AuthenticationIntegrationTests.cs`.
- Contract/state: `10_RUNTIME_CONTRACT.md`, `00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`, этот evidence.

Существующие uncommitted изменения `DATA-03` сохранены. Production DB/Blob и remote endpoints не использовались.

## Решения

- Использован ASP.NET Core antiforgery, не custom Origin-only defense.
- `GET /api/account/csrf-token` возвращает no-store request token; antiforgery cookie остаётся `HttpOnly`.
- Frontend получает fresh token перед каждым unsafe method. Это избегает identity-stale token после login/logout ценой одного дополнительного GET на mutation.
- Login и unsafe requests с authenticated JWT cookie требуют antiforgery. Bearer-only mutations не требуют token, поскольку bearer credential не отправляется browser автоматически; invalid JWT cookie сохраняет authorization response `401`.
- `CLIENT_URL` остаётся одним environment-specific origin, но теперь валидируется как exact HTTP(S) origin. Hosting provider не зафиксирован.
- Default production-like cookie: `Secure=true`, `SameSite=Lax`, host-only. Development HTTP loopback: `Secure=false`, `SameSite=Lax`. Если будущий deployment реально cross-site, он обязан явно задать `Secure=true`/`SameSite=None`.

## Выполненные изменения

1. Добавлены antiforgery registration, token endpoint и middleware после authentication.
2. CORS ограничен валидированным exact `CLIENT_URL`; unknown origin не получает allow-origin.
3. Login/logout используют единые bound cookie options; прямой `DOMAIN` удалён, optional `CookieOptions__Domain` сохраняет host-only default.
4. `GlobalContext.sendRequest` добавляет fresh `X-CSRF-TOKEN` для `POST`, `PUT`, `PATCH`, `DELETE` и других unsafe methods.
5. Integration suite расширена login/mutation, missing-token rejection, bearer bypass и allowed/unknown CORS preflight.

## Проверки

| Проверка | Команда или сценарий | Результат | Exit code | Что доказывает |
|---|---|---|---:|---|
| TDD RED | Targeted `dotnet test ... --filter FullyQualifiedName~AuthenticationIntegrationTests` на disposable PostgreSQL до production changes | 14 passed, 2 failed: token endpoint не возвращал JSON; cookie mutation без token вернула `200` вместо `400` | 1 | Новые tests реально ловили отсутствующую API-02 behavior |
| Targeted auth topology | Та же targeted test command после implementation | 16 passed, 0 failed/skipped | 0 | Login+cookie mutation с token работают; missing token отклонён; bearer mutation работает; CORS exact-origin matrix green |
| Invalid-cookie auth RED/GREEN | Targeted `CookieMutation_WithInvalidToken_ReturnsUnauthorized` до/после authenticated-principal guard | Сначала expected `401`, actual `400`; после correction 1 passed | 1 / 0 | Antiforgery не маскирует invalid JWT как CSRF failure; protected mutation сохраняет auth `401` |
| Real browser | Playwright-managed Firefox 152.0.4, Vite `127.0.0.1:5173`, real API `127.0.0.1:5268`, disposable PostgreSQL 17 | token `200` → login `200` → `isAdmin` `200` → token `200` → logout `200`; logout control исчез | 0 | Настоящий browser сохраняет credential cookie и frontend выполняет antiforgery sequence для login/logout |
| Controlled CSRF | Cookie-auth `POST /api/auth-test/mutation` с foreign Origin без antiforgery token | `400`; allow-origin отсутствует | 0 (assertion) | Cross-site/missing-token mutation не достигает endpoint |
| CORS matrix | OPTIONS preflight from configured and unknown origins | Configured origin: `204` + allow-origin; unknown: `204` без allow-origin | 0 (assertions) | Только `CLIENT_URL` получает credentialed CORS permission |
| Frontend build | `npm run build` | Vite built 535 modules | 0 | TypeScript и production bundle собираются |
| Targeted frontend lint | `npx eslint src/components/contexts/shared/GlobalContext.tsx` | 0 errors/warnings | 0 | Изменённый request transport соответствует lint rules |
| Full frontend lint | `npm run lint` | 5 errors/20 warnings только в других files | 1 | Зафиксирован существующий repo baseline; изменённый файл clean |
| Backend restore | `dotnet restore ... --no-cache --disable-build-servers` | 5 projects restored; 2 existing ImageSharp advisories | 0 | Dependencies разрешаются без package changes |
| Solution Release build | `dotnet build ...sln --no-restore --configuration Release --disable-build-servers` | 0 errors; 2 existing advisories | 0 | Полный backend компилируется |
| API Release build | `dotnet build ...Api.csproj --no-restore --configuration Release --disable-build-servers` | 0 errors; 2 existing advisories | 0 | Startup project компилируется |
| Full backend tests | `dotnet test ...sln --no-restore --configuration Release --disable-build-servers` | Build completed; 17 passed, 0 failed/skipped | 0 | Финальный source компилируется и полный текущий backend regression suite green |

## Невыполненные проверки

- Production/staging DNS, TLS, exact origin и cross-site cookie override не проверялись: provider/topology не выбраны и production access запрещён; это `OPS-01`/M5.
- Chrome channel не использован: local Playwright CLI потребовал system Chrome install через unavailable `sudo`; managed Firefox прошёл real browser scenario. Multi-browser release matrix остаётся QA/M6.
- Full frontend lint не green из-за 5 errors/20 warnings в несвязанных files; `GlobalContext.tsx` green, общий cleanup уже назначен `QA-03`.

## Проблемы вне scope

- Full ESLint baseline: 5 `@ts-ignore` errors и 20 hook warnings вне API-02 files.
- Browser на пустом disposable corpus показал четыре существующие Home console errors для `204` schedule/video responses; auth requests остались `200`.
- Restore/build сохраняют существующие `NU1902`/`NU1903` advisories для ImageSharp 3.1.6. Package upgrade не требовался API-02.

Ни одна проблема не блокирует cookie/CORS/CSRF acceptance.

## Итог по критериям приёмки

| Критерий | Итог |
|---|---|
| Credentialed login работает | Выполнен: integration test и real Firefox login `200`, затем cookie-auth `isAdmin` `200` |
| Credentialed mutation работает | Выполнен: protected integration mutation и real browser logout `200` после fresh token |
| Cross-site/missing-token mutation отклоняется | Выполнен: controlled request `400`, endpoint не выполнен |
| CORS принимает только configured origin | Выполнен: allowed/unknown preflight matrix |
| Local/future cookie topology согласована без provider choice | Выполнен: Development/base/cross-site override contract зафиксирован |

Все критерии `API-02` выполнены. Статус: `done`. Следующая задача backlog — `FEAT-01`; она не начиналась.
