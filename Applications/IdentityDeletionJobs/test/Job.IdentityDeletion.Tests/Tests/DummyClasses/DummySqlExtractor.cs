using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

namespace Backbone.Job.IdentityDeletion.Tests.Tests.DummyClasses;

public class DummySqlExtractor : ISqlExtractor
{
    public static readonly TableId TEST_ID = new() { Schema = "Test", Table = "Test" };

    private readonly ExtractedTable _table;

    public DummySqlExtractor(List<string> lines)
    {
        _table = new ExtractedTable
        {
            Id = TEST_ID,
            EntryLines = lines.ToAsyncEnumerable()
        };
    }

    public IAsyncEnumerable<ExtractedTable> ExtractTables(string file) => new[] { _table }.ToAsyncEnumerable();
}
