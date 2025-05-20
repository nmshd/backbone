namespace Backbone.AdminCli.Commands.Database.Types;

public class TokenExport
{
    public required string TokenId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required long CipherSize { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
