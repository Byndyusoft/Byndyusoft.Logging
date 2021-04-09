using System;
using Byndyusoft.Logging.Enrichers;
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
            string serviceName)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return loggerConfiguration
                .UseDefaultEnrichSettings(serviceName)
                .UseConsoleWriterSettings()
                .OverrideDefaultLoggers()
                .ReadFrom.Configuration(configuration);
        }

        public static LoggerConfiguration UseDefaultEnrichSettings(
            this LoggerConfiguration loggerConfiguration, string serviceName)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            if(serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));

            return loggerConfiguration
                .Enrich.WithBuildConfiguration()
                .Enrich.WithServiceName(serviceName)
                .Enrich.WithEnvironment()
                .Enrich.FromLogContext();
        }

        public static LoggerConfiguration UseConsoleWriterSettings(
            this LoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .WriteTo.Console(new JsonLoggingFormatter());
        }

        public static LoggerConfiguration OverrideDefaultLoggers(
            this LoggerConfiguration loggerConfiguration,
            LogEventLevel microsoft = LogEventLevel.Fatal,
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
                .WriteTo.Async( x=> x.File(new JsonLoggingFormatter(), "./logs/error.log", LogEventLevel.Error));
        }
    }
}
