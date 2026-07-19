# FEAT-06 — Group singleton, Members и Socials

**Дата проверки:** 15 июля 2026 года  
**Итог:** `done`

## Цель и границы

Задача закрывает один desktop Group vertical: единственный `GroupPage`, редактирование description/photo, member create/read/edit/photo/delete и member social create/edit/delete с readback. Versions, несколько одновременно существующих GroupPage, Music/group-socials, responsive redesign и production data не добавлялись.

Production DB/Blob не использовались. Проверка выполнена на одноразовых PostgreSQL 17 и Azurite, доступных только через localhost.

## Изменённые файлы

- `backend/StoronnimV.Server/StoronnimV.Application/Services/Entities/GroupPageService.cs`
- `backend/StoronnimV.Server/StoronnimV.Infrastructure/Migrations/20260715012000_EnforceGroupPageSingleton.cs`
- `backend/StoronnimV.Server/StoronnimV.Tests/Api/GroupCrudIntegrationTests.cs`
- `frontend/storonnimv.client/src/components/elements/group/forms/member/DeleteMemberModal.tsx`
- status/evidence: `docs/implementation/00_INDEX.md`, `04_BACKLOG.md`, `09_STATE.md`, `10_RUNTIME_CONTRACT.md`, `11_MIGRATION_WORKFLOW.md`, этот файл.

Предшествующие незакоммиченные `API-04` изменения сохранены без reset, clean, stash или перезаписи.

## Реализация

- `GroupPageService` отклоняет обычную повторную попытку создания до Blob upload и возвращает существующий unified `400` contract.
- PostgreSQL unique expression index по константе `TRUE` физически разрешает не более одной строки `GroupPages`, включая concurrent/direct DB writes.
- Migration перед созданием index проверяет существующие строки. При `COUNT(*) > 1` она останавливается с явной ошибкой и ничего не удаляет; исправление production duplicates остаётся отдельным approved data action.
- Удаление текущего GroupPage не запрещено: существующий CRUD contract позволяет затем создать новый singleton, но одновременно двух строк быть не может.
- После успешного member delete frontend перезагружает canonical Group read model, поэтому удалённая карточка не остаётся stale.
- Existing DATA-04 media coordinator продолжает отвечать за group/member photo create, replace, rollback и delete.

## RED → GREEN

| Сценарий | RED | GREEN |
|---|---|---|
| DB singleton | Без новой migration direct second insert завершался успешно; test ожидал `DbUpdateException` и упал, exit `1` | После migration second insert отклонён PostgreSQL constraint; test прошёл |
| Service/API singleton | Второй `POST /api/admin/group` возвращал `201 Created` | Повторная попытка возвращает `400 Bad Request`, row count остаётся `1`, duplicate Blob не создаётся |
| Member delete readback | API возвращал `204`, но карточка оставалась в DOM до ручного refresh | После `204` выполняется reload; карточка отсутствует в новом Group readback |

Targeted FEAT-06 integration run после исправлений: 2/2 passed.

## Real API/PostgreSQL/Azurite integration

Committed opt-in test `GroupCrudIntegrationTests` доказал:

- group create и public read;
- повторный group create → `400`, direct second DB row → constraint failure, row count `1`;
- group description edit и photo replace; old Blob удалён, new Blob существует;
- member create/read/edit и photo replace; old Blob удалён, new Blob существует;
- social create Instagram → edit Telegram → delete; каждое состояние прочитано через public member endpoint;
- member delete → отсутствие в Group readback, member endpoint `404`, Blob удалён;
- group delete → public Group endpoint `404`, Blob удалён;
- test-owned DB rows и Blobs удаляются в `finally`.

## Controlled desktop E2E и visual proof

WebKit, viewport `1440x900`, real API/Vite/PostgreSQL/Azurite и Basic Admin session:

1. Group description изменён с `feat06-browser-group-initial` на `feat06-browser-group-edited`; reload сохранил значение.
2. Group photo заменён; reload показал новое изображение.
3. Member создан, открыт, изменён и повторно открыт с новым full name, role и description.
4. Social создан как Instagram, изменён на Telegram и удалён; каждый modal readback показал новое состояние.
5. Первый delete дал browser RED: API `204`, stale card осталась. После frontend correction второй member delete автоматически перезагрузил страницу, карточка исчезла.

