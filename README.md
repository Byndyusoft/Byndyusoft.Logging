# Byndyusoft.Logging [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Logging.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Logging.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging/)

Структурные логи в json формате.

Библиотека является набором пресетов для Serilog, которые позволяют добавлять поддержку логирования в несколько строк.

```
using Byndyusoft.Logging.Configuration;
using Serilog;

...

Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) => configuration
        .UseDefaultSettings(context.Configuration, "Sample project")
    )

```

В результате такого подключения будут добавлено логирование в стандартный вывод логов в формате json. Каждый вывод в лог производится с новой строки и занимает 1 строку.

Например, для тестового вывода будет, будет получен следущей лог:

```
var values = new[] {"value1", "value2"};
logger.LogInformation("запрошены {@Values}", (object)values);

Вывод (отформатированный):
{
	"Timestamp": "2021-04-09T13:12:58.2633978Z",
	"Message": "запрошены [\"value1\", \"value2\"]",
	"MessageTemplateHash": "05114bc8",
	"Level": "Information",
	"Properties": {
		"Values": [
			"value1",
			"value2"
		]
	}
}
```

Где:

- `Timestamp` — время в utc в которое создана запись лога
- `Message` — залогированное сообщение с подставленными параметрами
- `MessageTemplateHash` — хэш шаблона сообщения, который не зависит от параметров
- `Level` — уровень лога
- `Properties.*` — все остальные свойства сообщения
- `Values` — параметры сообщения

Кроме того `Properties.*` обогащается полями добавленными в контекст лога. Т.е. если вызывать `logger.LogInformation("запрошены {@Values}", (object)values)` из метода контроллера, то в `Properties` добавятся следующие поля, которые добавляет инфраструктура asp.net

```
"SourceContext": "Byndyusoft.Logging.Sample.Controllers.ValuesController",
"ActionId": "21eb782a-3717-4616-9927-7d2a5a23c8b8",
"ActionName": "Byndyusoft.Logging.Sample.Controllers.ValuesController.Get (Byndyusoft.Logging.Sample)",
"RequestId": "0HM7RB4OJU2MI:00000001",
"RequestPath": "/api/values",
"SpanId": "|b01373be-4fffa4bff3ac61ea.",
"TraceId": "b01373be-4fffa4bff3ac61ea",
"ParentId": "",
"ConnectionId": "0HM7RB4OJU2MI",
```

В случе логирования исключения, добавляет информация об исключении (ToString()) и хэш стектрейса для поиска аналогичных исключений. Хэш считается без учёта номеров строк.

```
logger.LogError(ex, "Должен совпасть хэш ошибки")

{
    "Timestamp":"2021-04-09T10:10:16.6873034Z",
    "Message":"Должен совпасть хэш ошибки",
    "MessageTemplateHash":"3e4763a9",
    "Level":"Error",
    "Exception":"System.Exception: Что-то пошло не так\r\n ---> System.NotImplementedException: Скоро сделаем\r\n   at Byndyusoft.Logging.Sample.Controllers.ValuesController.ThrowError() in C:\\work\\reps\\Byndyusoft.Logging\\src\\Byndyusoft.Logging.Sample\\Controllers\\ValuesController.cs:line 58\r\n   at Byndyusoft.Logging.Sample.Controllers.ValuesController.ThrowErrorWithInnerError() in C:\\work\\reps\\Byndyusoft.Logging\\src\\Byndyusoft.Logging.Sample\\Controllers\\ValuesController.cs:line 65\r\n   --- End of inner exception stack trace ---\r\n   at Byndyusoft.Logging.Sample.Controllers.ValuesController.ThrowErrorWithInnerError() in C:\\work\\reps\\Byndyusoft.Logging\\src\\Byndyusoft.Logging.Sample\\Controllers\\ValuesController.cs:line 69\r\n   at Byndyusoft.Logging.Sample.Controllers.ValuesController.GetError() in C:\\work\\reps\\Byndyusoft.Logging\\src\\Byndyusoft.Logging.Sample\\Controllers\\ValuesController.cs:line 44",
    "ExceptionHash":"533f548e",
    ...
}

```

