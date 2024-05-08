using Byndyusoft.Logging.Builders;
using Byndyusoft.Logging.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Byndyusoft.Logging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) => configuration
                    .UseOpenTelemetryTraces()
                    .UseFileWriterSettings()
                    .UseDefaultSettings(context.Configuration)
                    .WriteToOpenTelemetry(activityEventBuilder: StructuredActivityEventBuilder.Instance)
                )
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}