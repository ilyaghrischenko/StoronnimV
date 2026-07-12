# Модуль Media

## Назначение и границы

Модуль хранит/показывает photos и videos для news, schedule, group, members, music platforms и video page. Azure хранит files, PostgreSQL — URLs/names/relations, React — playback/rendering.

## Точки входа и ключевые файлы

`BlobRepository.cs`, `IBlobRepository.cs`, `ImageResizerService.cs`, media methods `AdminController`, entity services News/Schedule/Group/Member/Music/Video, frontend media forms/components.

## Основные сущности и структура

`Video` (URL, BlobName, Title, Type), `News.Video?`, photo URL/string fields в content entities. Containers используются для photo/video; frontend использует native video/embeds/images.

## Зависимости

- **Входящие:** admin multipart/JSON requests; public reads.
- **Исходящие:** Azure Blob, EF/PostgreSQL, browser media/network.
- **Связи:** Admin/auth защищает mutations; Home выбирает promotion video.

## Основной поток

FormData → `[FromForm]` controller → service → Blob upload/delete + EF metadata → public response URL → image/video UI.

## Реализовано

Upload/overwrite/delete, URL retrieval, entity-specific add/replace/delete actions, video categories и promotion flow.

## Незавершено и риски

DB/Blob не atomic; replace может delete first; ImageResizer unused; no size/MIME/signature policy; delete-by-name enumerates container; public URL зависит от external ACL; hardcoded category images; weak responsive media/accessibility.

## Неизвестно

Azure account/container ACL, quotas, orphan files, real aspect ratios, accepted formats, CDN/cache policy.

## Порядок чтения

Blob contract/adapter → one entity media service → Admin media endpoints/DTO → EF fields → matching frontend form/rendering.

## Доказательства

`Infrastructure/Repositories/AzureBlobStorage/BlobRepository.cs`; `Application/Services/Entities/*Service.cs`; `AdminController.cs`; frontend `elements/*/forms`.
