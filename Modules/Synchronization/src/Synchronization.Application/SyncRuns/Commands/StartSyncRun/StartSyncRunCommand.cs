using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunCommand : IRequest<StartSyncRunResponse>
{
    public StartSyncRunCommand(SyncRun.SyncRunType type, ushort supportedDatawalletVersion) : this(type, null, supportedDatawalletVersion)
    {
    }

    [JsonConstructor]
    public StartSyncRunCommand(SyncRun.SyncRunType type, ushort? duration, ushort supportedDatawalletVersion)
    {
        Type = type;
        Duration = duration;
        SupportedDatawalletVersion = supportedDatawalletVersion;
    }

    public SyncRun.SyncRunType Type { get; set; }
    public ushort? Duration { get; set; }
    public ushort SupportedDatawalletVersion { get; set; }
}
