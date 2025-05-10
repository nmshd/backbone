using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class IMutableModelExtensions
{
    private const string PROVIDER_ANNOTATION = "DbProvider";
    private const string PROVIDER_POSTGRES = "Npgsql";
    private const string PROVIDER_SQLSERVER = "SqlServer";

    public static void SetDbProvider(this IMutableModel model, DatabaseFacade database)
    {
        model.AddAnnotation(PROVIDER_ANNOTATION, database.IsNpgsql() ? PROVIDER_POSTGRES : PROVIDER_SQLSERVER);
    }

    public static string? GetDbProvider(this IMutableModel model)
    {
        return model.FindAnnotation(PROVIDER_ANNOTATION)?.Value as string;
    }

    public static bool IsNpgsql(this IMutableModel model)
    {
        return model.GetDbProvider() is PROVIDER_POSTGRES;
    }

    public static bool IsSqlServer(this IMutableModel model)
    {
        return model.GetDbProvider() is PROVIDER_SQLSERVER;
    }
}
