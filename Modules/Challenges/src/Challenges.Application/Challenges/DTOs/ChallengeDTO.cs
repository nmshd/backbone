using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Challenges.Domain.Entities;

namespace Backbone.Modules.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO : IMapTo<Challenge>
{
    public required string Id { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
