# StoronnimV.Server

[![Built with .NET 8](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

The backend infrastructure for the "Storonnim V" multimedia portal. This system manages news feeds, concert schedules, band member profiles, and high-performance media storage.

## 🔗 Related Projects
* **Frontend (React):** [StoronnimV.Client](https://github.com/ilyaghrischenko/StoronnimV.Client/tree/main)

## 🛠 Tech Stack

* **Framework:** ASP.NET Core 8.0 / 9.0
* **Database:** Entity Framework Core with PostgreSQL/SQL Server support
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
- .NET 8 SDK
- Docker & Docker Compose
- Azure Storage Account (or Azurite for local dev)

### Configuration
Create a `.env` file in the root directory:
```env
DB_CONNECTION_STRING=Your_Connection_String
AZURE_STORAGE_CONNECTION=Your_Azure_String
JWT_KEY=Your_Secret_Key

```

### Running the App

```bash
# Clone the repository
git clone [https://github.com/ilyaghrischenko/storonnimv.server.git](https://github.com/ilyaghrischenko/storonnimv.server.git)

# Run migrations
dotnet ef database update --project StoronnimV.Infrastructure --startup-project StoronnimV.Api

# Launch
dotnet run --project StoronnimV.Api

```

## 📈 Key Features

* **Automated Workflows:** Daily background jobs to sync concert statuses.
* **Media Management:** Integrated image resizing and cloud storage.
* **Robust Security:** Custom exception handling middleware and request throttling.

---

## 3. Ukrainian Version: README.md

[![Побудовано на .NET 8](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)
[![Архітектура](https://img.shields.io/badge/Architecture-Clean-green)](#архітектура)

Бекенд-інфраструктура для мультимедійного порталу гурту "Стороннім В". Система забезпечує керування новинами, розкладом концертів, профілями учасників та медіа-контентом.

## 🔗 Пов'язані проєкти
* **Фронтенд (React):** [StoronnimV.Client](https://github.com/ilyaghrischenko/StoronnimV.Client/tree/main)

## 🛠 Технологічний стек

* **Платформа:** ASP.NET Core 8.0 / 9.0
* **База даних:** Entity Framework Core (PostgreSQL / SQL Server)
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
- .NET 8 SDK
- Docker & Docker Compose
- Акаунт Azure Storage (або Azurite для локальної розробки)

### Налаштування
Створіть файл `.env` у кореневій папці:
```env
DB_CONNECTION_STRING=Ваш_рядок_підключення
AZURE_STORAGE_CONNECTION=Ваш_рядок_Azure
JWT_KEY=Ваш_секретний_ключ
```

### Запуск

```bash
# Оновлення бази даних
dotnet ef database update --project StoronnimV.Infrastructure --startup-project StoronnimV.Api

# Запуск проєкту
dotnet run --project StoronnimV.Api
```

## 📈 Основні можливості

* **Автоматизація:** Щоденне фонове оновлення статусів виступів через Hangfire.
* **Керування медіа:** Інтегрована система завантаження та видалення файлів з хмари.
* **Стабільність:** Глобальна обробка винятків та захист від спам-запитів.
