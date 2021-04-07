using System;
using Byndyusoft.Logging.Enrichers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;

namespace Byndyusoft.Logging.Configuration
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration UseDefaultSettings(
            this LoggerConfiguration loggerConfiguration,
            IConfiguration configuration)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return loggerConfiguration
                .Enrich.WithExceptionDetails()
                .Enrich.WithServiceName(Environment.GetEnvironmentVariable("SERVICE_NAME"))
                .Enrich.WithApplicationInformationalVersion()
                
                .Enrich.WithMessageTemplateHash()
                .Enrich.WithLogEventHash()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration);
        }
    }
}
