# «Тёплый дом» — микросервисная экосистема умного дома (.NET 10)

Переход от монолита на Go к микросервисной SaaS-экосистеме умного дома.

Система написана **полностью заново** на .NET 10 / C#. Исходный монолит на Go
использован только как материал для анализа As-Is: его кода в решении нет.

**Архитектурная документация — в [`ARCHITECTURE.md`](ARCHITECTURE.md).**

## Стек

- .NET 10 / ASP.NET Core (Minimal API), C#
- PostgreSQL 16 + EF Core 10 (Npgsql), база на сервис
- RabbitMQ + MassTransit 8 (Apache-2.0) — асинхронный обмен событиями
- YARP — API Gateway
- OpenAPI 3.1 (`Microsoft.AspNetCore.OpenApi`) + Scalar, AsyncAPI 3.0
- Docker / Docker Compose

## Чистая архитектура внутри каждого микросервиса

Каждый сервис — один проект, слои внутри него разложены по папкам, а внутри
слоя папка соответствует роли типа:

```
src/Services/WarmHouse.<Сервис>/
├── Domain/
│   ├── Entities/        сущности и агрегаты
│   ├── ValueObjects/    value-объекты
│   ├── Enums/           перечисления домена
│   ├── Events/          доменные события
│   ├── Repositories/    порты хранения
│   └── Errors/          ошибки домена с устойчивыми кодами
├── Application/
│   ├── Abstractions/    исходящие порты (запросы, шлюзы, безопасность)
│   ├── Contracts/
│   │   ├── Requests/    входящие DTO
│   │   └── Responses/   исходящие DTO
│   ├── Handlers/
│   │   ├── Commands/    обработчики, меняющие состояние
│   │   ├── Queries/     обработчики чтения
│   │   └── Events/      реакция на интеграционные события
│   ├── Mapping/         преобразование домен ↔ DTO
│   └── DependencyInjection.cs
├── Infrastructure/
│   ├── Persistence/     DbContext, Configurations/, Repositories/,
│   │                    Queries/, Seeders/
│   ├── Messaging/Consumers/   потребители RabbitMQ
│   ├── ...              адаптеры внешних систем (Gateways/, Security/, Delivery/)
│   └── DependencyInjection.cs
├── Presentation/
│   └── Endpoints/       модули эндпойнтов
└── Program.cs           composition root
```

Направление зависимостей — `Presentation -> Infrastructure -> Application -> Domain`.
Здесь это соглашение, а не ограничение компилятора: для системы такого
размера отдельный проект на слой оказался лишней церемонией.

Два правила держат раскладку предсказуемой: **один тип — один файл**
(имя файла совпадает с именем типа) и **пространство имён совпадает с папкой**.
Поэтому по имени типа сразу видно его роль и слой.

Что это даёт:

- **Домен ничего не знает о фреймворках.** В `Domain` нет ни EF Core, ни
  MassTransit, ни HTTP. Маппинг вынесен в `IEntityTypeConfiguration`, поэтому
  сущности свободны от атрибутов персистентности.
- **Обработчики не знают о транспорте.** Они возвращают `Result`
  с типизированной ошибкой; в HTTP-код её переводит единственное место —
  `ResultExtensions` в слое представления.
- **Инфраструктура подключается через порты.** `IDeviceRepository`,
  `IDeviceQueries`, `IDeviceGateway`, `IIntegrationEventPublisher`,
  `IUnitOfWork` объявлены внутри, реализованы снаружи.
- **Эндпойнты разложены по модулям.** Каждый `IEndpointModule` описывает одну
  группу ресурсов и подключается автоматически.

## Доступ к API

Анонимны только два эндпойнта: `POST /api/v1/users` (регистрация) и
`POST /api/v1/auth/token` (выпуск токена). Всё остальное закрыто политикой по
умолчанию и требует заголовка `Authorization: Bearer <access_token>`.

```bash
TOKEN=$(curl -sS -X POST http://localhost:8000/api/v1/auth/token \
  -H 'Content-Type: application/json' \
  -d '{"email":"resident@warmhouse.example","password":"warmhouse-2026"}' \
  | grep -o '"access_token":"[^"]*"' | cut -d'"' -f4)

curl -sS "http://localhost:8000/api/v1/houses" -H "Authorization: Bearer $TOKEN"
```

Токен несёт `sub` (пользователь) и claim `house` на каждый доступный дом.
Идентификатор инициатора берётся из токена, поэтому его нет в телах запросов;
дом, к которому токен не даёт доступа, отвечает `403`. Доступ, выданный после
выпуска токена, попадёт в него только при следующем выпуске.

Ключ подписи задаётся переменной `JWT_SIGNING_KEY` и должен быть одинаковым у
всех сервисов. Значение по умолчанию годится только для разработки.

## Быстрый старт

```bash
docker compose up -d --build      # сборка и запуск всех контейнеров
docker compose ps                 # состояние
bash deploy/smoke-test.sh         # сквозная проверка экосистемы
docker compose down -v            # остановка и очистка данных
```

Требуется Docker и ~4 ГБ свободной памяти. Первая сборка занимает несколько минут.

> Схема создаётся через `EnsureCreated`, который не трогает уже существующую
> базу. Если тома остались от сборки без outbox, поднимайте стек после
> `docker compose down -v` — иначе таблиц `OutboxMessage`, `OutboxState` и
> `InboxState` в базе не окажется.

Локальная сборка без Docker:

```bash
dotnet build WarmHouse.slnx
```

## Запуск из Rider

