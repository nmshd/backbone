using System.Diagnostics.CodeAnalysis;

namespace Files.Jobs.SanityCheck;

[ExcludeFromCodeCoverage]
internal static class IConfigurationExtensions
{
    public static BlobStorageConfiguration GetBlobStorageConfiguration(this IConfiguration configuration)
    {
        return new BlobStorageConfiguration(configuration);
    }

    public static SqlDatabaseConfiguration GetSqlDatabaseConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("SqlDatabase").Get<SqlDatabaseConfiguration>() ?? new SqlDatabaseConfiguration();
    }
}

public class SqlDatabaseConfiguration
{
    public string ConnectionString { get; set; }
}

public class BlobStorageConfiguration
{
    private readonly IConfigurationSection _blobStorageConfiguration;

    public BlobStorageConfiguration(IConfiguration configuration)
    {
        _blobStorageConfiguration = configuration.GetSection("BlobStorage");
    }

    public string ConnectionString => _blobStorageConfiguration["ConnectionString"] ?? "";
    public string CloudProvider => _blobStorageConfiguration["CloudProvider"] ?? "Azure";
    public string ContainerName => _blobStorageConfiguration["ContainerName"] ?? "";
}