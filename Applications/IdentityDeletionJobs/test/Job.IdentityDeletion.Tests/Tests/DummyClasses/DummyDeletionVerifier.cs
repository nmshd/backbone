using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;
using Backbone.Tooling.Extensions;

namespace Backbone.Job.IdentityDeletion.Tests.Tests.DummyClasses;

public class DummyDeletionVerifier : IDeletionVerifier
{
    private static readonly TableId TEST_ID = new() { Schema = "Test", Table = "Test" };

    private readonly DatabaseCheckResult _result;

    public DummyDeletionVerifier(Dictionary<string, int> occurrences)
    {
        Dictionary<TableId, Dictionary<string, int>> found = [];
        if (occurrences.Count != 0)
            found[TEST_ID] = occurrences;

        _result = new DatabaseCheckResult
        {
            Success = occurrences.IsEmpty(),
            FoundOccurrences = found
        };
    }

    public Task<DatabaseCheckResult> VerifyDeletion(List<string> addressesToVerify, CancellationToken cancellationToken) => Task.FromResult(_result);
    public Task SaveFoundOccurrences(DatabaseCheckResult result, CancellationToken cancellationToken) => Task.CompletedTask;
}
