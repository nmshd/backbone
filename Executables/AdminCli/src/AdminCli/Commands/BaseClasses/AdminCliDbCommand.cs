using System.CommandLine;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.AdminCli.Commands.BaseClasses;

public abstract class AdminCliDbCommand : AdminCliCommand
{
    protected static readonly Option<string> DB_CONNECTION_STRING_OPTION =
        new("-c", "The connection string to the database.");

    protected static readonly Option<string> DB_PROVIDER_OPTION =
        new("-p", "The database provider. Possible values: Postgres, SqlServer");

    protected AdminCliDbCommand(string name, ServiceLocator serviceLocator, string? description = null) : base(name, serviceLocator, description)
    {
        AddOption(DB_PROVIDER_OPTION);
        AddOption(DB_CONNECTION_STRING_OPTION);
    }

    static AdminCliDbCommand()
    {
        DB_PROVIDER_OPTION.AddAlias("--dbProvider");
        DB_PROVIDER_OPTION.SetDefaultValueFactory(GetDbProviderFromEnvVar);
        DB_PROVIDER_OPTION.AddValidator(result => { result.ErrorMessage = ValidateProvider(result.GetValueOrDefault<string>()); });

        DB_CONNECTION_STRING_OPTION.AddAlias("--dbConnectionString");
        DB_CONNECTION_STRING_OPTION.SetDefaultValueFactory(GetDbConnectionStringFromEnvVar);
        DB_CONNECTION_STRING_OPTION.AddValidator(result => { result.ErrorMessage = ValidateDbConnectionString(result.GetValueOrDefault<string>()); });
    }

    private static string? GetDbProviderFromEnvVar()
    {
        var provider = Environment.GetEnvironmentVariable("Database__Provider");

        if (provider.IsNullOrEmpty())
            provider = Environment.GetEnvironmentVariable("Database:Provider");

        return provider;
    }

    private static string? GetDbConnectionStringFromEnvVar()
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString");

        if (connectionString.IsNullOrEmpty())
            connectionString = Environment.GetEnvironmentVariable("Database:ConnectionString");

        return connectionString;
    }

    private static string? ValidateDbConnectionString(string? connectionString)
    {
        return connectionString.IsNullOrEmpty()
            ? "You need to specify a database connection string by passing it via an option (-c/--connectionString) or by setting it via environment variable (Database__ConnectionString/Database:ConnectionString)."
            : null;
    }

    private static string? ValidateProvider(string? provider)
    {
        return provider.IsNullOrEmpty()
            ? "You need to specify a database provider by passing it via an option (-p/--provider) or by setting it via environment variable (Database__Provider/Database:Provider)."
            : null;
    }
}
