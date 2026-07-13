# План проверки

## Принципы

- Проверка выполняется на clean checkout и документированном toolchain.
- Production данные до backup не изменяются.
- Наличие кода не считается доказательством: каждый обязательный сценарий имеет наблюдаемый результат.
- Автоматические проверки дают repeatability; визуальные и реальные external integrations проверяются вручную.

## Автоматические проверки

### Frontend

- TypeScript/Vite production build: `npm run build`.
- ESLint: `npm run lint` без errors и без оставленных correctness warnings.
- Компонентные тесты: route guard, auth states, validation, loading/error/empty, pagination и shared modal.
- Проверка bundle на отсутствие `localhost:44315`, secrets и development endpoints.
- Automated accessibility checks для основных pages/modals/forms.

### Backend

- `dotnet restore` и `dotnet build backend/StoronnimV.Server/StoronnimV.Server.sln`.
- `dotnet test backend/StoronnimV.Server/StoronnimV.Server.sln`.
- Service tests: dates/statuses, `GroupPage` singleton, role restrictions, file validation и Hangfire completion/idempotence.
- Controller/contract tests: JSON/multipart binding, validation/error response, nullable Home fields и Schedule status.
- Auth matrix: anonymous, Basic Admin, SuperAdmin, invalid/expired token и logout.

### Data и migrations

- Применение всех migrations к пустой PostgreSQL отдельной командой.
- Повторная migration command не меняет актуальную schema.
- Проверка unique/login и singleton invariants.
- Для M1: backup/restore локального DATA-02 corpus, Blob copy через Azurite, сверка entity counts, Blob inventory/checksums и public local URL samples.
- Для M5: отдельный rehearsal фактического production content source после его выбора.

## Интеграционные проверки

### API

- Public GET для Home, News, Schedule, Group, Music, Video и group socials.
- Pagination: valid, empty, out-of-range и invalid page/pageSize.
- Basic Admin login → `isAdmin` → one mutation per content type → public readback → logout.
- SuperAdmin list/create/edit/delete Basic Admin и запрет mutation SuperAdmin record.
- Content forms: create/edit/delete text and optional media.
- Unified error responses для validation, unauthorized, forbidden, not found, unsupported media и server failure.

### Forms и media

- Required/invalid/long fields и server validation отображаются у соответствующего input.
- Uploads: allowed type at/below limit; oversized; wrong extension/MIME/signature; interrupted upload.
- Create/replace/delete проверяют DB metadata и Blob state.
- Fault injection между DB и Blob подтверждает выбранную compensation strategy.
- Promotion video остаётся доступным при failed replacement.

### Background

- Несколько просроченных Active schedules становятся Passed до завершения job.
- Неистёкшие records не меняются.
- Повторный job idempotent.
- Production-mode dashboard route отсутствует или возвращает недоступность.

## Browser E2E

### Visitor

1. Открыть Home и перейти по каждой navigation link.
2. Проверить real/empty/error/loading для Home sections.
3. Пройти News и Schedule pagination/detail; проверить map.
4. Открыть participant details/socials, music links и Spotify.
5. Открыть три video categories, pagination и playback.
6. Открыть `/developers`, `/error`, неизвестный route и каждый direct deep link.

### Basic Admin

1. Login, refresh и server session detection.
2. По одному create/edit/delete/readback для каждого content type.
3. Upload/replace/delete representative photo и video.
4. Logout и подтверждение закрытия mutations.

### SuperAdmin

1. Доступ к Basic Admin screen только с server-confirmed role.
2. Create, edit login/password и delete Basic Admin.
3. Попытка затронуть SuperAdmin отклоняется.
4. Подмена `sessionStorage.role` не предоставляет доступ.

## Mobile и desktop matrix

| Width | Основная цель |
|---:|---|
| 320 px | Минимальный телефон: navigation, forms, modal, media, no overflow |
| 375 px | Типичный телефон и touch targets |
| 768 px | Tablet portrait и breakpoint transitions |
| 1024 px | Tablet landscape/compact desktop |
| 1440 px | Desktop baseline regression |

Для каждой ширины проверяются Home, Schedule, News, Music, Group, обе Video pages, Admin login, Basic Admin management, Developers, Error, shared header/footer/modal и все CRUD forms. На mobile/tablet выполняются portrait/landscape проверки для navigation, media и admin table.

Браузеры: актуальные стабильные Chrome, Safari, Firefox и Edge; фактические версии записываются в release evidence.

## Доступность

- Полный keyboard path без mouse.
- Logical tab order и видимый focus.
- Modal focus trap, Escape, close name и возврат focus.
- Form labels/errors связаны программно.
- Content images имеют полезный `alt`; decorative images скрыты от assistive tech.
- Touch targets не требуют точного hover/click.
- Проверка contrast и `prefers-reduced-motion`.

## SEO и performance

- Ukrainian `lang`, canonical, route title/description, Open Graph и favicon/manifest URLs.
- Direct links возвращают SPA, неизвестные URLs дают корректный 404 experience.
- Main public routes проверяются на layout shift, image sizing, lazy/preload policy и тяжёлый media traffic.
- Production bundle/API responses используют compression и не содержат development URLs.

## Security

- Controlled CSRF attempt для cookie-authenticated mutation.
- CORS отклоняет неизвестный origin и принимает только configured origins.
- Dashboard недоступен в production.
- Rate limit проверяется от двух независимых clients.
- Upload path не принимает executable/polyglot/oversized input.
- Exception response не раскрывает stack, connection strings или internal paths.
- Secret scan tracked files и generated artifacts.

## Проверки реального окружения

Требуют production/staging access и выполняются только в M5/M6:

- DNS/TLS, exact origins, cookie domain/SameSite и CORS.
- PostgreSQL/Blob backup, migration level, ACL и restore.
- CI artifact соответствует deployed commit.
- `/health`, public routes, controlled admin mutation/readback и external embeds.
- Hangfire job registration и отсутствие dashboard.
- Rollback доступен до начала smoke test.

## Release gate

Release разрешён, когда frontend/backend builds, tests, lint, integration/E2E, device matrix и production smoke green; все findings зарегистрированы; P0/P1 отсутствуют; документация соответствует deployed commit; владелец подтвердил финальный checklist.
