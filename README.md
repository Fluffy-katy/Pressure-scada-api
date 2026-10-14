# Лабораторная работа №2 — Pressure SCADA API

Web API для модуля SCSDA-систем измерения давления в нефтегазовой отрасли.

## Запуск в VS Code

```bash
cd PressureScada.Api
dotnet run
```

После запуска Swagger доступен по адресу `https://localhost:порт/swagger`.

## Предметная модель

- `MeasurementStation` — производственная площадка или скважина;
- `PressureSensor` — датчик давления с диапазоном измерений;
- `PressureMeasurement` — измерение давления во времени.

Классы содержат бизнес-методы: обновление данных, проверку диапазона датчика, проверку критического значения и изменение статуса измерения.

## API

CRUD реализован для `/api/stations`, `/api/sensors` и `/api/measurements`.

Сводные LINQ-запросы:

- `GET /api/reports/latest-by-station` — последнее измерение по станции;
- `GET /api/reports/sensor-statistics` — статистика датчиков;
- `GET /api/reports/alerts?threshold=180` — превышения порога давления.

Для отчетов нужен JWT-токен. Для POST/PUT/DELETE нужен пользователь с ролью `Admin`.

Тестовые пользователи: `operator / operator123` и `admin / admin123`.