# Поддержка трассировки

## OpenTracing [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Logging.OpenTracing.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTracing/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Logging.OpenTracing.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTracing/)

Сначала нужно подключить `Byndyusoft.Logging.OpenTracing`

Можно заменить значения `TraceId` и `SpanId` на полученные от OpenTracing. `ParentId` совсем удаляет.

```
.UseSerilog((context, configuration) => configuration
    .UseOpenTracingTraces()
```


Можно сделать так, чтобы всё что пишется в логи, оказалось в трасах.

```
.UseSerilog((context, configuration) => configuration
    .WriteToOpenTracing()
```

## OpenTelemetry [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Logging.OpenTelemetry.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTelemetry/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Logging.OpenTelemetry.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTelemetry/)

Сначала нужно подключить `Byndyusoft.Logging.OpenTelemetry`

Можно заменить значения `TraceId` и `SpanId` на полученные от OpenTelemetry. `ParentId` совсем удаляет.

```
.UseSerilog((context, configuration) => configuration
    .UseOpenTelemetryTraces()
```


Можно сделать так, чтобы всё что пишется в логи, оказалось в трасах.

```
.UseSerilog((context, configuration) => configuration
    .WriteToOpenTelemetry()
```


# Предусмотренные сценарии

## Как добавить вывод в файл?

```
.UseSerilog((context, configuration) => configuration
    .UseFileWriterSettings()
```

В рабочей паке будут добавлены 2 файла `logs/verbose.log` и `error.log`

## Как изменить уровень логирования на проде?

Это просто Serilog. Поэтому в ваших настройках должен быть параметр `Serilog:MinimumLevel:Default` с желаемым уровнем логирования. В переменных окружения `:` нужно заменить на `__`: `Serilog__MinimumLevel__Default`.

## Мне не подходят стандартные настройки

Помните, что это всё ещё обычный Serilog. Вы можете изменить настройки так, как вам нравится. Скажем `UseDefaultSettings` представляет из себя следующей вызов:

```
return loggerConfiguration
    .Enrich.FromLogContext()
    .UseConsoleWriterSettings(restrictedToMinimumLevel)
    .OverrideDefaultLoggers()
    .ReadFrom.Configuration(configuration)
```

Можно использовать только те его части, которые подходят вашему проекту.

## Хочу положить в события трассировок только определенные свойства (структурные события)

Сначала нужно подключить `Byndyusoft.Logging.OpenTelemetry.Abstractions` [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.Logging.OpenTelemetry.Abstractions.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTelemetry.Abstractions/) [![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.Logging.OpenTelemetry.Abstractions.svg?style=flat)](https://www.nuget.org/packages/Byndyusoft.Logging.OpenTelemetry.Abstractions/)

После этого нужно подключить построитель структурных событий логов для минимизации событий в трассировке.

```
.UseSerilog((context, configuration) => configuration
    .WriteToOpenTelemetry(activityEventBuilder: StructuredActivityEventBuilder.Instance)
```

Пример логирования структурных событий:

```
var eventItems = new[]
{
    new StructuredActivityEventItem("Id", 10),
    new StructuredActivityEventItem("Company.Name", "Byndyusoft")
};
_logger.LogStructuredActivityEvent("MethodInput", eventItems);
```

В событии активности добавятся только два свойства: _Id_ и _Company.Name_. Имя события будет _MethodInput_.

# Обогащение дополнительными полями

Для обогащения информацией о параметрах окружения с наименованием BUILD_*, именем сервиса и версией сервиса можно воспользоваться библиотекой [Byndyusoft.Telemetry.Logging.Serilog](https://www.nuget.org/packages/Byndyusoft.Telemetry.Logging.Serilog). Документация - [тут](https://github.com/Byndyusoft/Byndyusoft.Telemetry/blob/master/README.md).
