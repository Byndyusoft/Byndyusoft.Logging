using System;
using Byndyusoft.Logging.Enrichers;
using Byndyusoft.Logging.Sinks;
using Serilog;

namespace Byndyusoft.Logging.Configuration
{
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Добавлять TraceId, SpanId трассы в логи
        /// </summary>
        public static LoggerConfiguration UseOpenTelemetryTraces(
            this LoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .Enrich.WithOpenTelemetryTraces();
        }

        /// <summary>
        /// Дублировать логи в трассы OpenTracing
        /// </summary>
        public static LoggerConfiguration WriteToOpenTelemetry(this LoggerConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));

            return loggerConfiguration
                .WriteTo.Sink(new OpenTelemetrySink(formatProvider));
        }
    }
}