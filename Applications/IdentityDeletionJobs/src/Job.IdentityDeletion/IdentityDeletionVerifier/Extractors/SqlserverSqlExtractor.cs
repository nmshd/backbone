using System.Text;
using Backbone.Tooling.Extensions;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

public class SqlserverSqlExtractor : ISqlExtractor
{
    private readonly ILogger<SqlserverSqlExtractor> _logger;

    public SqlserverSqlExtractor(ILogger<SqlserverSqlExtractor> logger)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<ExtractedTable> ExtractTables(string file)
    {
        _logger.LogInformation("Extracting dumped sql server data");

        var reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line.IsNullOrEmpty()) continue;

            if (line.StartsWith("table"))
            {
                var id = ExtractTableNameAndSchema(line);
                var lines = ExtractLines(reader);
                yield return new ExtractedTable
                {
                    Id = id,
                    EntryLines = lines
                };
            }
        }

        _logger.LogInformation("Extraction complete");
    }

    private static TableId ExtractTableNameAndSchema(string line)
    {
        var parts = line.Split(';');
        return new TableId
        {
            Schema = parts[1],
            Table = parts[2]
        };
    }

    private static async IAsyncEnumerable<string> ExtractLines(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line.IsNullOrEmpty()) break;

            yield return line;
        }
    }
}
