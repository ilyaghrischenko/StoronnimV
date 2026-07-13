# StoronnimV.Client

## English

[![React](https://img.shields.io/badge/React-18.3-61dafb)](https://reactjs.org/)
[![Vite](https://img.shields.io/badge/Vite-6.0-646cff)](https://vitejs.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.6-3178c6)](https://www.typescriptlang.org/)

The frontend application for the **Storonnim V** multimedia portal. Built with React and TypeScript, this client provides a dynamic interface for fans and a comprehensive dashboard for administrators.

## 🔗 Repository Links
* **Server (Backend API):** [https://github.com/ilyaghrischenko/StoronnimV.Server](https://github.com/ilyaghrischenko/StoronnimV.Server)

## 🛠 Tech Stack

- **Framework:** React 18 (Functional Components, Hooks)
- **Build Tool:** Vite
- **Language:** TypeScript
- **State Management:** React Context API (Modular Approach)
- **Styling:** SCSS (Sassy CSS)
- **Routing:** React Router DOM
- **Deployment:** Optimized for Azure Static Web Apps

## 🏗 Project Structure

- `src/components/pages`: Main view components (Home, News, Schedule, etc.).
- `src/components/elements`: Reusable UI modules organized by feature (Admin, Music, Group).
- `src/components/contexts`: Global state providers for authentication and data management.
- `src/models`: TypeScript interfaces reflecting Backend DTOs.
- `src/styles`: Modular SCSS files with shared variables and themes.

## 🚀 Getting Started

### Prerequisites
- Node.js supported by the locked Vite version: `^18.0.0 || ^20.0.0 || >=22.0.0`
- npm (exact version is not pinned; `package-lock.json` is authoritative)

### Installation
1. Clone the repository:
git clone [https://github.com/ilyaghrischenko/StoronnimV.Client.git](https://github.com/ilyaghrischenko/StoronnimV.Client.git)

2. Install dependencies:
npm install

3. Copy the local frontend environment example and confirm that its API URL matches the local backend endpoint:

```bash
cp storonnimv.client/.env.example storonnimv.client/.env.local
```

`VITE_API_URL` is required and must be an absolute HTTP(S) URL without credentials, query, or fragment. Read the canonical [runtime contract](../docs/implementation/10_RUNTIME_CONTRACT.md) before starting the services.

The install/dev/build commands below are repository scripts, not successful build or startup evidence from `BASE-01`.

### Development

```bash
npm run dev

```

### Build for Production

```bash
npm run build

```

## 📈 Key Features

* **Dynamic Content:** Real-time news feeds and automated concert schedules.
* **Admin Dashboard:** Secure management of band members, media, and events.
* **Responsive Design:** Fully optimized for desktop and mobile devices.
* **Interactive Media:** Integrated Spotify/YouTube embeds and photo galleries.

---

## Ukrainian

[![React](https://img.shields.io/badge/React-18.3-61dafb)](https://reactjs.org/)
[![Архітектура](https://img.shields.io/badge/Architecture-Modular-green)](#архітектура)

Фронтенд-частина мультимедійного порталу гурту **"Стороннім В"**. Додаток розроблений на React та TypeScript, надаючи зручний інтерфейс для користувачів та повноцінну панель керування для адміністраторів.

## 🔗 Посилання на репозиторії
* **Сервер (API):** [https://github.com/ilyaghrischenko/StoronnimV.Server](https://github.com/ilyaghrischenko/StoronnimV.Server)

## 🛠 Технологічний стек

- **Фреймворк:** React 18 (Функціональні компоненти та Хуки)
- **Збірка:** Vite (висока швидкість розробки)
- **Мова:** TypeScript (типобезпека)
- **Стейт-менеджмент:** React Context API
- **Стилізація:** SCSS (модульний підхід)
- **Роутинг:** React Router DOM
- **Деплой:** Налаштовано для Azure Static Web Apps

## 🏗 Структура проєкту

- `src/components/pages`: Компоненти сторінок (Головна, Новини, Розклад тощо).
- `src/components/elements`: Багаторазові UI-модулі, згруповані за функціоналом (Адмін-панель, Музика, Гурт).
- `src/components/contexts`: Провайдери глобального стану для авторизації та даних.
- `src/models`: TypeScript інтерфейси, що відповідають структурам даних бекенду.
- `src/styles`: Модульні SCSS стилі зі спільними змінними та темами.

## 🚀 Запуск проєкту

### Попередні вимоги
- Node.js у діапазоні, який підтримує зафіксований Vite: `^18.0.0 || ^20.0.0 || >=22.0.0`
- npm (точна версія не зафіксована; authoritative file — `package-lock.json`)

### Встановлення
1. Клонуйте репозиторій:
git clone [https://github.com/ilyaghrischenko/StoronnimV.Client.git](https://github.com/ilyaghrischenko/StoronnimV.Client.git)

2. Встановіть залежності:
npm install

3. Скопіюйте локальний приклад frontend environment і перевірте, що його API URL відповідає локальному backend endpoint:

```bash
cp storonnimv.client/.env.example storonnimv.client/.env.local
```

`VITE_API_URL` є обов'язковою абсолютною HTTP(S) URL без credentials, query або fragment. Перед запуском services прочитайте канонічний [runtime contract](../docs/implementation/10_RUNTIME_CONTRACT.md).

Наведені нижче install/dev/build commands є scripts репозиторію, а не доказом успішного build або startup у `BASE-01`.

### Розробка

```bash
npm run dev
```

## 📈 Основні можливості

* **Динамічний контент:** Стрічка новин та автоматизований розклад концертів.
* **Адмін-панель:** Захищений доступ для редагування учасників, медіа та новин.
* **Адаптивність:** Інтерфейс оптимізовано для десктопних та мобільних пристроїв.
* **Мультимедіа:** Інтеграція зі Spotify, YouTube та хмарним сховищем для фото.
