namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Files;

public class File
{
    public string Id { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Owner { get; set; } = null!;
    public long CipherSize { get; set; }
    public DateTime ExpiresAt { get; set; }
}
