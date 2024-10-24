using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Tags.Application;
using Backbone.Modules.Tags.Application.Extensions;
using Backbone.Modules.Tags.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tags.ConsumerApi;

public class TagsModule : AbstractModule
{
    public override string Name => "Tags";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));

        services.AddApplication();
        services.AddPersistence();
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
        // No Event bus needed here
    }
}
