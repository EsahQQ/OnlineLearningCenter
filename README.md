# Web-приложение баз данных "Центр онлайн обучения"

[![Build and Publish](https://github.com/EsahQQ/OnlineLearningCenter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/EsahQQ/OnlineLearningCenter/actions/workflows/dotnet.yml)

Приложение представляет собой систему управления для центра онлайн-обучения и реализовано на платформе .NET 8.

## Технологический стек

*   **Backend:** C#, .NET 8, ASP.NET Core MVC
*   **База данных:** MS SQL Server
*   **Доступ к данным:** Entity Framework Core 8
*   **Аутентификация:** ASP.NET Core Identity
*   **Frontend:** HTML, CSS, JavaScript, Bootstrap 5
*   **Тестирование:** xUnit, Moq, FluentAssertions
*   **CI/CD:** GitHub Actions

## Структура проекта

Проект построен на основе трехуровневой архитектуры:
- `OnlineLearningCenter.DataAccess`: Слой доступа к данным (EF Core, репозитории).
- `OnlineLearningCenter.BusinessLogic`: Слой бизнес-логики (сервисы, DTO, AutoMapper).
- `OnlineLearningCenter.Web`: Слой представления (ASP.NET Core MVC, Identity).

## Запуск и настройка

1.  Клонируйте репозиторий.
2.  Откройте решение `OnlineLearningCenter.sln` в Visual Studio 2022.
3.  Настройте строки подключения к базам данных. В проекте `OnlineLearningCenter.Web` кликните правой кнопкой мыши -> "Управление секретами пользователя" и создайте файл `secrets.json` следующей структуры, заполнив его своими данными:

    ```json
    {
      "Database:AppUser": "ЛОГИН_К_ОСНОВНОЙ_БД",
      "Database:AppPassword": "ПАРОЛЬ_К_ОСНОВНОЙ_БД",
      "Database:IdentityUser": "ЛОГИН_К_БД_IDENTITY",
      "Database:IdentityPassword": "ПАРОЛЬ_К_БД_IDENTITY"
    }
    ```
4.  Создайте базы данных и примените миграции с помощью команд `Update-Database` в "Консоли диспетчера пакетов" для каждого DbContext.
5.  Запустите проект (`F5`).

---
*Курсовой проект по дисциплине "Разработка приложений баз данных для информационных систем"*
