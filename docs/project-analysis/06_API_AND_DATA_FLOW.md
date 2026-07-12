# API и потоки данных

## Базовые условия

Все controller routes имеют prefix `/api`. Public controllers ограничены policy `UserLimitPerMinute`; admin — JWT + `AdminLimitPerMinute`; SuperAdmin — role policy. `Request` ниже указывает binding shape, `Response` — declared/normal success shape. **Согласованность** оценивает найденный frontend consumer, а не runtime availability.

## API-контракты

| Method | Route | Backend handler | Request | Response | Frontend consumer | Назначение | Согласованность | Доказательство | Уверенность |
|---|---|---|---|---|---|---|---|---|---|
| POST | `/api/account/login` | `AccountController.LogIn` | `LogInRequest` JSON | `200 role` + cookie | `AdminContext.logIn` | вход | согласован статически | `AccountController.cs`, `AdminContext.tsx` | Подтверждено |
| GET | `/api/admin/isAdmin` | `AdminController.IsAdmin` | — | `200 true` | `GlobalContext.fetchIsAdmin` | server auth check | route совпадает | `AdminController.cs`, `GlobalContext.tsx` | Подтверждено |
| POST | `/api/admin/logout` | `AdminController.LogOut` | cookie | `200` | `Header.handleLogOut` | logout | требует cookie/domain | `AdminController.cs`, `Header.tsx` | Требует запуска |
| GET | `/api/home/news/{count}` | `HomeController.GetMainNews` | count | `NewsHomeResponse[]` | `HomeContext` (`6`) | home news | согласован | `HomeController.cs`, `HomeContext.tsx` | Подтверждено |
| GET | `/api/home/schedule` | `HomeController.GetNearestSchedule` | — | `ScheduleHomeResponse?` | `HomeContext` | ближайшее событие | nullability risk | те же файлы | Вероятно |
| GET | `/api/home/video` | `HomeController.GetPromotionVideo` | — | `VideoPageResponse?` | `HomeContext` | promo video | nullability risk | те же файлы | Вероятно |
| GET | `/api/group` | `GroupPageController.GetGroupPageInfo` | — | `GroupPageFullInfoResponse` | `GroupContext` | группа/участники | согласован | controller/context | Подтверждено |
| GET | `/api/group/member/{id}` | `GroupPageController.GetMember` | member id | `MemberFullInfoResponse` | `GroupContext` | detail участника | согласован | controller/context | Подтверждено |
| GET | `/api/group-socials` | `GroupSocialsController.GetAllGroupSocials` | — | `GroupSocialResponse[]` | `Footer` | socials footer | согласован | controller/Footer | Подтверждено |
| GET | `/api/group-socials/{id}` | `GetGroupSocial` | id | `GroupSocialResponse` | не найден | detail social | backend-only | `GroupSocialsController.cs` | Подтверждено |
| GET | `/api/music` | `MusicController.GetMusicPlatforms` | — | `MusicResponse[]` | `MusicContext` | platforms | согласован | controller/context | Подтверждено |
| GET | `/api/music/{id}` | `GetMusicPlatform` | id | `MusicResponse` | не найден | detail platform | backend-only | `MusicController.cs` | Подтверждено |
| GET | `/api/news/{id}` | `NewsController.GetNewsItem` | id | `NewsResponse` | `NewsContext` | detail news | согласован | controller/context | Подтверждено |
| GET | `/api/news/page/{page}` | `GetNewsForPage` | page + pageSize | paged `NewsShortResponse` | `NewsContext` | news list | согласован | controller/context | Подтверждено |
| GET | `/api/schedules/{id}` | `SchedulesController.GetSchedule` | id | `ScheduleResponse` | `ScheduleContext` | event detail | согласован | controller/context | Подтверждено |
| GET | `/api/schedules/page/{page}` | `GetSchedulesForPage` | page + pageSize | paged `ScheduleShortResponse` | `ScheduleContext` | schedule list | frontend ждёт extra `status` | DTO/interface | Подтверждено |
| GET | `/api/videos/{id}` | `VideoController.GetVideo` | id | `VideoPageResponse` | не найден | video detail | backend-only | `VideoController.cs` | Подтверждено |
| GET | `/api/videos/page/{type}/{page}` | `GetVideosForPage` | enum type/page/pageSize | paged videos | `VideoContext` | video list | согласован | controller/context | Подтверждено |
| GET | `/api/super-admin/basic-admins` | `GetAllBasicAdmins` | JWT role | `BasicAdminResponse[]` | `AdminContext` | list admins | SuperAdmin auth risk | controller/context | Вероятно сломан |
| POST | `/api/super-admin/basic-admins` | `CreateBasicAdmin` | JSON | `BasicAdminResponse` | `AdminContext.addAdmin` | create admin | route/body совпадают | controller/context | Подтверждено статически |
| DELETE | `/api/super-admin/basic-admins/{id}` | `DeleteBasicAdmin` | id | `204` | `AdminContext.deleteAdmin` | delete admin | UI state не обновляется | controller/context | Частично |
| PATCH | `/api/super-admin/basic-admins/{id}/login` | `EditBasicAdminLogin` | JSON | `BasicAdminResponse` | `AdminContext` | edit login | согласован | controller/context | Подтверждено статически |
| PATCH | `/api/super-admin/basic-admins/{id}/password` | `EditBasicAdminPassword` | JSON | `200` | `AdminContext` | edit password | frontend condition противоречит validator | context/validator | Вероятно сломан |
| DELETE | `/api/admin/news/{id}` | `DeleteNewsItem` | id | `204` | `DeleteNewsItemModal` | delete news | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/schedules/{id}` | `DeleteSchedule` | id | `204` | `DeleteScheduleModal` | delete schedule | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/videos/{id}` | `DeleteVideo` | id | `204` | `DeleteVideoModal` | delete video | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/group/{id}` | `DeleteGroup` | id | `204` | не найден | delete group page | backend-only | `AdminController.cs` | Подтверждено |
| DELETE | `/api/admin/group/members/{id}` | `DeleteMember` | id | `204` | `DeleteMemberModal` | delete member | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/music/{id}` | `DeleteMusicPlatform` | id | `204` | `DeleteMusicPlatformModal` | delete platform | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/socials/{id}` | `DeleteSocial` | id | `204` | `DeleteSocialModal` | delete social | согласован | controller/form | Подтверждено статически |
| DELETE | `/api/admin/group-socials/{id}` | `DeleteGroupSocial` | id | `204` | `DeleteGroupSocialModal` | delete group social | согласован | controller/form | Подтверждено статически |
| POST | `/api/admin/news` | `AddNewsItem` | multipart `NewsItemAdditionRequest` | `201` | `AddNewsItemModal` | create news | date format mismatch | service/form | Вероятно неверная дата |
| POST | `/api/admin/schedules` | `AddSchedule` | multipart | `201` | `AddScheduleModal` | create event | согласован статически | controller/form | Подтверждено статически |
| POST | `/api/admin/videos` | `AddVideo` | multipart | `201` | `AddVideoModal` | upload video | согласован статически | controller/form | Подтверждено статически |
| POST | `/api/admin/group` | `AddGroup` | multipart | `201` | не найден | create group page | backend-only; duplicates possible | controller/service | Риск |
| POST | `/api/admin/group/members` | `AddMember` | multipart | `201` | `AddMemberModal` | create member | согласован статически | controller/form | Подтверждено статически |
| POST | `/api/admin/music` | `AddMusicPlatform` | multipart | `201` | `AddMusicPlatformModal` | create platform | согласован статически | controller/form | Подтверждено статически |
| POST | `/api/admin/socials` | `AddSocial` | JSON `[FromBody]` | `201` | `AddSocialModal` | create social | FormData/JSON mismatch | controller/form | Вероятно сломан |
| POST | `/api/admin/group-socials` | `AddGroupSocial` | multipart | `201` | `AddGroupSocialModal` | create group social | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/news` | `UpdateNewsItem` | JSON `[FromBody]` | `204` | `EditNewsItemModal` | edit news | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/schedules` | `UpdateSchedule` | JSON | `204` | `EditScheduleModal` | edit event | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/videos` | `UpdateVideo` | JSON | `204` | `EditVideoModal` | edit video | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/group-pages` | `UpdateGroupPage` | JSON | `204` | `EditGroupModal` | edit group | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/group-pages/members` | `UpdateMember` | JSON | `204` | `EditMemberModal` | edit member | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/music-platforms` | `UpdateMusicPlatform` | JSON | `204` | `EditMusicPlatformModal` | edit platform | JSON body следует перепроверить | controller/form | Требует запуска |
| PATCH | `/api/admin/socials` | `UpdateSocial` | JSON | `204` | `EditSocialModal` | edit social | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/group-socials` | `UpdateGroupSocial` | JSON | `204` | `EditGroupSocialModal` | edit group social | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/news/photo` | `UpdateNewsItemPhoto` | multipart `PhotoEditRequest` | `204` | `EditNewsItemModal` | replace photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/news/delete-photo` | `DeleteNewsItemPhoto` | JSON long | `204` | `EditNewsItemModal` | detach/delete photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/schedules/photo` | `UpdateSchedulePhoto` | multipart | `204` | `EditScheduleModal` | replace photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/group-page/photo` | `UpdateGroupPhoto` | multipart | `204` | `EditGroupModal` | replace group photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/group-page/members/photo` | `UpdateMemberPhoto` | multipart | `204` | `EditMemberModal` | replace member photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/music-platforms/photo` | `UpdateMusicPlatformPhoto` | multipart | `204` | `EditMusicPlatformModal` | replace platform photo | согласован статически | controller/form | Подтверждено статически |
| PATCH | `/api/admin/news/video` | `UpdateNewsItemVideo` | JSON | `204` | `EditNewsItemModal` | attach video | FormData/JSON mismatch | controller/form | Вероятно сломан |
| PATCH | `/api/admin/news/delete-video` | `DeleteNewsItemVideo` | JSON long | `204` | `EditNewsItemModal` | detach video | согласован статически | controller/form | Подтверждено статически |

