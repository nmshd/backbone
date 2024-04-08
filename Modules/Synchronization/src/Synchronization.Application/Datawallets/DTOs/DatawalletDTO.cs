using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class DatawalletDTO : IHaveCustomMapping
{
    public ushort Version { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Datawallet, DatawalletDTO>()
            .ForMember(dto => dto.Version, expression => expression.MapFrom(m => m.Version.Value));
    }
}
