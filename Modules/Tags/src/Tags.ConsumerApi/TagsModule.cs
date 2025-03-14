using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Tags.Application;
using Backbone.Modules.Tags.Application.Extensions;
using Backbone.Modules.Tags.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.ConsumerApi;

public class TagsModule : AbstractModule<ApplicationOptions, Configuration.InfrastructureConfiguration>
{
    public override string Name => "Tags";

    protected override void ConfigureServices(IServiceCollection services, Configuration.InfrastructureConfiguration _, IConfigurationSection __)
    {
        services.AddApplication();
        services.AddPersistence();
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
