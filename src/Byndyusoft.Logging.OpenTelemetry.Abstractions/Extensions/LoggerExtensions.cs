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
}