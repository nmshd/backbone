using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;

namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier;

public interface IDeletionVerifier
{
    Task<DatabaseCheckResult> VerifyDeletion(List<string> addressesToVerify, CancellationToken cancellationToken);
    Task SaveFoundOccurrences(DatabaseCheckResult result, CancellationToken cancellationToken);
}

public record DatabaseCheckResult
{
    public required bool Success { get; set; }
    public required Dictionary<TableId, Dictionary<string, int>> FoundOccurrences { get; init; }

    public int NumberOfOccurrences => FoundOccurrences
        .SelectMany(v => v.Value.Values)
        .Aggregate((a, b) => a + b);
}
