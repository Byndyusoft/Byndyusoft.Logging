﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Configuration;

namespace Byndyusoft.Logging.Enrichers
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithApplicationVersion(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string versionString)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            if (string.IsNullOrWhiteSpace(versionString))
                throw new ArgumentNullException(nameof(versionString));

            return enrichmentConfiguration.WithProperty(LoggingPropertyNames.Version, versionString);
        }

        public static LoggerConfiguration WithApplicationInformationalVersion(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.WithApplicationVersion(
                Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion
            );
        }

        public static LoggerConfiguration WithApplicationAssemblyVersion(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.WithApplicationVersion(
                Assembly.GetEntryAssembly().GetName().Version.ToString(4)
            );
        }

        public static LoggerConfiguration WithServiceName(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string serviceName = null)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.WithProperty(
                LoggingPropertyNames.ServiceName,
                string.IsNullOrWhiteSpace(serviceName) == false
                    ? serviceName
                    : Assembly.GetEntryAssembly()?.GetName().Name ?? ""
            );
        }

        public static LoggerConfiguration WithBuildConfiguration(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            const string buildKeyPrefix = "BUILD_";

            var buildProperties = new Dictionary<string, string>();
            var variables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry variable in variables)
            {
                var property = variable.Key.ToString();
                if (property.StartsWith(buildKeyPrefix))
                {
                    var key = property.Remove(0, buildKeyPrefix.Length);
                    buildProperties.Add(EnvironmentKeyToCameCase(key), variable.Value.ToString());
                }
            }

            return enrichmentConfiguration
                .WithProperty(LoggingPropertyNames.Build, buildProperties, true);
        }

        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        private static string EnvironmentKeyToCameCase(string environmentProperty)
        {
            var keyParts = environmentProperty.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => TextInfo.ToTitleCase(TextInfo.ToLower(x)));

            return string.Join("", keyParts);
        }

        public static LoggerConfiguration WithEnvironment(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.WithProperty(
                LoggingPropertyNames.Environment,
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            );
        }
    }
}