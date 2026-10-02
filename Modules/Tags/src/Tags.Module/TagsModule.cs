using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Tags.Abstractions;
using Backbone.Modules.Tags.Infrastructure.Persistence;
using Backbone.Modules.Tags.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tags.Module;

public class TagsModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Tags";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration _, IConfigurationSection __)
    {
        services.AddTransient<IOptions<TagsRepositoryOptions>>(sp => Options.Create(new TagsRepositoryOptions
        {
            SupportedLanguages = sp.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.SupportedLanguages,
            TagsForAttributeValueTypes = sp.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.TagsForAttributeValueTypes
        }));
        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(TagsModule).Assembly)
        );
        services.AddPersistence();
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
