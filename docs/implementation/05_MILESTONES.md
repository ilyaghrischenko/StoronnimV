# Milestones

## M1 — Воспроизводимый локальный запуск

- **Цель:** доказать сборку и один public vertical на безопасной копии данных.
- **Задачи:** `BASE-01`, `BASE-02`, `DATA-01`, `DATA-02`, `BASE-03`, `BASE-04`, `QA-01`.
- **Вход:** clean repository, .NET 9 и Node/npm.
- **Выход:** документированный startup, green builds, schema, non-production content/media, browser public smoke.
- **Демонстрация:** Home/News получают реальные данные через локально настроенный API.
- **Блокеры:** доступ к backup; PostgreSQL/Blob development resources.
- **Принятие:** другой сеанс повторяет запуск без догадок и production mutation.
- **Можно отложить:** admin, mobile polish, production hosting.

## M2 — Функционально завершённая desktop-версия

- **Цель:** закрыть auth, CRUD, media и background flows на desktop.
- **Задачи:** `API-01`, `DATA-03`, `API-02`, `FEAT-01`, `FEAT-02`, `API-03`, `DATA-04`, `FEAT-03`, `FEAT-04`, `FEAT-05`, `API-04`, `FEAT-06`, `FEAT-07`, `FEAT-08`, `FEAT-09`, `FEAT-10`.
- **Вход:** M1 принят.
- **Выход:** visitor, Basic Admin и SuperAdmin desktop scenarios работают end-to-end.
- **Демонстрация:** login, изменение каждого типа контента, readback, media lifecycle, account management и schedule job.
- **Блокеры:** действующая non-production SuperAdmin запись; upload policy draft.
- **Принятие:** desktop checklist проходит с реальными данными без P0/P1 defects.
- **Можно отложить:** responsive layout и production deploy.

## M3 — Функционально завершённая мобильная версия

- **Цель:** сделать тот же обязательный объём usable на телефоне и планшете.
- **Задачи:** `MOB-01`, `MOB-02`, `MOB-03`, `MOB-04`, `MOB-05`, `MOB-06`.
- **Вход:** M2 принят; desktop runtime CSS зафиксирован baseline.
- **Выход:** public и admin flows работают на 320, 375, 768 и 1024 px.
- **Демонстрация:** visitor и admin проходят ключевые сценарии touch/keyboard без overflow.
- **Блокеры:** неизвестные реальные aspect ratios media.
- **Принятие:** cross-device checklist и screenshots не имеют блокирующих расхождений.
- **Можно отложить:** декоративная pixel-perfect frame на mobile.

## M4 — Интеграционно проверенная версия

- **Цель:** превратить рабочие сценарии в повторяемые quality gates.
- **Задачи:** `QA-02`, `QA-03`, `QA-04`, `QA-05`.
- **Вход:** M3 принят.
- **Выход:** tests, lint/build, E2E и security/accessibility/performance evidence.
- **Демонстрация:** clean run всех автоматических gates и ручной audit report.
- **Блокеры:** стабильные test dependencies и fixtures.
- **Принятие:** green gates; findings P0/P1 устранены или возвращены в backlog.
- **Можно отложить:** P3 visual polish.

## M5 — Готовность к deployment

- **Цель:** выбрать production topology и безопасно выпустить проверенную версию.
- **Задачи:** `OPS-01`, `OPS-02`, `OPS-03`, `CLEAN-01`, `DOC-01`, `OPS-04`.
- **Вход:** M4 принят; hosting/access/backup подтверждены.
- **Выход:** CI, environment contract, migration rehearsal, rollback, deployment и актуальные runbooks.
- **Демонстрация:** staging/rehearsal deploy тем же процессом, затем production deploy.
- **Блокеры:** внешний OPEN-001 и доступы владельца.
- **Принятие:** deployment успешен, migrations подтверждены, secrets не раскрыты, dashboard закрыт.
- **Можно отложить:** новая стратегия логирования и analytics.

## M6 — Финальный release candidate

- **Цель:** подтвердить логическое завершение на production.
- **Задачи:** `QA-06`, `QA-07`.
- **Вход:** M5 принят.
- **Выход:** production smoke evidence, итоговый audit и owner acceptance.
- **Демонстрация:** утверждённые visitor/admin сценарии на production и device matrix.
- **Блокеры:** реальный контент, внешние embeds и принимающий владелец.
- **Принятие:** нет P0/P1; владелец подтверждает checklist.
- **Можно отложить:** только явно зарегистрированные P2/P3 post-release items.
