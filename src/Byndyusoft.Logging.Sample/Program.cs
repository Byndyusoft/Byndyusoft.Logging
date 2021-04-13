using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Byndyusoft.Logging.Configuration;
using Serilog;

namespace Byndyusoft.Logging.Sample
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
                    .UseOpenTracingTraces()
                    .UseFileWriterSettings()
                    .UseDefaultSettings(context.Configuration, "Sample project")
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
