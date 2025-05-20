namespace Backbone.AdminCli.Commands.Database.Types;

public class FileExport
{
    public required string FileId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Owner { get; set; }
    public string? OwnerClientName { get; set; }
    public required long CipherSize { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
