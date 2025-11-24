using System.Text;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;

public class SqlServerDbExporter : IDbExporter
{
    private readonly DatabaseConfiguration _configuration;
    private readonly ILogger<SqlServerDbExporter> _logger;

    public SqlServerDbExporter(IOptions<IdentityDeletionJobConfiguration> configuration, ILogger<SqlServerDbExporter> logger)
    {
        _configuration = configuration.Value.Infrastructure.SqlDatabase;
        _logger = logger;
    }

    public async Task ExportDb(string targetFile, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Sql Server database dump");

        var parts = _configuration.ConnectionString.Split(';');
        var databaseName = parts.ExtractString("Database=");
        var server = new Server(new ServerConnection(new SqlConnection(_configuration.ConnectionString)));
        var database = server.Databases[databaseName] ?? throw new ArgumentException($"No database with name \"{databaseName}\" found in sql server");
        var scripter = new Scripter(server)
        {
            Options = new ScriptingOptions
            {
                EnforceScriptingOptions = true,
                WithDependencies = false,
                IncludeHeaders = false,
                ScriptDrops = false,
                AppendToFile = false,
                ScriptSchema = false,
                ScriptData = true,
                IncludeIfNotExists = false,
                Default = true,
                Indexes = false
            }
        };

        await using var writer = new StreamWriter(new FileStream(targetFile, FileMode.Create), Encoding.UTF8);
        foreach (var table in database.Tables.Cast<Table>())
        {
            if (table.IsSystemObject) continue;

            _logger.LogInformation("Writing table {schema}.{table}", table.Schema, table.Name);

            await writer.WriteLineAsync($"table;{table.Schema};{table.Name}");

            foreach (var line in scripter.EnumScript(new[] { table.Urn }))
            {
                await writer.WriteLineAsync(line);
            }

            await writer.WriteLineAsync();
            await writer.FlushAsync(cancellationToken);
        }

        _logger.LogInformation("Sql Server database dump completed");
    }
}
