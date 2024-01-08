using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Tokens.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Tokens.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DbOptions options)
    {
        services
            .AddDbContext<TokensDbContext>(dbContextOptions =>
                {
                    switch (options.Provider)
                    {
                        case SQLSERVER:
                            dbContextOptions.UseSqlServer(options.ConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                            }).UseModel(Modules.Tokens.Infrastructure.CompiledModels.SqlServer.TokensDbContextModel.Instance);
                            break;
                        case POSTGRES:
                            dbContextOptions.UseNpgsql(options.ConnectionString, sqlOptions =>
                            {
                                sqlOptions.CommandTimeout(20);
                                sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                                sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                                sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Tokens"); //TODO: Remove this once the issue with package 'Npgsql.EntityFrameworkCore.PostgreSQL' is fixed https://github.com/npgsql/efcore.pg/issues/2878
                            });//.UseModel(Modules.Tokens.Infrastructure.CompiledModels.Postgres.TokensDbContextModel.Instance); TODO: Add this when issues with PostgreSQL compiled models are fixed https://github.com/npgsql/efcore.pg/issues/2972
                            break;
                        default:
                            throw new Exception($"Unsupported database provider: {options.Provider}");
                    }
                }
            );
    }
}
