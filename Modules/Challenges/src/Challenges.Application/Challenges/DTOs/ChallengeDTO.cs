using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Challenges.Domain.Entities;
using Backbone.Challenges.Domain.Ids;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO : IMapTo<Challenge>
{
    public ChallengeId Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
}
