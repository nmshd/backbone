using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.DTOs;

public class SyncRunDTO : IHaveCustomMapping
{
    public enum SyncRunType
    {
        ExternalEventSync,
        DatawalletVersionUpgrade
    }

    public SyncRunId Id { get; set; }
    public SyncRunType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public int EventCount { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<SyncRun, SyncRunDTO>();

        configuration.CreateMap<SyncRun.SyncRunType, SyncRunType>().ConvertUsingEnumMapping(opt => opt
            .MapValue(SyncRun.SyncRunType.DatawalletVersionUpgrade, SyncRunType.DatawalletVersionUpgrade)
            .MapValue(SyncRun.SyncRunType.ExternalEventSync, SyncRunType.ExternalEventSync)
        );
    }
}
