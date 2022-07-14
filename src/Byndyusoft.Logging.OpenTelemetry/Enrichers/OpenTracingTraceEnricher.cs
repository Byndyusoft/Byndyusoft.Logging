using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Byndyusoft.Logging.Enrichers
{
    public class OpenTelemetryTraceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var currentActivity = Activity.Current;

            if (currentActivity == null)
            {
                return;
            }

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", currentActivity.TraceId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SpanId", currentActivity.SpanId));
            logEvent.RemovePropertyIfPresent("ParentId");
        }
    }
}