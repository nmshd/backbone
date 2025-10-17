namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

public interface ISqlExtractor
{
    IAsyncEnumerable<ExtractedTable> ExtractTables(string file);
}

public record ExtractedTable
{
    public required TableId Id { get; init; }
    public required IAsyncEnumerable<string> EntryLines { get; init; }
}

public record TableId
{
    public required string Schema { get; init; }
    public required string Table { get; init; }
}
