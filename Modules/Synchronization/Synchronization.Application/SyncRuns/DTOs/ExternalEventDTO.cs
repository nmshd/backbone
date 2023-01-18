using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.DTOs;

public class ExternalEventDTO : IHaveCustomMapping
{
    public ExternalEventId Id { get; set; }
    public string Type { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte SyncErrorCount { get; set; }
    public object Payload { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<ExternalEventType, string>().ConvertUsing((externalEventType, _) =>
        {
            return externalEventType switch
            {
                ExternalEventType.MessageDelivered => "MessageDelivered",
                ExternalEventType.MessageReceived => "MessageReceived",
                ExternalEventType.RelationshipChangeCreated => "RelationshipChangeCreated",
                ExternalEventType.RelationshipChangeCompleted => "RelationshipChangeCompleted",
                _ => throw new ArgumentOutOfRangeException(nameof(externalEventType), externalEventType, null)
            };
        });
        configuration.CreateMap<ExternalEvent, ExternalEventDTO>();
    }
}
