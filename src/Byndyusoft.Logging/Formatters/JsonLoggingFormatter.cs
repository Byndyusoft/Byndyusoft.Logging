using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace Byndyusoft.Logging.Formatters
{
    //JsonFormatter устарел и будет удалён, поэтому сделаем свой форматтер на основании RenderedCompactJsonFormatter
    //https://github.com/serilog/serilog-formatting-compact/blob/dev/src/Serilog.Formatting.Compact/Formatting/Compact/RenderedCompactJsonFormatter.cs
    public class JsonLoggingFormatter : ITextFormatter
    {
        readonly JsonValueFormatter _valueFormatter;

        /// <summary>
        ///     Construct a <see cref="CompactJsonFormatter" />, optionally supplying a formatter for
        ///     <see cref="LogEventPropertyValue" />s on the event.
        /// </summary>
        /// <param name="valueFormatter">A value formatter, or null.</param>
        public JsonLoggingFormatter(JsonValueFormatter valueFormatter = null)
        {
            this._valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
        }

        /// <summary>
        ///     Format the log event into the output. Subsequent events will be newline-delimited.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatEvent(logEvent, output, _valueFormatter);
            output.WriteLine();
        }

        /// <summary>
        ///     Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue" />s on the event.</param>
        public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (valueFormatter == null)
                throw new ArgumentNullException(nameof(valueFormatter));

            output.Write("{\"Timestamp\":\"");
            output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
            output.Write("\",\"Message\":");
            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, output);
            output.Write(",\"MessageTemplateHash\":\"");
            var id = EventIdHash.Compute(logEvent.MessageTemplate.Text);
            output.Write(id.ToString("x8"));
            output.Write('"');
            output.Write(",\"Level\":\"");
            output.Write(logEvent.Level);
            output.Write('\"');

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);

                var builder = new StringBuilder();
                for (var exception = logEvent.Exception; exception != null; exception = exception.InnerException)
                {
                    builder.AppendLine(exception.GetType().AssemblyQualifiedName);
                    builder.AppendLine(new StackTrace(exception, false).ToString());
                }

                output.Write(",\"ExceptionHash\":\"");
                var errorId = EventIdHash.Compute(builder.ToString());
                output.Write(errorId.ToString("x8"));
                output.Write('"');
            }

            output.Write(",\"Properties\": {");

            bool isFirstProperty = true;
            foreach (var property in logEvent.Properties)
            {
                if (isFirstProperty)
                {
                    isFirstProperty = false;
                }
                else
                {
                    output.Write(',');
                }

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                valueFormatter.Format(property.Value, output);
            }

            output.Write("}}");
        }
    }
}