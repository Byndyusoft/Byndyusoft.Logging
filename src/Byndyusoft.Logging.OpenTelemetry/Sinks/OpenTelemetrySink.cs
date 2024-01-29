using System;
using System.Diagnostics;
using Byndyusoft.Logging.Builders;
using Byndyusoft.Logging.Builders.Interfaces;
using Serilog.Core;
using Serilog.Events;

namespace Byndyusoft.Logging.Sinks
{
    /// <summary>
    ///     Записывает логи в лог спана
    /// </summary>
    public class OpenTelemetrySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IActivityEventBuilder _activityEventBuilder;

        public OpenTelemetrySink(
            IFormatProvider formatProvider = null,
            IActivityEventBuilder activityEventBuilder = null)
        {
            _formatProvider = formatProvider;
            _activityEventBuilder = activityEventBuilder ?? DefaultActivityEventBuilder.Instance;
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
                var activityLogEvent = _activityEventBuilder.Build(_formatProvider, logEvent);
                activity.AddEvent(activityLogEvent);
            }
            catch (Exception logException)
            {
                var activityExceptionEvent = _activityEventBuilder.BuildForLogException(logException);
                activity.AddEvent(activityExceptionEvent);
            }
        }
    }
}