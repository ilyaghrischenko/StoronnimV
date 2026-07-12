# Состояние проекта для будущих сеансов

## Текущая цель

Выполнить утверждённый план завершения StoronnimV, начиная с воспроизводимого локального запуска. План создан; исходный код ещё не изменялся.

## Утверждённый объём

- Public: Home, Schedule, News, Music, Group, Video, Footer/socials, Error и пустая static Developers page.
- Admin: полный content/media CRUD и SuperAdmin management Basic Admin accounts.
- Devices: mobile, tablet и desktop для public/admin.
- Data: существующие PostgreSQL/Azure Blob данные после backup/inventory.
- Operations: Hangfire status job; production dashboard disabled; explicit migrations; последующий deployment.

## Исключено

Analytics, contact/booking forms, commerce/tickets, search, multilingual UI, новая admin dashboard, automatic startup migrations, public Hangfire dashboard и новая logging strategy.

## Активный milestone

`M1 — Воспроизводимый локальный запуск`.

## Следующая задача

`BASE-01 — Зафиксировать runtime contract`. Это первая незаблокированная задача.

## Ключевые ограничения

- Читать актуальный root/user `AGENTS.md` перед каждой задачей.
- Выполнять только одну backlog task или явно согласованный связанный набор.
- Не менять production DB/Blob до backup и authorization.
- Не хранить secrets/credentials в Git, logs или документации.
- Сохранять React/.NET/PostgreSQL/Azure/Hangfire architecture.
- Desktop visual baseline — runtime `style.css`; mobile frame можно упрощать.
- Migrations только отдельной командой; SuperAdmin вручную.

## Команды проверки

Конкретные команды уточняются `BASE-01`; ожидаемый baseline:

```bash
dotnet restore backend/StoronnimV.Server/StoronnimV.Server.sln
dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln
dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln
npm run build --prefix frontend/storonnimv.client
npm run lint --prefix frontend/storonnimv.client
```

Migration command выполняется только после проверки target connection и backup согласно `DATA-01`/`OPS-03`.

## Открытые решения

См. [08_OPEN_ITEMS.md](08_OPEN_ITEMS.md). Первый milestone потенциально зависит от доступа к backup/content, но `BASE-01`, `BASE-02`, `DATA-01` и `BASE-03` можно начинать до него.

## Что читать перед реализацией

1. Корневые инструкции `AGENTS.md`, если файл существует, и инструкции пользователя текущего сеанса.
2. [01_REQUIREMENTS.md](01_REQUIREMENTS.md), [02_DECISIONS.md](02_DECISIONS.md).
3. Строку задачи в [04_BACKLOG.md](04_BACKLOG.md) и связанный milestone.
4. Релевантные `docs/project-analysis/*` и только затем затрагиваемый код и callers.
5. [06_VALIDATION_PLAN.md](06_VALIDATION_PLAN.md) для required evidence.

## Правило обновления состояния

После каждой принятой задачи обновлять её статус, фактические проверки, новые риски/open items, активный milestone и следующую незаблокированную задачу. Не отмечать задачу завершённой без выполнения её критериев приёмки. Scope/decision меняется только после явного решения владельца и синхронного обновления requirements, decisions, plan, backlog и traceability.
