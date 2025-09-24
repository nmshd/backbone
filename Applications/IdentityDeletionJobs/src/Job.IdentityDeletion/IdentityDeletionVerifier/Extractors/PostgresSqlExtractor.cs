using System.Text;
using System.Text.RegularExpressions;
using Backbone.Tooling.Extensions;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

public partial class PostgresSqlExtractor : ISqlExtractor
{
    [GeneratedRegex("""
                    \"[^\"]*\"
                    """)]
    private static partial Regex TableNameAndSchemaRegex();

    private readonly ILogger<PostgresSqlExtractor> _logger;

    public PostgresSqlExtractor(ILogger<PostgresSqlExtractor> logger)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<ExtractedTable> ExtractTables(string file)
    {
        _logger.LogInformation("Extracting dumped postgres data");

        var reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read), Encoding.UTF8);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line.IsNullOrEmpty()) continue;

            if (line.Trim().StartsWith("COPY"))
            {
                var id = ExtractTableNameAndSchema(line);
                var entryLines = ReadCopyLines(reader);
                yield return new ExtractedTable
                {
                    Id = id,
                    EntryLines = entryLines
                };
            }
        }

        _logger.LogInformation("Extraction complete");
    }

    private static TableId ExtractTableNameAndSchema(string startLine)
    {
        var matches = TableNameAndSchemaRegex().Matches(startLine);
        var schemaName = matches[0].Value.Trim('"');
        var tableName = matches[1].Value.Trim('"');

        return new TableId
        {
            Schema = schemaName,
            Table = tableName
        };
    }

    private static async IAsyncEnumerable<string> ReadCopyLines(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null or "\\.") break;

            yield return line;
        }
    }
}
