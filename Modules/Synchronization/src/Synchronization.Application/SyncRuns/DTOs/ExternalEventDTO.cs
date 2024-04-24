using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

public class ExternalEventDTO : IHaveCustomMapping
{
    public required ExternalEventId Id { get; set; }
    public required string Type { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte SyncErrorCount { get; set; }
    public required object Payload { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<ExternalEventType, string>().ConvertUsing((externalEventType, _) => externalEventType switch
        {
            ExternalEventType.MessageDelivered => "MessageDelivered",
            ExternalEventType.MessageReceived => "MessageReceived",
            ExternalEventType.RelationshipStatusChanged => "RelationshipStatusChanged",
            _ => throw new ArgumentOutOfRangeException(nameof(externalEventType), externalEventType, null)
        });
        configuration.CreateMap<ExternalEvent, ExternalEventDTO>();
    }
}
