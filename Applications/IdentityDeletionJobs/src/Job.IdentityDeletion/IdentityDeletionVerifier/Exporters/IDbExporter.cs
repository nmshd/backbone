namespace Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;

public interface IDbExporter
{
    Task ExportDb(string targetFile, CancellationToken cancellationToken);
}
