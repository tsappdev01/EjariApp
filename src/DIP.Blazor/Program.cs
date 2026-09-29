using DIP.Blazor.Pages;
using DIP.Connected_Services.DocumentIntelligent;
using DIP.Interface;
using DIP.QrService;
using DIP.Services;
using DIP.UaePassService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace DIP.Blazor;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
       .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 10485760)//10 MB Size (10  * 1024 * 1024)
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<DIPBlazorModule>();
            builder.Services.AddScoped<IUaePassClient, UaePassClient>();
            builder.Services.AddScoped<IDocumentIntelligentClient, DocumentIntelligentClient>();
            builder.Services.AddScoped<IQrServiceClient, QrServiceClient>();
            builder.Services.AddScoped<IDIPAppsettingService, DIPAppsettingService>();
            builder.Services.AddScoped<ICCPaymentHelpers, CCPaymentHelpers>();

            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
