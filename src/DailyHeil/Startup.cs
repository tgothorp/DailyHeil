using System;
using System.Globalization;
using System.Threading;
using DailyHeil.Functions.Infrastructure;
using DailyHeil.Functions.Infrastructure.DailyHeilContextFactory;
using DailyHeil.Functions.Infrastructure.RazorEngine;
using DailyHeil.Functions.Infrastructure.RssReader;
using DailyHeil.Functions.Infrastructure.SiteScraperFactory;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: FunctionsStartup(typeof(DailyHeil.Functions.Startup))]

namespace DailyHeil.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            InitializeCulture();
            InitializeLogging();
            
            Log.Logger.Information($"Configuring services...");
            ConfigureServices(builder.Services);
            Log.Logger.Information($"Services configured, starting functions...");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(x => x.AddSerilog(dispose: true))
                .AddSingleton<FunctionConfiguration>()
                .AddSingleton<IRssReader, RssReader>()
                .AddSingleton<IDailyHeilContextFactory, DailyHeilContextFactory>()
                .AddSingleton<ISiteScraperFactory, SiteScraperFactory>()
                .AddSingleton<IRazorEngine, RazorEngine>();
        }
        
        private static void InitializeCulture()
        {
            var culture = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        private static void InitializeLogging()
        {
            var environment = Environment.GetEnvironmentVariable("Environment");
            if (string.IsNullOrEmpty(environment))
                throw new Exception($"Could not load environment varible");
            
            if (environment == "Development")
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Debug()
                    .CreateLogger();
            }
            else
            {
                // Basically Microsoft are warning not to use TelemetryConfiguration.Active and instead provider an instance of
                // a TelemetryConfiguration object, shouldn't be a massive fix as all you really need to do is provide an instrumentation
                // key, will fix this if I ever get round it it.

#pragma warning disable 618
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                    .CreateLogger();
#pragma warning restore 618
            }
        }
    }
}