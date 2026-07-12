# Модуль Deployment

## Назначение и границы

Модуль описывает имеющиеся artifacts для backend container и frontend Azure Static Web Apps. Реального CI/CD workflow или infrastructure-as-code нет.

## Точки входа и ключевые файлы

`StoronnimV.Api/Dockerfile`, `.dockerignore`, frontend `staticwebapp.config.json`, `.env.production` (только имя API variable), `launchSettings.json`, appsettings keys.

## Основные сущности и структура

Multi-stage .NET 9 image restore/build/publish; static SPA routes с explicit rewrites; runtime env передаётся извне.

## Зависимости

- **Входящие:** build agent/cloud settings/secrets/domain/DNS/TLS.
- **Исходящие:** container registry/host, Azure Static Web Apps, PostgreSQL, Blob.
- **Связи:** exact frontend origin должен совпасть с backend CORS; cookie domain/SameSite/HTTPS — с обоими hosts.

## Основной поток

Frontend source → Vite bundle → static host; backend source → Docker image → runtime env → API/DB/Blob. Мigrations требуют отдельного, не найденного шага.

## Реализовано

Dockerfile, frontend route rewrites, health endpoint, production env variable name, Development Swagger.

## Незавершено и риски

No CI/CD, IaC, migration/seed step, secret contract doc, rollback/backup/monitoring. Frontend ignores production URL. Static fallback incomplete. Docker build/run не проверен; committed dist может быть stale.

## Неизвестно

Current providers/resources, deployed commit, DNS/TLS, reverse proxy, dashboard exposure, secret storage/rotation, backup/restore/SLA.

## Порядок чтения

Frontend hardcoded URL/static routes → backend env reads/CORS/cookie → Dockerfile → health/Hangfire → external deployment settings у владельца.

## Доказательства

Dockerfile; `staticwebapp.config.json`; `GlobalContext.tsx`; `Program.cs`; `WebApplicationBuilderExtensions.cs`; отсутствие `.github/workflows` в tracked files.
