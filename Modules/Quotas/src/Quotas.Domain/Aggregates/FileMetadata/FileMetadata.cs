namespace Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
public class FileMetadata : ICreatedAt
{
    public string? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public long CipherSize { get; set; }
}