В решении есть папки `Solution Items`, `deploy` и `docs` — `docker-compose.yml`,
скрипты и диаграммы видны прямо в дереве решения. `Dockerfile` каждого сервиса
лежит рядом с его проектом, поэтому виден в дереве этого проекта.

Готовые конфигурации запуска лежат в `.run/` и подхватываются Rider автоматически:

| Конфигурация | Что делает |
|---|---|
| **docker-compose up** | Поднимает весь стек (13 контейнеров) |
| **docker-compose infrastructure only** | Поднимает только postgres, rabbitmq и имитатор датчика |
| **smoke test** | Прогоняет `deploy/smoke-test.sh` через шлюз |

Чтобы отлаживать сервис прямо из IDE:

1. запустите **docker-compose infrastructure only**;
2. запустите нужный проект по F5 — профиль `http` в `Properties/launchSettings.json`
   уже указывает на `localhost` для PostgreSQL и RabbitMQ и занимает тот же порт,
   что и соответствующий контейнер.

Шлюз при локальном запуске переопределяет адреса кластеров на `localhost`
(см. его `launchSettings.json`), поэтому маршрутизация работает и без Docker.

## Карта портов

| Контейнер | Порт | Назначение | Документация API |
|---|---|---|---|
| `gateway` | **8000** | API Gateway — единая точка входа | http://localhost:8000/scalar |
| `temperature-api` | **8081** | Имитатор партнёрского датчика | http://localhost:8081/scalar |
| `identity` | 8010 | Пользователи и дома | http://localhost:8010/scalar |
| `devices` | 8020 | Управление устройствами | http://localhost:8020/scalar |
| `heating` | 8030 | Управление отоплением | http://localhost:8030/scalar |
| `lighting` | 8040 | Управление освещением | http://localhost:8040/scalar |
| `gates` | 8050 | Управление воротами | http://localhost:8050/scalar |
| `monitoring` | 8060 | Видеонаблюдение | http://localhost:8060/scalar |
| `telemetry` | 8070 | Телеметрия | http://localhost:8070/scalar |
| `scenarios` | 8075 | Сценарии автоматизации | http://localhost:8075/scalar |
| `notifications` | 8085 | Уведомления | http://localhost:8085/scalar |
| `postgres` | 5432 | PostgreSQL | — |
| `rabbitmq` | 5672 / 15672 | Брокер / веб-консоль (`guest`/`guest`) | http://localhost:15672 |

У каждого сервиса есть `GET /health` (живость) и `GET /health/ready`
(готовность зависимостей).

## Структура репозитория

```
warmhouse/
├── ARCHITECTURE.md                  архитектурная документация
├── WarmHouse.slnx                   решение (.NET 10)
├── Directory.Build.props            общие настройки сборки
├── Directory.Packages.props         централизованные версии пакетов
├── docker-compose.yml
├── deploy/
│   ├── smoke-test.sh                сквозная проверка экосистемы
│   └── postgres/init/               создание баз данных сервисов
├── docs/
│   ├── c4/                          контекст, контейнеры, компоненты, код
│   ├── er/                          ER-диаграмма
│   └── api/                         OpenAPI по сервисам + AsyncAPI
└── src/
    ├── Shared/
    │   ├── WarmHouse.Shared              Kernel/ Application/ Infrastructure/ Presentation/
    │   └── WarmHouse.Shared.Contracts    интеграционные события (общий контракт)
    ├── Gateway/WarmHouse.Gateway         YARP
    ├── Simulators/WarmHouse.TemperatureApi   имитатор датчика
    └── Services/                         9 микросервисов, по проекту на сервис
        ├── WarmHouse.Identity/  WarmHouse.Devices/  WarmHouse.Heating/
        ├── WarmHouse.Lighting/  WarmHouse.Gates/    WarmHouse.Monitoring/
        └── WarmHouse.Telemetry/ WarmHouse.Scenarios/ WarmHouse.Notifications/
```

У каждого разворачиваемого проекта — 9 сервисов, шлюз и имитатор датчика —
свой `Dockerfile` рядом с `.csproj`. Контекст сборки всегда корень решения:
образу нужны `Directory.Build.props`, централизованные версии пакетов и общие
проекты `WarmHouse.Shared*`. `docker-compose.yml` только указывает на нужный
файл и не хранит параметров сборки:

```bash
docker build -f src/Services/WarmHouse.Heating/Dockerfile -t warmhouse/heating .
docker compose build heating          # то же самое через compose
```

Образы всегда Linux: оба этапа собираются на тегах `10.0-noble`
(Ubuntu 24.04), а не на плавающих `10.0`, которые на Windows-демоне
разворачиваются в Nano Server. Для IDE то же самое зафиксировано свойством
`DockerDefaultTargetOS=Linux` в каждом `.csproj`.

## Архитектурные решения

**Database per service.** Каждый сервис владеет своей БД. Физически это один
инстанс PostgreSQL (упрощение MVP), логически базы независимы — связи между
ними логические, целостность обеспечивается событиями.

**Асинхронный обмен между сервисами.** Телеметрия и команды — это поток:
издатель не ждёт подписчиков и не знает их списка. Это же изолирует отказы:
недоступность уведомлений не мешает включить отопление.

**Расширяемость под неизвестные устройства.** Прибор описывается записью
`DeviceType` с набором `capabilities`; команда адресуется возможности, а не
модели прибора. Новый тип подключается через `POST /api/v1/device-types` без
изменения кода — доменные сервисы сами фильтруют интересные им устройства при
обработке события `DeviceRegistered`.

**Проекции вместо распределённых запросов.** Доменные сервисы держат локальную
копию нужных им полей устройства, наполняемую из событий, поэтому на горячем
пути нет синхронных вызовов между сервисами.
