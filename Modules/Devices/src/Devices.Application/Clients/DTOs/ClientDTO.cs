using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;

public class ClientDTO : IHaveCustomMapping
{
    public string ClientId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string DefaultTier { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<OAuthClient, ClientDTO>()
            .ForMember(dto => dto.DefaultTier, expression => expression.MapFrom(m => m.DefaultTier.Value));
    }
}
