using System;
using Byndyusoft.Logging.Formatters;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Byndyusoft.Logging.Configuration
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration UseDefaultSettings(
            this LoggerConfiguration loggerConfiguration,
            IConfiguration configuration,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return loggerConfiguration
                .Enrich.FromLogContext()
                .UseConsoleWriterSettings(restrictedToMinimumLevel)
                .OverrideDefaultLoggers()
                .ReadFrom.Configuration(configuration);
        }

        public static LoggerConfiguration UseConsoleWriterSettings(
            this LoggerConfiguration loggerConfiguration,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .WriteTo.Console(new JsonLoggingFormatter(), restrictedToMinimumLevel);
        }

        public static LoggerConfiguration OverrideDefaultLoggers(
            this LoggerConfiguration loggerConfiguration,
            LogEventLevel microsoft = LogEventLevel.Error,
            LogEventLevel system = LogEventLevel.Warning,
            LogEventLevel microsoftHostingLifetime = LogEventLevel.Information)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .MinimumLevel.Override("Microsoft", microsoft)
                .MinimumLevel.Override("System", system)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", microsoftHostingLifetime);
        }

        public static LoggerConfiguration UseFileWriterSettings(
            this LoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .WriteTo.Async(x => x.File(new JsonLoggingFormatter(), "./logs/verbose.log"))
                .WriteTo.Async(x => x.File(new JsonLoggingFormatter(), "./logs/error.log", LogEventLevel.Error));
        }
    }
}