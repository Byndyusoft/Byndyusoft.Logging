using System;
using System.Collections.Generic;
using System.Diagnostics;
using Byndyusoft.Logging.Builders.Interfaces;
using Serilog.Events;

namespace Byndyusoft.Logging.Builders
{
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
            AddMessageTemplatePropertyFields(
                logEvent, 
                fields,
                name => name != LogEventPropertyNames.StructureActivityEventName);
            fields.Remove(LogEventPropertyNames.StructureActivityEventName);

            return new ActivityEvent(eventName, tags: new ActivityTagsCollection(fields));
        }
    }
}