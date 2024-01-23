using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;

public class ClientDTO : IHaveCustomMapping
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<OAuthClient, ClientDTO>()
            .ForMember(dto => dto.DefaultTier, expression => expression.MapFrom(m => m.DefaultTier.Value));
    }
}
