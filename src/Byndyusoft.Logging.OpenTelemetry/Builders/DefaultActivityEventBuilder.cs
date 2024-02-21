using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Byndyusoft.Logging.Builders.Interfaces;
using Serilog.Events;
using Serilog.Parsing;

namespace Byndyusoft.Logging.Builders
{
    public class DefaultActivityEventBuilder : IActivityEventBuilder
    {
        public static IActivityEventBuilder Instance { get; } = new DefaultActivityEventBuilder();

        public virtual ActivityEvent Build(
            IFormatProvider formatProvider,
            LogEvent logEvent)
        {
            var renderMessage = logEvent.RenderMessage(formatProvider);

            var fields = new Dictionary<string, object>
            {
                ["level"] = logEvent.Level.ToString(),
                ["component"] = logEvent.Properties["SourceContext"],
                ["message"] = renderMessage
            };

            AddExceptionFields(logEvent, fields);
            AddMessageTemplatePropertyFields(logEvent, fields);

            return new ActivityEvent(logEvent.MessageTemplate.Text, tags: new ActivityTagsCollection(fields));
        }

        protected static void AddExceptionFields(LogEvent logEvent, Dictionary<string, object> fields)
        {
            if (logEvent.Exception != null)
            {
                fields["error.kind"] = logEvent.Exception.GetType().FullName;
                fields["error.object"] = logEvent.Exception;
            }
        }

        protected void AddMessageTemplatePropertyFields(
            LogEvent logEvent, 
            Dictionary<string, object> fields,
            Func<string, bool> propertyNameFilter = null)
        {
            var propertyNames = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Select(x => x.PropertyName)
                .ToArray();
            foreach (var property in logEvent.Properties)
            {
                if (propertyNameFilter?.Invoke(property.Key) == false)
                    continue;

                if (propertyNames.Contains(property.Key))
                {
                    var key = property.Key.Replace('_', '.');
                    fields[key] = GetValue(property.Value);
                }
            }
        }

        public virtual ActivityEvent BuildForLogException(Exception logException)
        {
            var fields = new Dictionary<string, object>
            {
                ["event"] = "Logging error",
                ["logging.error"] = logException.ToString()
            };

            return new ActivityEvent("Logging error", tags: new ActivityTagsCollection(fields));
        }

        protected object GetValue(LogEventPropertyValue logEventPropertyValue)
        {
            if (logEventPropertyValue is ScalarValue scalarValue)
                return scalarValue.Value;

            return logEventPropertyValue;
        }
    }
}