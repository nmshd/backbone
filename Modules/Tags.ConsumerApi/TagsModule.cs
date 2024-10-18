using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Tags.Application;
using Backbone.Modules.Tags.Application.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tags.ConsumerApi;

public class TagsModule : AbstractModule
{
    public override string Name => "Tags";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;

        services.AddApplication();
        services.AddTags(parsedConfiguration);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
        //No Event bus needed here
    }
}
