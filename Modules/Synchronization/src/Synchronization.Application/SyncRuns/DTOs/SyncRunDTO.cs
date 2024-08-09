using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

public class SyncRunDTO
{
    public SyncRunDTO()
    {
    }

    public SyncRunDTO(SyncRun syncRun)
    {
        Id = syncRun.Id;
        Type = syncRun.Type;
        ExpiresAt = syncRun.ExpiresAt;
        Index = syncRun.Index;
        CreatedAt = syncRun.CreatedAt;
        CreatedBy = syncRun.CreatedBy;
        CreatedByDevice = syncRun.CreatedByDevice;
        EventCount = syncRun.EventCount;
    }

    public string Id { get; set; } = null!;
    public SyncRun.SyncRunType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string CreatedByDevice { get; set; } = null!;
    public int EventCount { get; set; }
}
