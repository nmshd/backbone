﻿using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER = "SqlServer";
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Files.Infrastructure.Database.SqlServer";
    private const string POSTGRES = "Postgres";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.Modules.Files.Infrastructure.Database.Postgres";

    public static void AddDatabase(this IServiceCollection services, Action<DbOptions> setupOptions)
    {
        var options = new DbOptions();
        setupOptions?.Invoke(options);

        services.AddDatabase(options);
    }

    public static void AddDatabase(this IServiceCollection services, DbOptions options)
    {
        services
            .AddDbContext<FilesDbContext>(dbContextOptions =>
            {
                switch (options.Provider)
                {
                    case SQLSERVER:
                        dbContextOptions.UseSqlServer(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(SQLSERVER_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        }).UseModel(CompiledModels.SqlServer.FilesDbContextModel.Instance);
                        break;
                    case POSTGRES:
                        dbContextOptions.UseNpgsql(options.DbConnectionString, sqlOptions =>
                        {
                            sqlOptions.CommandTimeout(20);
                            sqlOptions.MigrationsAssembly(POSTGRES_MIGRATIONS_ASSEMBLY);
                            sqlOptions.EnableRetryOnFailure(options.RetryOptions.MaxRetryCount, TimeSpan.FromSeconds(options.RetryOptions.MaxRetryDelayInSeconds), null);
                        }).UseModel(CompiledModels.Postgres.FilesDbContextModel.Instance);
                        break;
                    default:
                        throw new Exception($"Unsupported database provider: {options.Provider}");
                }
            });

        services.AddScoped<IFilesDbContext, FilesDbContext>();
    }
}

public class DbOptions
{
    public string Provider { get; set; }
    public string DbConnectionString { get; set; }
    public RetryOptions RetryOptions { get; set; } = new();
}

public class RetryOptions
{
    public byte MaxRetryCount { get; set; } = 15;
    public int MaxRetryDelayInSeconds { get; set; } = 30;
}
