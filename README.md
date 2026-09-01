# «Тёплый дом» — микросервисная экосистема умного дома (.NET 10)

Решение проектной работы 1 спринта курса «Архитектура программного
обеспечения» (Яндекс Практикум Казахстан). Кейс компании «Тёплый дом»:
переход от монолита на Go к микросервисной SaaS-экосистеме.

Система написана **полностью заново** на .NET 10 / C#. Исходный монолит на Go
использован только как материал для анализа As-Is: его кода в решении нет.

**Ответы на задания 1–5 — в [`Project_template.md`](Project_template.md).**

## Стек

- .NET 10 / ASP.NET Core (Minimal API), C#
- PostgreSQL 16 + EF Core 10 (Npgsql), база на сервис
- RabbitMQ + MassTransit 8 (Apache-2.0) — асинхронный обмен событиями
- YARP — API Gateway
- OpenAPI 3.1 (`Microsoft.AspNetCore.OpenApi`) + Scalar, AsyncAPI 3.0
- Docker / Docker Compose

## Чистая архитектура внутри каждого микросервиса

Каждый сервис — один проект, слои внутри него разложены по папкам:

```
src/Services/WarmHouse.<Сервис>/
├── Domain/          сущности, агрегаты, value-объекты, доменные события,
│                   правила, порты хранения
├── Application/     сценарии использования (по файлу на сценарий), DTO,
│                   исходящие порты
├── Infrastructure/  DbContext, конфигурации EF, репозитории,
│                   потребители RabbitMQ, адаптеры
├── Api/             модули эндпойнтов
└── Program.cs       composition root
```

Направление зависимостей — `Api -> Infrastructure -> Application -> Domain`.
Здесь это соглашение, а не ограничение компилятора: для системы такого
размера отдельный проект на слой оказался лишней церемонией.

Что это даёт:

- **Домен ничего не знает о фреймворках.** В `Domain` нет ни EF Core, ни
  MassTransit, ни HTTP. Маппинг вынесен в `IEntityTypeConfiguration`, поэтому
  сущности свободны от атрибутов персистентности.
- **Сценарии использования не знают о транспорте.** Они возвращают `Result`
  с типизированной ошибкой; в HTTP-код её переводит единственное место —
  `ResultExtensions` в слое представления.
- **Инфраструктура подключается через порты.** `IDeviceRepository`,
  `IDeviceQueries`, `IDeviceGateway`, `IIntegrationEventPublisher`,
  `IUnitOfWork` объявлены внутри, реализованы снаружи.
- **Эндпойнты разложены по модулям.** Каждый `IEndpointModule` описывает одну
  группу ресурсов и подключается автоматически.

## Быстрый старт

```bash
docker compose up -d --build      # сборка и запуск всех контейнеров
docker compose ps                 # состояние
bash deploy/smoke-test.sh         # сквозная проверка экосистемы
docker compose down -v            # остановка и очистка данных
```

Требуется Docker и ~4 ГБ свободной памяти. Первая сборка занимает несколько минут.

Локальная сборка без Docker:

```bash
dotnet build WarmHouse.slnx
```

## Запуск из Rider

В решении есть папки `Solution Items`, `deploy` и `docs` — `docker-compose.yml`,
`Dockerfile`, скрипты и диаграммы видны прямо в дереве решения.

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
| `temperature-api` | **8081** | Имитатор партнёрского датчика (задание 5) | http://localhost:8081/scalar |
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
warmhouse-dotnet/
├── Project_template.md              ответы на задания 1–5
├── WarmHouse.slnx                   решение (.NET 10)
├── Directory.Build.props            общие настройки сборки
├── Directory.Packages.props         централизованные версии пакетов
├── docker-compose.yml
├── deploy/
│   ├── Dockerfile                   один многостадийный образ на все сервисы
│   ├── smoke-test.sh                сквозная проверка экосистемы
│   └── postgres/init/               создание баз данных сервисов
├── docs/
│   ├── c4/                          контекст, контейнеры, компоненты, код
│   ├── er/                          ER-диаграмма
│   └── api/                         OpenAPI по сервисам + AsyncAPI
└── src/
    ├── Shared/
    │   ├── WarmHouse.Shared              Kernel/ Application/ Infrastructure/
    │   └── WarmHouse.Shared.Contracts    интеграционные события (общий контракт)
    ├── Gateway/WarmHouse.Gateway         YARP
    ├── Simulators/WarmHouse.TemperatureApi   имитатор датчика (задание 5)
    └── Services/                         9 микросервисов, по проекту на сервис
        ├── WarmHouse.Identity/  WarmHouse.Devices/  WarmHouse.Heating/
        ├── WarmHouse.Lighting/  WarmHouse.Gates/    WarmHouse.Monitoring/
        └── WarmHouse.Telemetry/ WarmHouse.Scenarios/ WarmHouse.Notifications/
```

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
