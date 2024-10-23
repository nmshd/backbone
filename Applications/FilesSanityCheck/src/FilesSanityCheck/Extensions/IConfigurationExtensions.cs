using System.ComponentModel.DataAnnotations;

namespace Backbone.FilesSanityCheck.Extensions;

internal static class IConfigurationExtensions
{
    public static BlobStorageConfiguration GetBlobStorageConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("BlobStorage").Get<BlobStorageConfiguration>() ?? new BlobStorageConfiguration();
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

public class BlobStorageConfiguration
{
    [Required]
    [MinLength(1)]
    [RegularExpression("Azure|GoogleCloud|Ionos")]
    public string CloudProvider { get; set; } = string.Empty;

    public string ConnectionInfo { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;

    public IonosS3Config? IonosS3Config { get; set; }
}

public class IonosS3Config
{
    [Required]
    public string ServiceUrl { get; set; } = string.Empty;

    [Required]
    public string AccessKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string BucketName { get; set; } = string.Empty;
}
