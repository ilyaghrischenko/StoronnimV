# Модуль Admin

## Назначение и границы

Admin покрывает login/logout, появление content controls, protected mutations и SuperAdmin management basic accounts. Он пересекает SPA, JWT/cookies/CORS, controllers/services и Admin table.

## Точки входа и ключевые файлы

Frontend: `AdminContext.tsx`, `AuthForm.tsx`, `ProtectedRoute.tsx`, `BasicAdmins.tsx`, all feature forms. Backend: `AccountController`, `AdminController`, `SuperAdminController`, identity/admin services, validators, JWT/cookie settings.

## Основные сущности и структура

`Admin` с `AdminType.Basic|SuperAdmin`; JWT name/role claims; HttpOnly `Token` cookie; frontend `isAdmin` + `sessionStorage.role`.

## Зависимости

- **Входящие:** credentials/admin actions.
- **Исходящие:** PostgreSQL, Blob, all content services.
- **Связи:** CORS/HTTPS/cookie domain и rate limits обязательны для browser flow.

## Основной поток

Login → password hash verify → JWT cookie + role response → frontend stores role/checks `isAdmin` → content form → protected endpoint → service/repositories.

## Реализовано

Login/logout, JWT generation, Admin/SuperAdmin authorization declarations, account validators, 34 content/admin endpoints + 5 SuperAdmin endpoints, broad CRUD UI.

## Незавершено и риски

SuperAdmin role policy without `UseAuthentication`; client guard trusts sessionStorage; no seed/bootstrap; 9 body mismatches; impossible password condition; basic-admin Type not checked; no antiforgery; no auth tests; login enumeration/raw errors.

## Неизвестно

Existing admin accounts, deployed cookie/CORS behavior, expected admin scope/mobile access, identity recovery/rotation process.

## Порядок чтения

AuthForm/AdminContext → Account controller/services → JWT/cookie/CORS → AdminController + one form → ProtectedRoute/SuperAdmin → admin services/repository/validators.

## Доказательства

Перечисленные files; API matrix в [../06_API_AND_DATA_FLOW.md](../06_API_AND_DATA_FLOW.md).
