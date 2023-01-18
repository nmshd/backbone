using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Synchronization.Domain.Entities;

namespace Synchronization.Application.Datawallets.DTOs;

public class DatawalletDTO : IHaveCustomMapping
{
    public ushort Version { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Datawallet, DatawalletDTO>()
            .ForMember(dto => dto.Version, expression => expression.MapFrom(m => m.Version.Value));
    }
}
