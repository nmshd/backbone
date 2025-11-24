using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;
using PgDump;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;

public class PostgresDbExporter : IDbExporter
{
    private readonly DatabaseConfiguration _configuration;
    private readonly ILogger<PostgresDbExporter> _logger;

    public PostgresDbExporter(IOptions<IdentityDeletionJobConfiguration> configuration, ILogger<PostgresDbExporter> logger)
    {
        _configuration = configuration.Value.Infrastructure.SqlDatabase;
        _logger = logger;
    }

    public async Task ExportDb(string targetFile, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Postgres database dump");

        var parts = _configuration.ConnectionString.Split(';');
        var host = parts.ExtractString("Server=");
        var port = int.Parse(parts.ExtractString("Port="));
        var username = parts.ExtractString("User ID=");
        var password = parts.ExtractString("Password=");
        var database = parts.ExtractString("Database=");

        var connection = new ConnectionOptions(host, port, username, password, database);
        var client = new PgClient(connection);

        await client.DumpAsync(new FileOutputProvider(targetFile), 1.Minutes(), DumpFormat.Plain, cancellationToken);

        _logger.LogInformation("Postgres database dump completed");
    }
}
