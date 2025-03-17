using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Tags.Application;
using Backbone.Modules.Tags.Application.Extensions;
using Backbone.Modules.Tags.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.Module;

public class TagsModule : AbstractModule<ApplicationOptions, InfrastructureConfiguration>
{
    public override string Name => "Tags";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration _, IConfigurationSection __)
    {
        services.AddApplication();
        services.AddPersistence();
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
