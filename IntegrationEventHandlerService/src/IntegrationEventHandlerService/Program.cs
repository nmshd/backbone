using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.IntegrationEventHandlerService;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("IntegrationEventHandlerService - Creating app..."); //Log.Information("Creating app...");

            var hostBuilder = CreateHostBuilder(args);
            var host = hostBuilder.Build();

            Console.WriteLine("IntegrationEventHandlerService - App created."); //Log.Information("App created.");
            Console.WriteLine("IntegrationEventHandlerService - Starting app..."); //Log.Information("Starting app...");

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IntegrationEventHandlerService - {ex} - host terminated unexpectedly"); //Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Console.WriteLine("IntegrationEventHandlerService - host duty ended"); //Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {

            })
            .ConfigureServices((hostContext, services) =>
            {

            });
}
