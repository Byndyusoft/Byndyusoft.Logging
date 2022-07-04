using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using OpenTracing;
using OpenTracing.Util;
using Tracer = Jaeger.Tracer;

namespace Byndyusoft.Logging.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                var reporter = new NoopReporter();

                ITracer tracer = new Tracer.Builder("sample service")
                    .WithReporter(reporter)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.RegisterIfAbsent(tracer);

                return tracer;
            });

            services.AddOpenTracing();

            services.AddOpenTelemetryTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddConsoleExporter()
                    .AddAspNetCoreInstrumentation()
                    .AddSource("SampleApp")
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
