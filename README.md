# Test task #

## Cloud solution | Extra info auto puller ##

## Generic description ##

ТЗ (Задача для оценки логики, реализации, code style)
Оценка работы с :

- Sql ( любая на выбор , желательно PostgreSql )
- ORM ( любая на выбор , желательно EF Core 6.x )
- Mappers
- gRPC
- REST
- Swagger
- Asp.Net (.net 6)
- Mock testing

## Service 1 ##

- Приложение на Asp.Net (.net6)
- Приложение использует структурное логгирование
- Имеет подключение к БД используя ORM EF Core 6.x (подход Code First)
- Миграции БД должны быть включены в приложение
- Структура БД включает единственную таблицу (желательно не в схеме public)
Ключем в таблице является автогенерируемый long, далее свободный набор полей (минимум еще одно)
- Приложение отдает данные на основе proto контракта, используется протокол http2 (без SSL/c SSL на выбор)
1-2 rpc метода согласно google proto code style (в идеале еще составить lint файл для buf и проверить им proto)

## Service 2 ##

- Приложение на Asp.Net (.net6)
- Приложение использует структурное логгирование
- Приложение забирает данные у приложения 1 на основе grpc конракта
- Приложение отдает данные по Rest ( желательно имплементировать swagger )

### Приветствуется использование Mapper и Interceptor, а так же Mock (.net console app) для тестирования grpc методов ###

---

## Postgres Database ##

- Docker app (docker compose file + .env + local volume)

`docker-compose -f Db/compose-postgresql-db.yaml up -d`
