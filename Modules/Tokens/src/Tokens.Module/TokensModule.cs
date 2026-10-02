using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.OpenApi;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Tokens.Infrastructure;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Module;

public class TokensModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Tokens";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.Configure<OpenApiSchemaNames>(names =>
        {
            names.Overrides[typeof(Features.Tokens.Shared.Response)] = "ListTokensResponse";
        });

        services.AddPersistence(infrastructureConfiguration.SqlDatabase);

        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(TokensModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            .AddOpenBehavior(typeof(DbConcurrencyBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(TokensModule).Assembly);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override Task ConfigureEventBus(IEventBus eventBus)
    {
        return Task.CompletedTask;
    }
}
