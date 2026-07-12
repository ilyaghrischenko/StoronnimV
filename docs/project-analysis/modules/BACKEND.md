# Модуль Backend

## Назначение и границы

Backend обслуживает public content reads, identity/admin mutations, data/media access, background updates и operational endpoints. Граница — solution `backend/StoronnimV.Server`.

## Точки входа и ключевые файлы

`StoronnimV.Api/Program.cs`, `Extensions/WebApplicationBuilderExtensions.cs`, `Controllers/*.cs`, затем Application services и Infrastructure repositories.

## Основные сущности и структура

Api → Application → Domain contracts/entities; Infrastructure реализует PostgreSQL/Azure adapters. Controller services отделяют HTTP orchestration от entity services.

## Зависимости

- **Входящие:** React API requests, Hangfire scheduler, health probes.
- **Исходящие:** PostgreSQL, Azure Blob, logs.
- **Связи:** JSON/multipart DTO contracts с frontend; EF projections/mappings к responses.

## Основной поток

Controller → controller service → entity/home/identity service → repository → DB/Blob → AutoMapper response.

## Реализовано

55 endpoints, DI, validation для account credentials, JWT/cookies, 9-entity model, 24 migrations, Blob adapter, Hangfire, rate limit, health, Swagger, Dockerfile.

## Незавершено и риски

No backend build/runtime proof; SuperAdmin auth pipeline, contract mismatches, non-awaited job, non-atomic DB/Blob, open dashboard, CSRF/file validation, no tests/seed/migration workflow.

## Неизвестно

Cloud resources/schema/ACL, deployed auth/CORS/cookies, Docker/health/job behavior.

## Порядок чтения

Program/DI → controllers → controller services/DTO → entity services → Domain → EF/blob → frontend consumers.

## Доказательства

Solution/csproj; `Program.cs`; DI extensions; controllers; services; context/snapshot/repositories. Подробности: [../05_BACKEND.md](../05_BACKEND.md).
