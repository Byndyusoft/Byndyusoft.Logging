using OpenTracing.Util;
using Serilog.Core;
using Serilog.Events;

namespace Byndyusoft.Logging.Enrichers
{
    public class OpenTracingTraceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var tracer = GlobalTracer.Instance;

            var activeSpan = tracer?.ActiveSpan;
            if (activeSpan == null)
            {
                return;
            }

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", activeSpan.Context.TraceId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SpanId", activeSpan.Context.SpanId));
            logEvent.RemovePropertyIfPresent("ParentId");
        }
    }
}