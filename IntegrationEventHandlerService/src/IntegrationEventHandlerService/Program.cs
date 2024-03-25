using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.IntegrationEventHandlerService;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine("IntegrationEventHandlerService - Creating app..."); //Log.Information("Creating app...");

            var app = CreateHostBuilder(args);

            Console.WriteLine("IntegrationEventHandlerService - App created."); //Log.Information("App created.");
            Console.WriteLine("IntegrationEventHandlerService - Starting app..."); //Log.Information("Starting app...");

            await app.Build().RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IntegrationEventHandlerService - {ex} - host terminated unexpectedly"); //Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Console.WriteLine("IntegrationEventHandlerService - host duty ended"); //Log.CloseAndFlush();
        }

        Console.Read();
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
