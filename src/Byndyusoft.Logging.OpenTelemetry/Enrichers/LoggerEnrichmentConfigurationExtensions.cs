using System;
using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Configuration;

namespace Byndyusoft.Logging.Enrichers
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        [ExcludeFromCodeCoverage]
        public static LoggerConfiguration WithOpenTelemetryTraces(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.With<OpenTelemetryTraceEnricher>();
        }
    }
}