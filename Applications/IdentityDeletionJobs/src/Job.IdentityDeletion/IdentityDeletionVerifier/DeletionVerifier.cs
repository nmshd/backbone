using System.Text.Json;
using System.Text.RegularExpressions;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier;

public class DeletionVerifier : IDeletionVerifier
{
    private static readonly JsonSerializerOptions SERIALIZER_OPTIONS = new()
    {
        IndentSize = 4,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly IDbExporter _dbExporter;
    private readonly ISqlExtractor _sqlExtractor;

    public DeletionVerifier(IDbExporter dbExporter, ISqlExtractor sqlExtractor)
    {
        _dbExporter = dbExporter;
        _sqlExtractor = sqlExtractor;
    }

    public async Task<DatabaseCheckResult> VerifyDeletion(List<string> addressesToVerify, CancellationToken cancellationToken)
    {
        await ExportDatabase(cancellationToken);

        return await CheckExportedDatabase(addressesToVerify, cancellationToken);
    }

    private async Task ExportDatabase(CancellationToken cancellationToken)
    {
        if (!FilePaths.TempDirExists()) Directory.CreateDirectory(FilePaths.PATH_TO_TEMP_DIR);

        await _dbExporter.ExportDb(FilePaths.PATH_TO_DUMP_FILE, cancellationToken);
    }

    private async Task<DatabaseCheckResult> CheckExportedDatabase(List<string> addressesToCheck, CancellationToken cancellationToken)
    {
        var result = new DatabaseCheckResult
        {
            Success = true,
            FoundOccurrences = []
        };

        await foreach (var extractedTable in _sqlExtractor.ExtractTables(FilePaths.PATH_TO_DUMP_FILE).WithCancellation(cancellationToken))
        {
            var found = addressesToCheck.ToDictionary(i => i, _ => 0);

            await foreach (var line in extractedTable.EntryLines.WithCancellation(cancellationToken))
            {
                foreach (var identity in addressesToCheck)
                {
                    var count = Regex.Matches(line, identity).Count;
                    found[identity] += count;

                    if (count != 0) result.Success = false;
                }
            }

            if (found.Any(entry => entry.Value != 0))
                result.FoundOccurrences.Add(extractedTable.Id, found.Where(kvp => kvp.Value != 0).ToDictionary());
        }

        return result;
    }

    public async Task SaveFoundOccurrences(DatabaseCheckResult result, CancellationToken cancellationToken)
    {
        var groupedOccurrences = result.FoundOccurrences
            .GroupBy(x => x.Key.Schema)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    v => v.Key.Table,
                    v => v.Value
                )
            );

        await using var stream = new FileStream(FilePaths.PATH_TO_FOUND_FILE, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, groupedOccurrences, SERIALIZER_OPTIONS, cancellationToken);
    }
}

public class DeletionFailedException(DatabaseCheckResult result)
    : Exception($"Some identities were still found in the database ({result.NumberOfOccurrences} times in total). Check \"{FilePaths.PATH_TO_FOUND_FILE}\" for more details.");

file static class FilePaths
{
    private const string DUMP_FILENAME = "db-dump.sql";
    private const string FOUND_FILENAME = "found-identities.json";

    public static readonly string PATH_TO_TEMP_DIR = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone");
    public static readonly string PATH_TO_DUMP_FILE = Path.Combine(PATH_TO_TEMP_DIR, DUMP_FILENAME);
    public static readonly string PATH_TO_FOUND_FILE = Path.Combine(PATH_TO_TEMP_DIR, FOUND_FILENAME);

    public static bool TempDirExists() => Directory.Exists(PATH_TO_TEMP_DIR);
}
