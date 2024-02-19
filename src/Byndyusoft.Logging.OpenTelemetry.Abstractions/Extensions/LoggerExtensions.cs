using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Logging.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogStructuredActivityEvent(
            this ILogger logger, 
            string eventName,
            StructuredActivityEventItem[] eventItems, 
            LogLevel logLevel = LogLevel.Information)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (eventItems == null)
                throw new ArgumentNullException(nameof(eventItems));

            var messageBuilder =
                new StringBuilder($"Structured Event {{{LogEventPropertyNames.StructureActivityEventName}}} Properties: ");
            var parameters = new List<object> { eventName };
            foreach (var eventItem in eventItems)
            {
                var itemName = eventItem.Name.Replace('.', '_');
                messageBuilder.Append($"{eventItem.Name} = {{{itemName}}}; ");
                parameters.Add(eventItem.Value);
            }

            logger.Log(logLevel, messageBuilder.ToString(), parameters.ToArray());
        }
    }
}