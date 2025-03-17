using System.CommandLine;
using Backbone.AdminCli.Configuration;
using Backbone.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.AdminCli;

public static class IServiceCollectionExtensions
{
    public static void AddCliCommands(this IServiceCollection services)
    {
        var commands = typeof(Program)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Command)) && !t.IsAbstract);

        foreach (var command in commands)
        {
            services.AddTransient(command);
        }
    }

    public static void AddCommonInfrastructure(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
#pragma warning disable ASP0000 // We retrieve the Configuration via IOptions here so that it is validated
        var parsedConfiguration = serviceProvider.GetRequiredService<IOptions<AdminCliConfiguration>>().Value;
#pragma warning restore ASP0000

        services.AddEventBus(parsedConfiguration.Infrastructure.EventBus);
    }
}
