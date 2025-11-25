using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public static class IMutableModelExtensions
{
    private const string PROVIDER_ANNOTATION = "DbProvider";
    private const string PROVIDER_POSTGRES = "Npgsql";
    private const string PROVIDER_SQLSERVER = "SqlServer";

    extension(IMutableModel model)
    {
        public void SetDbProvider(DatabaseFacade database)
        {
            model.AddAnnotation(PROVIDER_ANNOTATION, database.IsNpgsql() ? PROVIDER_POSTGRES : PROVIDER_SQLSERVER);
        }

        public string? GetDbProvider()
        {
            return model.FindAnnotation(PROVIDER_ANNOTATION)?.Value as string;
        }

        public bool IsNpgsql()
        {
            return model.GetDbProvider() is PROVIDER_POSTGRES;
        }

        public bool IsSqlServer()
        {
            return model.GetDbProvider() is PROVIDER_SQLSERVER;
        }
    }
}
