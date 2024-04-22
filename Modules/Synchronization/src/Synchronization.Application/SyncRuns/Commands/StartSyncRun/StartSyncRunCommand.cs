using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunCommand : IRequest<StartSyncRunResponse>
{
    public StartSyncRunCommand(SyncRunDTO.SyncRunType type, ushort supportedDatawalletVersion) : this(type, null, supportedDatawalletVersion)
    {
    }

    [JsonConstructor]
    public StartSyncRunCommand(SyncRunDTO.SyncRunType type, ushort? duration, ushort supportedDatawalletVersion)
    {
        Type = type;
        Duration = duration;
        SupportedDatawalletVersion = supportedDatawalletVersion;
    }

    public SyncRunDTO.SyncRunType Type { get; set; }
    public ushort? Duration { get; set; }
    public ushort SupportedDatawalletVersion { get; set; }
}
