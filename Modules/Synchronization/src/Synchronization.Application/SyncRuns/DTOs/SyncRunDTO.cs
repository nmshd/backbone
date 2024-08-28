using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

public class SyncRunDTO
{
    public enum SyncRunType
    {
        ExternalEventSync,
        DatawalletVersionUpgrade
    }

    public SyncRunDTO()
    {
    }

    public SyncRunDTO(SyncRun syncRun)
    {
        Id = syncRun.Id;
        Type = MapSyncRunType(syncRun.Type);
        ExpiresAt = syncRun.ExpiresAt;
        Index = syncRun.Index;
        CreatedAt = syncRun.CreatedAt;
        CreatedBy = syncRun.CreatedBy;
        CreatedByDevice = syncRun.CreatedByDevice;
        EventCount = syncRun.EventCount;
    }

    public string Id { get; set; } = null!;
    public SyncRunType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string CreatedByDevice { get; set; } = null!;
    public int EventCount { get; set; }

    private SyncRunType MapSyncRunType(SyncRun.SyncRunType type)
    {
        return type switch
        {
            SyncRun.SyncRunType.DatawalletVersionUpgrade => SyncRunType.DatawalletVersionUpgrade,
            SyncRun.SyncRunType.ExternalEventSync => SyncRunType.ExternalEventSync,
            _ => throw new Exception($"Unsupported Sync Run Type: {type}")
        };
    }
}
