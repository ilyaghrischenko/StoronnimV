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
        AND (SELECT COUNT(*) FROM "GroupPages") = 0
        AND (SELECT COUNT(*) FROM "GroupSocials") = 0
        AND (SELECT COUNT(*) FROM "Members") = 0
        AND (SELECT COUNT(*) FROM "MusicPlatforms") = 0
        AND (SELECT COUNT(*) FROM "NewsItems") = 0
        AND (SELECT COUNT(*) FROM "Schedules") = 0
        AND (SELECT COUNT(*) FROM "Socials") = 0
        AND (SELECT COUNT(*) FROM "Videos") = 0
    THEN 1 ELSE 0
END;

BEGIN;

INSERT INTO "GroupPages" ("PhotoUrl", "Description", "CreatedAt", "UpdatedAt")
VALUES (
    :'photo_base' || '/data-02-photo.jpg',
    'Локальний опис гурту для перевірки DATA-02.',
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
);

INSERT INTO "GroupSocials" ("PhotoUrl", "Name", "LinkUrl", "CreatedAt", "UpdatedAt")
VALUES (
    :'photo_base' || '/data-02-photo.jpg',
    1,
    'https://example.com/storonnimv-local',
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
);

INSERT INTO "Members" ("PhotoUrl", "FullName", "Description", "Role", "CreatedAt", "UpdatedAt")
VALUES (
    :'photo_base' || '/data-02-photo.jpg',
    'Тестовий учасник',
    'Локальний учасник для перевірки сторінки гурту.',
    'Вокал',
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
);

INSERT INTO "Socials" ("MemberId", "Url", "Type", "CreatedAt", "UpdatedAt")
SELECT
    "Id",
    'https://example.com/storonnimv-member-local',
    1,
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
FROM "Members"
WHERE "FullName" = 'Тестовий учасник';

INSERT INTO "MusicPlatforms" ("BgImageUrl", "PlatformUrl", "CreatedAt", "UpdatedAt")
VALUES (
    :'photo_base' || '/data-02-photo.jpg',
    'https://open.spotify.com/',
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
);

INSERT INTO "Videos" ("Url", "BlobName", "Title", "Type", "CreatedAt", "UpdatedAt")
VALUES
    (:'video_base' || '/data-02-promotion.mp4', 'data-02-promotion.mp4', 'Локальний виступ', 0, TIMESTAMPTZ '2026-07-13 12:00:00+00', TIMESTAMPTZ '2026-07-13 12:00:00+00'),
    (:'video_base' || '/data-02-promotion.mp4', 'data-02-promotion.mp4', 'Локальний backstage', 1, TIMESTAMPTZ '2026-07-13 12:00:00+00', TIMESTAMPTZ '2026-07-13 12:00:00+00'),
    (:'video_base' || '/data-02-promotion.mp4', 'data-02-promotion.mp4', 'Локальна репетиція', 2, TIMESTAMPTZ '2026-07-13 12:00:00+00', TIMESTAMPTZ '2026-07-13 12:00:00+00'),
    (:'video_base' || '/data-02-promotion.mp4', 'data-02-promotion.mp4', 'Локальне промо', 3, TIMESTAMPTZ '2026-07-13 12:00:00+00', TIMESTAMPTZ '2026-07-13 12:00:00+00');

INSERT INTO "NewsItems" ("Title", "Description", "Photo", "VideoId", "Priority", "Date", "CreatedAt", "UpdatedAt")
SELECT
    'Локальна новина DATA-02',
    'Тестова новина для перевірки локального content flow.',
    :'photo_base' || '/data-02-photo.jpg',
    "Id",
    0,
    DATE '2026-07-13',
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
FROM "Videos"
WHERE "Type" = 3;

INSERT INTO "Schedules" ("Title", "PerformanceDateTime", "Description", "Location", "Photo", "Status", "CreatedAt", "UpdatedAt")
VALUES (
    'Локальний концерт DATA-02',
    TIMESTAMPTZ '2026-08-01 18:00:00+00',
    'Тестова афіша для локальної перевірки.',
    'Local Test Venue',
    :'photo_base' || '/data-02-photo.jpg',
    0,
    TIMESTAMPTZ '2026-07-13 12:00:00+00',
    TIMESTAMPTZ '2026-07-13 12:00:00+00'
);

COMMIT;
