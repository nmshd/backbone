using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.OpenApi;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Announcements.Infrastructure;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsForActiveIdentityInLanguage;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Module;

public class AnnouncementsModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Announcements";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(AnnouncementsModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(AnnouncementsModule).Assembly);

        services.AddDatabase(infrastructureConfiguration.SqlDatabase);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
