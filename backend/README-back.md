# StoronnimV.Server

[![Built with .NET 9](https://img.shields.io/badge/.NET-9.0-512bd4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

The backend infrastructure for the "Storonnim V" multimedia portal. This system manages news feeds, concert schedules, band member profiles, and high-performance media storage.

## 🔗 Related Projects
* **Frontend (React):** [StoronnimV.Client](https://github.com/ilyaghrischenko/StoronnimV.Client/tree/main)

## 🛠 Tech Stack

* **Framework:** ASP.NET Core 9.0
* **Database:** Entity Framework Core with PostgreSQL
* **Background Processing:** Hangfire (for automated schedule status updates)
* **Storage:** Azure Blob Storage for media assets
* **Security:** JWT Bearer Authentication & Rate Limiting
* **Observability:** Serilog & ASP.NET Core Health Checks

## 🏗 Architecture

The project follows **Clean Architecture** principles:
- **Core (Domain):** Pure business entities and repository abstractions.
- **Application:** Business logic, DTOs, and AutoMapper profiles.
- **Infrastructure:** Database context, repository implementations, and external integrations (Blob Storage).
- **Presentation (API):** REST Controllers, Middlewares, and Swagger documentation.

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK (exact patch is not pinned)
- PostgreSQL (exact server version is not pinned)
- A separate non-production Azure Blob Storage resource for media operations

### Configuration
Use the canonical [runtime contract](../docs/implementation/10_RUNTIME_CONTRACT.md) and copy `StoronnimV.Server/StoronnimV.Api/.env.example` to an untracked `.env`. It lists the names actually read by the code and safe local placeholders. `VITE_API_URL` is not active yet.

### Running the App

Build is validated by `BASE-02`. Use the explicit [migration workflow](../docs/implementation/11_MIGRATION_WORKFLOW.md) for schema changes; API startup remains a separate `BASE-03` check.

## 📈 Key Features

* **Automated Workflows:** Daily background jobs to sync concert statuses.
* **Media Management:** Integrated image resizing and cloud storage.
* **Robust Security:** Custom exception handling middleware and request throttling.

---

## 3. Ukrainian Version: README.md

[![Побудовано на .NET 9](https://img.shields.io/badge/.NET-9.0-512bd4)](https://dotnet.microsoft.com/)
[![Архітектура](https://img.shields.io/badge/Architecture-Clean-green)](#архітектура)

Бекенд-інфраструктура для мультимедійного порталу гурту "Стороннім В". Система забезпечує керування новинами, розкладом концертів, профілями учасників та медіа-контентом.

## 🔗 Пов'язані проєкти
* **Фронтенд (React):** [StoronnimV.Client](https://github.com/ilyaghrischenko/StoronnimV.Client/tree/main)

## 🛠 Технологічний стек

* **Платформа:** ASP.NET Core 9.0
* **База даних:** Entity Framework Core (PostgreSQL)
* **Фонові завдання:** Hangfire (автоматичне оновлення статусів розкладу)
* **Хмарне сховище:** Azure Blob Storage для фото та відео
* **Безпека:** JWT Bearer авторизація та Rate Limiting
* **Моніторинг:** Serilog (логування) та Health Checks

## 🏗 Архітектура

Проєкт реалізовано згідно з принципами **Clean Architecture**:
- **Domain:** Сутності бізнес-логіки та інтерфейси репозиторіїв.
- **Application:** Сервіси, DTO, валідація (FluentValidation) та мапінг (AutoMapper).
- **Infrastructure:** Реалізація доступу до даних, міграції та зовнішні інтеграції.
- **API:** REST-контролери та Middleware для обробки помилок.

## 🚀 Швидкий старт

### Вимоги
- .NET 9 SDK (точний patch не зафіксовано)
- PostgreSQL (точну версію server не зафіксовано)
- Окремий non-production Azure Blob Storage resource для media operations

### Налаштування
Використовуйте канонічний [runtime contract](../docs/implementation/10_RUNTIME_CONTRACT.md) і скопіюйте `StoronnimV.Server/StoronnimV.Api/.env.example` в untracked `.env`. Шаблон містить тільки фактичні імена та безпечні local placeholders. `VITE_API_URL` поки не діє.

### Запуск

Build перевірено в `BASE-02`. Для schema changes використовуйте окремий [migration workflow](../docs/implementation/11_MIGRATION_WORKFLOW.md); API startup залишається окремою перевіркою `BASE-03`.

## 📈 Основні можливості

* **Автоматизація:** Щоденне фонове оновлення статусів виступів через Hangfire.
* **Керування медіа:** Інтегрована система завантаження та видалення файлів з хмари.
* **Стабільність:** Глобальна обробка винятків та захист від спам-запитів.
