namespace Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;

public class FileMetadata : ICreatedAt
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required long CipherSize { get; set; }
}
