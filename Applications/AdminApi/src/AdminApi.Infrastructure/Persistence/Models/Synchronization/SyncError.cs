namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class SyncError
{
    public string Id { get; set; } = null!;
    public ExternalEvent ExternalEvent { get; set; } = null!;
    public SyncRun SyncRun { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
}

public class ExternalEvent
{
    public string Id { get; set; } = null!;
    public string Owner { get; set; } = null!;
}

public class SyncRun
{
    public string Id { get; set; } = null!;
    public DateTime? FinalizedAt { get; set; }
}
