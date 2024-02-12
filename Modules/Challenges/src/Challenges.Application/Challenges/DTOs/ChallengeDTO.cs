using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;

namespace Backbone.Modules.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO : IMapTo<Challenge>
{
    public required ChallengeId Id { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public IdentityAddress? CreatedBy { get; set; }
    public DeviceId? CreatedByDevice { get; set; }
}
