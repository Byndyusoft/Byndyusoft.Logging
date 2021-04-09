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
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("OtTraceId", activeSpan.Context.TraceId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("OtSpanId", activeSpan.Context.SpanId));
        }
    }
}
