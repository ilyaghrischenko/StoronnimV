\set ON_ERROR_STOP on

SELECT 1 / CASE
    WHEN LENGTH(:'source_photo_base') > 0
        AND LENGTH(:'target_photo_base') > 0
        AND :'source_photo_base' <> :'target_photo_base'
        AND RIGHT(:'source_photo_base', 1) <> '/'
        AND RIGHT(:'target_photo_base', 1) <> '/'
    THEN 1 ELSE 0
END;

SELECT 1 / CASE
    WHEN LENGTH(:'source_video_base') > 0
        AND LENGTH(:'target_video_base') > 0
        AND :'source_video_base' <> :'target_video_base'
        AND RIGHT(:'source_video_base', 1) <> '/'
        AND RIGHT(:'target_video_base', 1) <> '/'
    THEN 1 ELSE 0
END;

BEGIN;

UPDATE "GroupPages"
SET "PhotoUrl" = :'target_photo_base' || SUBSTRING("PhotoUrl" FROM LENGTH(:'source_photo_base') + 1)
WHERE "PhotoUrl" LIKE :'source_photo_base' || '/%';

UPDATE "GroupSocials"
SET "PhotoUrl" = :'target_photo_base' || SUBSTRING("PhotoUrl" FROM LENGTH(:'source_photo_base') + 1)
WHERE "PhotoUrl" LIKE :'source_photo_base' || '/%';

UPDATE "Members"
SET "PhotoUrl" = :'target_photo_base' || SUBSTRING("PhotoUrl" FROM LENGTH(:'source_photo_base') + 1)
WHERE "PhotoUrl" LIKE :'source_photo_base' || '/%';

UPDATE "MusicPlatforms"
SET "BgImageUrl" = :'target_photo_base' || SUBSTRING("BgImageUrl" FROM LENGTH(:'source_photo_base') + 1)
WHERE "BgImageUrl" LIKE :'source_photo_base' || '/%';

UPDATE "NewsItems"
SET "Photo" = :'target_photo_base' || SUBSTRING("Photo" FROM LENGTH(:'source_photo_base') + 1)
WHERE "Photo" LIKE :'source_photo_base' || '/%';

UPDATE "Schedules"
SET "Photo" = :'target_photo_base' || SUBSTRING("Photo" FROM LENGTH(:'source_photo_base') + 1)
WHERE "Photo" LIKE :'source_photo_base' || '/%';

UPDATE "Videos"
SET "Url" = :'target_video_base' || SUBSTRING("Url" FROM LENGTH(:'source_video_base') + 1)
WHERE "Url" LIKE :'source_video_base' || '/%';

COMMIT;