Before screenshot показывал исходные Group description/photo и отсутствие members. After screenshot показывает изменённые description/photo и отсутствие удалённого member; layout не менялся. На обоих скриншотах виден существующий desktop frame/nav overlap — это responsive/layout scope `MOB-01`/`MOB-03`, не regression FEAT-06.

Browser console также сохранил existing non-blocking findings:

- localhost не может загрузить production `https://storonnimv.com/site.webmanifest`;
- `Description.tsx` выдаёт WebKit `TypeError: The provided value is non-finite` внутри существующей animation;
- Swiper предупреждает о недостаточном числе slides для loop mode;
- initial anonymous `isAdmin` probe получает ожидаемый `401` до login.

CRUD/readback продолжал работать; эти findings не исправлялись скрыто и остаются для responsive/frontend quality задач.

## Финальные проверки

| Проверка | Результат |
|---|---|
| `dotnet restore StoronnimV.Server.sln --no-cache --disable-build-servers` | exit `0`; 5 projects restored; existing ImageSharp `NU1902`/`NU1903` |
| Release solution build | exit `0`; 0 errors, 2 existing advisory warnings |
| Full backend suite с `DATA04_INTEGRATION=1 FEAT04_INTEGRATION=1 FEAT05_INTEGRATION=1 FEAT06_INTEGRATION=1 API04_INTEGRATION=1` | exit `0`; 99/99 passed, 0 failed/skipped |
| `dotnet ef database update` repeat | exit `0`; `No migrations were applied. The database is already up to date.` |
| `dotnet ef migrations has-pending-model-changes` | exit `0`; model unchanged since last migration |
| FEAT-06 migration downgrade/upgrade rehearsal | pre-migration constraint test RED; migration reapplied successfully; final DB current |
| Existing-duplicate migration probe | 2 rows дали expected exit `1`/`P0001`; row count остался `2`, migration history `0`, index отсутствовал; после test cleanup migration применилась |
| Targeted changed-file ESLint | exit `0`, 0 findings |
| `VITE_API_URL=https://api.example.test/api npm run build` | exit `0`; 535 modules transformed |
| Production bundle scan | configured localhost endpoints и FEAT-06 fixture markers не найдены |
| Full `npm run lint` | exit `1`; existing 4 errors/6 warnings вне изменённого file; targeted lint green |
| Controlled WebKit E2E | group/member/social mutation readback и member delete RED/GREEN прошли |
| `git diff --check` и scoped secret scan | whitespace findings нет; найден только существующий документированный `local-only-change-me` template, real secrets/private keys/tokens не найдены |
| Disposable cleanup | browser fixture, PostgreSQL/Azurite rows/Blobs, API/Vite/browser processes и containers удалены |

Первый sandboxed `dotnet build` завис до compile output из-за local IPC restriction. Созданные этим сеансом build processes были остановлены; тот же build вне sandbox завершился за секунды с exit `0`. Первый cleanup вызов использовал неверно восстановленные disposable DB credentials, получил PostgreSQL authentication failure и затем был повторён с фактической container configuration; cleanup прошёл. Ни один случай не потребовал source correction.

## Критерии приёмки

| Критерий | Результат |
|---|---|
| Нельзя создать второй GroupPage | `passed`: service/API guard + PostgreSQL unique constraint |
| Existing duplicates не теряются молча | `passed`: migration aborts before index, rows не удаляются |
| Description/photo CRUD | `passed`: API/Blob assertions и browser reload readback |
| Member CRUD/photo | `passed`: API/DB/Blob lifecycle; browser create/edit/delete readback |
| Social create/edit/delete | `passed`: API и browser member-modal readback |
| Mutation видна после readback | `passed`: reload/public endpoint/404/blob absence assertions |
| Versions/multiple groups не добавлены | `passed` |

Все критерии `FEAT-06` выполнены. Статус: `done`. Следующая задача backlog — `FEAT-07`; она не начиналась. Коммит и branch change не выполнялись.
