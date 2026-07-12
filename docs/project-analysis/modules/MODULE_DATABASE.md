# Модуль Database

## Назначение и границы

PostgreSQL хранит content/admin metadata и служит storage Hangfire. Модуль охватывает `StoronnimVContext`, EF repositories, projections и migrations; Blob file contents не входят.

## Точки входа и ключевые файлы

- `Infrastructure/StoronnimVContext.cs`;
- `Infrastructure/Repositories/Database/*`;
- `Infrastructure/Migrations/StoronnimVContextModelSnapshot.cs`;
- `WebApplicationBuilderExtensions.AddDbContext/AddHangfire/AddHealthChecks`.

## Основные сущности и структура

9 tables: Admins, GroupPages, GroupSocials, Members, Socials, NewsItems, Schedules, MusicPlatforms, Videos. `Member 1—N Social` cascade; `News 0—1 Video` optional.

## Зависимости

- **Входящие:** Application entity services через Domain contracts; Hangfire; health checks.
- **Исходящие:** PostgreSQL via Npgsql.
- **Связи:** media metadata содержит Blob URL/name.

## Основной поток

Service запрашивает repository → EF projection/query/update → PostgreSQL → projection/AutoMapper response.

## Реализовано

Generic + specialized repositories, pagination/projections, 24 migrations и current snapshot.

## Незавершено и риски

Startup migrations/seed отсутствуют; no unique Admin login index, no concurrency tokens, `UpdatedAt` не меняется, singleton GroupPage не enforced, DB/Blob не atomic, Windows HintPath portability risk.

## Неизвестно

Applied production migration, backups, data volume/quality, indexes/performance, existing SuperAdmin.

## Порядок чтения

Context → snapshot/relationships → Domain entities/projections → generic repository → feature repositories → services → migrations chronology only when needed.

## Доказательства

`StoronnimVContext.cs`; model snapshot; `Repository.cs`; `AdminRepository.cs`; `GroupPageRepository.cs`; `DatabaseInitializer.cs`.
