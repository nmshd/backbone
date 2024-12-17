using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public interface IChallengeFactory
{
    Task Create(CreateChallenges.Command request, DomainIdentity identityWithChallenge);
    int TotalConfiguredChallenges { get; set; }
}
