namespace Backbone.AdminCli.Commands.Database.Types;

public class SyncErrorExport
{
    public required string ErrorId { get; set; }
    public required string SyncItemOwner { get; set; }
    public string? SyncItemOwnerClientName { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required string ErrorCode { get; set; }
}
