using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenResponse : IHaveCustomMapping
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Token, CreateTokenResponse>()
            .ForMember(createTokenResponse => createTokenResponse.Id, expression => expression.MapFrom(m => m.Id.Value));
    }
}
