using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Byndyusoft.Logging.Builders.Interfaces;
using Microsoft.Extensions.Logging;
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

    public class StructuredActivityEventBuilder : DefaultActivityEventBuilder
    {
        public new static IActivityEventBuilder Instance { get; } = new StructuredActivityEventBuilder();

        public override ActivityEvent Build(
            IFormatProvider formatProvider, 
            LogEvent logEvent)
        {
            if (logEvent.Properties.TryGetValue(
                    LogEventPropertyNames.StructureActivityEventName,
                    out var eventNameValue) == false)
                return base.Build(formatProvider, logEvent);

            var eventName = GetValue(eventNameValue)?.ToString() ?? "Unrecognized Event";
            var fields = new Dictionary<string, object>();

            AddExceptionFields(logEvent, fields);
            AddMessageTemplatePropertyFields(logEvent, fields,
                name => name != LogEventPropertyNames.StructureActivityEventName);
            fields.Remove(LogEventPropertyNames.StructureActivityEventName);

            return new ActivityEvent(eventName, tags: new ActivityTagsCollection(fields));
        }
    }

    public static class LogEventPropertyNames
    {
        public static string StructureActivityEventName => "_activity_event_name";
    }

    public static class LoggerExtensions
    {
        public static void LogStructuredActivityEvent(
            this ILogger logger, 
            string eventName,
            StructuredActivityEventItem[] eventItems)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (eventItems == null)
                throw new ArgumentNullException(nameof(eventItems));

            var messageBuilder =
                new StringBuilder($"{LogEventPropertyNames.StructureActivityEventName} = {{{eventName}}}; ");
            var parameters = new List<object>();
            foreach (var eventItem in eventItems)
            {
                var itemName = eventItem.Name.Replace('.', '_');
                messageBuilder.Append($"{eventItem.Description} = {{{itemName}}}; ");
                parameters.Add(eventItem.Value);
            }

            logger.LogInformation(messageBuilder.ToString(), parameters.ToArray());
        }
    }

    public class StructuredActivityEventItem
    {
        public string Name { get; }

        public object Value { get; }

        public string Description { get; }

        public StructuredActivityEventItem(string name, object value, string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }
    }
}