using System;
using System.Collections.Generic;
using System.Linq;
using OpenTracing;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Byndyusoft.Logging.Sinks
{
    /// <summary>
    /// Записывает логи в лог спана
    /// </summary>
    public class OpenTracingSink : ILogEventSink
    {
        private readonly ITracer tracer;
        private readonly IFormatProvider formatProvider;

        public OpenTracingSink(ITracer tracer, IFormatProvider formatProvider)
        {
            this.tracer = tracer;
            this.formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            ISpan span = tracer.ActiveSpan;

            if (span == null)
            {
                return;
            }

            try
            {
                var fields = new Dictionary<string, object>
                {
                    [LogFields.Event] = logEvent.MessageTemplate.Text,
                    ["level"] = logEvent.Level.ToString(),
                    ["component"] = logEvent.Properties["SourceContext"],
                    [LogFields.Message] = logEvent.RenderMessage(formatProvider)
                };

                if (logEvent.Exception != null)
                {
                    fields[LogFields.ErrorKind] = logEvent.Exception.GetType().FullName;
                    fields[LogFields.ErrorObject] = logEvent.Exception;
                }

                var propertyNames = logEvent.MessageTemplate.Tokens
                    .OfType<PropertyToken>()
                    .Select(x => x.PropertyName)
                    .ToArray();
                foreach (var property in logEvent.Properties)
                {
                    if (propertyNames.Contains(property.Key))
                        fields[property.Key] = property.Value;
                }

                span.Log(fields);
            }
            catch (Exception logException)
            {
                var fields = new Dictionary<string, object>
                {
                    [LogFields.Event] = "Logging error",
                    ["logging.error"] = logException.ToString()
                };
                span.Log(fields);
            }
        }
    }
}