Endpoints без SPA consumer: three public detail GETs, create/delete group page и операционные `/health`, `/hangfire`, Swagger, `/`. Frontend URL+method без backend route не найдено.

## Поток 1: просмотр новостей

1. Пользователь открывает `/news`.
2. `NewsList` вызывает `NewsContext.paginate/fetchNews`.
3. Axios: `GET /api/news/page/{page}?pageSize=6`.
4. `NewsController.GetNewsForPage` → `NewsControllerService` → `NewsService`.
5. `NewsRepository` строит projection из PostgreSQL.
6. AutoMapper формирует paged response.
7. Context обновляет list/current/total и sessionStorage.
8. UI показывает cards; detail вызывает `GET /api/news/{id}` и global modal.

**Состояние:** статически реализован; production блокируется hardcoded localhost, runtime data не проверялись.

## Поток 2: admin content edit

1. Login отправляет credentials, backend выдаёт JWT cookie и role.
2. Public page вызывает `/admin/isAdmin` и показывает CRUD controls.
3. Edit form собирает локальное состояние/`FormData`.
4. Для многих PATCH endpoints frontend помечает FormData как JSON.
5. `[ApiController]` ожидает JSON `[FromBody]`; binding, вероятно, прекращает запрос с 400/415.
6. Поэтому service/repository/blob часто не достигаются.

**Состояние:** частично реализован, девять mutation contracts вероятно сломаны; требует integration test.

## Поток 3: media create/update

1. Admin выбирает file в FormData.
2. `[FromForm]` endpoint передаёт request в controller service/entity service.
3. Service создаёт/обновляет metadata и вызывает `BlobRepository`.
4. Azure upload возвращает URL, EF сохраняет его.
5. UI обычно reload-ит страницу.

**Риск:** DB и Blob не образуют одну transaction; partial rows/orphan files возможны. File validation ограничена.

## Поток 4: ежедневное обновление афиш

1. `Program.cs` регистрирует daily Hangfire recurring job.
2. `ScheduleStatusUpdaterService` вызывает `ScheduleService.UpdateStatusesAsync`.
3. Service выбирает active expired schedules.
4. `List.ForEach(async ...)` запускает updates без await.
5. Job может завершиться и dispose-ить scoped dependencies до завершения writes.

**Состояние:** вероятно ненадёжен; требует runtime job test.
