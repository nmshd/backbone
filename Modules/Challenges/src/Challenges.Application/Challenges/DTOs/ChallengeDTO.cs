using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Challenges.Application.Challenges.DTOs;

public class ChallengeDTO : IMapTo<Challenge>
{
    public ChallengeId Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
}
