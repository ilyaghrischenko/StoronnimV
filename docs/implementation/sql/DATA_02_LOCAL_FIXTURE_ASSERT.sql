\set ON_ERROR_STOP on

SELECT 1 / CASE
    WHEN LENGTH(:'photo_base') > 0
        AND LENGTH(:'video_base') > 0
        AND RIGHT(:'photo_base', 1) <> '/'
        AND RIGHT(:'video_base', 1) <> '/'
    THEN 1 ELSE 0
END;

SELECT 1 / CASE
    WHEN (SELECT COUNT(*) FROM "Admins") = 0
        AND (SELECT COUNT(*) FROM "GroupPages") = 1
        AND (SELECT COUNT(*) FROM "GroupSocials") = 1
        AND (SELECT COUNT(*) FROM "Members") = 1
        AND (SELECT COUNT(*) FROM "MusicPlatforms") = 1
        AND (SELECT COUNT(*) FROM "NewsItems") = 1
        AND (SELECT COUNT(*) FROM "Schedules") = 1
        AND (SELECT COUNT(*) FROM "Socials") = 1
        AND (SELECT COUNT(*) FROM "Videos") = 4
    THEN 1 ELSE 0
END;

WITH media_urls AS (
    SELECT "PhotoUrl" AS url FROM "GroupPages"
    UNION ALL
    SELECT "PhotoUrl" FROM "GroupSocials"
    UNION ALL
    SELECT "PhotoUrl" FROM "Members"
    UNION ALL
    SELECT "BgImageUrl" FROM "MusicPlatforms"
    UNION ALL
    SELECT "Photo" FROM "NewsItems"
    UNION ALL
    SELECT "Photo" FROM "Schedules"
    UNION ALL
    SELECT "Url" FROM "Videos"
)
SELECT 1 / CASE
    WHEN (SELECT COUNT(*) FROM media_urls WHERE url IS NOT NULL AND BTRIM(url) <> '') = 10
        AND (SELECT COUNT(DISTINCT url) FROM media_urls WHERE url IS NOT NULL AND BTRIM(url) <> '') = 2
        AND (SELECT COUNT(*) FROM media_urls WHERE url = :'photo_base' || '/data-02-photo.jpg') = 6
        AND (SELECT COUNT(*) FROM media_urls WHERE url = :'video_base' || '/data-02-promotion.mp4') = 4
    THEN 1 ELSE 0
END;

SELECT 1 / CASE
    WHEN (SELECT COUNT(*) FROM "Videos" WHERE "BlobName" = 'data-02-promotion.mp4') = 4
        AND (SELECT COUNT(DISTINCT "Type") FROM "Videos") = 4
        AND (SELECT MIN("Type") FROM "Videos") = 0
        AND (SELECT MAX("Type") FROM "Videos") = 3
    THEN 1 ELSE 0
END;
