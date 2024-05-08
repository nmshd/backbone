using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Challenges.Domain.Entities;

namespace Backbone.Modules.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO : IHaveCustomMapping
{
    public required string Id { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Challenge, ChallengeDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(m => m.Id.Value));
    }
}
