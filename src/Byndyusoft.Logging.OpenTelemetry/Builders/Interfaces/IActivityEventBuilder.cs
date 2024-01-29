using System;
using System.Diagnostics;
using Serilog.Events;

namespace Byndyusoft.Logging.Builders.Interfaces
{
    /// <summary>
    /// Строитель события активности (для OpenTelemetry) на основе лога
    /// </summary>
    public interface IActivityEventBuilder
    {
        /// <summary>
        /// Строение события активности на основе строки лога
        /// </summary>
        /// <param name="formatProvider"><see cref="IFormatProvider"/>></param>
        /// <param name="logEvent">Строка лога</param>
        /// <returns>Событие активности <see cref="ActivityEvent"/></returns>
        ActivityEvent Build(IFormatProvider formatProvider, LogEvent logEvent);

        /// <summary>
        /// Строение события активности на основе ошибки, возникшего при построении активности
        /// </summary>
        /// <param name="logException">Ошибка, возникшая при построении активности</param>
        /// <returns>Событие активности <see cref="ActivityEvent"/></returns>
        ActivityEvent BuildForLogException(Exception logException);
    }
}