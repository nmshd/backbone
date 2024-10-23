using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;

namespace Backbone.FilesSanityCheck.Extensions;

internal static class IConfigurationExtensions
{
    public static BlobStorageOptions GetBlobStorageConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("BlobStorage").Get<BlobStorageOptions>() ?? new BlobStorageOptions();
    }

    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("SqlDatabase").Get<SqlDatabaseConfiguration>() ?? new SqlDatabaseConfiguration();
    }
}

public class SqlDatabaseConfiguration
{
    [Required]
    [MinLength(1)]
    [RegularExpression("SqlServer|Postgres")]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = string.Empty;
}
