using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.OpenApi;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Files.Infrastructure;
using Backbone.Modules.Files.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Module;

public class FilesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Files";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.Configure<OpenApiSchemaNames>(names =>
        {
            names.Overrides[typeof(Features.Files.ClaimFileOwnership.RequestBody)] = "ClaimFileOwnershipRequest";
        });

        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(FilesModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(FilesModule).Assembly);

        services.AddPersistence(infrastructureConfiguration.SqlDatabase, infrastructureConfiguration.BlobStorage);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
