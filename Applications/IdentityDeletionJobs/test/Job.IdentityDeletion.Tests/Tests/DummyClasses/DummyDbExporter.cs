using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;

namespace Backbone.Job.IdentityDeletion.Tests.Tests.DummyClasses;

public class DummyDbExporter : IDbExporter
{
    public Task ExportDb(string targetFile, CancellationToken cancellationToken) => Task.CompletedTask;
}
