namespace Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
public class FileMetadata
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public int CipherSize { get; set; }
}
