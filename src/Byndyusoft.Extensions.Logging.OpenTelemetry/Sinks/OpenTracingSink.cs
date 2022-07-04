using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTelemetry;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Byndyusoft.Logging.Sinks
{
    /// <summary>
    /// Записывает логи в лог спана
    /// </summary>
    public class OpenTelemetrySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public OpenTelemetrySink(IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var activity = Activity.Current;

            if (activity is null)
            {
                return;
            }

            try
            {
                var renderMessage = logEvent.RenderMessage(_formatProvider);

                var fields = new Dictionary<string, object>
                {
                    ["event"] = logEvent.MessageTemplate.Text,
                    ["level"] = logEvent.Level.ToString(),
                    ["component"] = logEvent.Properties["SourceContext"],
                    ["message"] = renderMessage
                };

                if (logEvent.Exception != null)
                {
                    fields["error.kind"] = logEvent.Exception.GetType().FullName;
                    fields["error.object"] = logEvent.Exception;
                }

                var propertyNames = logEvent.MessageTemplate.Tokens.OfType<PropertyToken>().Select(x => x.PropertyName)
                    .ToArray();
                foreach (var property in logEvent.Properties)
                {
                    if (propertyNames.Contains(property.Key))
                        fields[property.Key] = property.Value;
                }

                var activityLogEvent = new ActivityEvent(renderMessage, tags: new ActivityTagsCollection(fields));
                activity.AddEvent(activityLogEvent);
            }
            catch (Exception logException)
            {
                var fields = new Dictionary<string, object>
                {
                    ["event"] = "Logging error",
                    ["logging.error"] = logException.ToString()
                };

                var activityExceptionEvent =
                    new ActivityEvent("Logging error", tags: new ActivityTagsCollection(fields));
                activity.AddEvent(activityExceptionEvent);
            }
        }
    }
}