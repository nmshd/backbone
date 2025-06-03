namespace Backbone.AdminCli.Commands.Database.Model;

public class SyncErrorExport
{
    public required string ErrorId { get; set; }
    public required string SyncItemOwner { get; set; }
    public required string? SyncItemOwnerClientName { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required string ErrorCode { get; set; }
}
