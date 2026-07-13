WITH media_urls AS (
    SELECT "PhotoUrl" AS url FROM "GroupPages"
    UNION ALL
    SELECT "PhotoUrl" AS url FROM "GroupSocials"
    UNION ALL
    SELECT "PhotoUrl" AS url FROM "Members"
    UNION ALL
    SELECT "BgImageUrl" AS url FROM "MusicPlatforms"
    UNION ALL
    SELECT "Photo" AS url FROM "NewsItems"
    UNION ALL
    SELECT "Photo" AS url FROM "Schedules"
    UNION ALL
    SELECT "Url" AS url FROM "Videos"
), metrics AS (
    SELECT 'entity.Admins' AS metric, COUNT(*)::bigint AS value FROM "Admins"
    UNION ALL
    SELECT 'entity.GroupPages', COUNT(*)::bigint FROM "GroupPages"
    UNION ALL
    SELECT 'entity.GroupSocials', COUNT(*)::bigint FROM "GroupSocials"
    UNION ALL
    SELECT 'entity.Members', COUNT(*)::bigint FROM "Members"
    UNION ALL
    SELECT 'entity.MusicPlatforms', COUNT(*)::bigint FROM "MusicPlatforms"
    UNION ALL
    SELECT 'entity.NewsItems', COUNT(*)::bigint FROM "NewsItems"
    UNION ALL
    SELECT 'entity.Schedules', COUNT(*)::bigint FROM "Schedules"
    UNION ALL
    SELECT 'entity.Socials', COUNT(*)::bigint FROM "Socials"
    UNION ALL
    SELECT 'entity.Videos', COUNT(*)::bigint FROM "Videos"
    UNION ALL
    SELECT 'media.references.nonempty', COUNT(*)::bigint
    FROM media_urls
    WHERE url IS NOT NULL AND BTRIM(url) <> ''
    UNION ALL
    SELECT 'media.urls.distinct', COUNT(DISTINCT url)::bigint
    FROM media_urls
    WHERE url IS NOT NULL AND BTRIM(url) <> ''
    UNION ALL
    SELECT 'media.video_blob_names.nonempty', COUNT(*)::bigint
    FROM "Videos"
    WHERE BTRIM("BlobName") <> ''
    UNION ALL
    SELECT 'schema.migrations', COUNT(*)::bigint FROM "__EFMigrationsHistory"
)
SELECT metric, value
FROM metrics
ORDER BY metric;
