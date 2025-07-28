// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class SyncError
{
    public required string Id { get; init; }
    public virtual required ExternalEvent ExternalEvent { get; init; }
    public virtual required SyncRun SyncRun { get; init; }
    public required string ErrorCode { get; init; }
}

public class ExternalEvent
{
    public required string Id { get; init; }
    public required string Owner { get; init; }
}

public class SyncRun
{
    public required string Id { get; init; }
    public required DateTime? FinalizedAt { get; init; }
}
