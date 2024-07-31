using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;

public class SyncRunDTO : IHaveCustomMapping
{
    public enum SyncRunType
    {
        ExternalEventSync,
        DatawalletVersionUpgrade
    }

    public required string Id { get; set; }
    public required SyncRunType Type { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public required int EventCount { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<SyncRun, SyncRunDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(t => t.Id.Value))
            .ForMember(dto => dto.CreatedBy, expression => expression.MapFrom(t => t.CreatedBy.Value))
            .ForMember(dto => dto.CreatedByDevice, expression => expression.MapFrom(t => t.CreatedByDevice.Value));

        configuration.CreateMap<SyncRun.SyncRunType, SyncRunType>().ConvertUsingEnumMapping(opt => opt
            .MapValue(SyncRun.SyncRunType.DatawalletVersionUpgrade, SyncRunType.DatawalletVersionUpgrade)
            .MapValue(SyncRun.SyncRunType.ExternalEventSync, SyncRunType.ExternalEventSync)
        );
    }
}
