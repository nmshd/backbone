using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;


namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateChallengesTests : SnapshotCreatorTestsBase
{
    private readonly IChallengeFactory _challengeFactory;
    private readonly CreateChallenges.CommandHandler _handler;

    public CreateChallengesTests()
    {
        _challengeFactory = A.Fake<IChallengeFactory>();
        _handler = new CreateChallenges.CommandHandler(_challengeFactory);
    }

    [Fact]
    public async Task Handle_ShouldSetTotalChallenges()
    {
        // Arrange
        var identities = new List<DomainIdentity>
        {
            new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 0, 0),
            new(null!, null, 0, 0, 0, IdentityPoolType.App, 3, "", 0, 0),
            new(null!, null, 0, 0, 0, IdentityPoolType.Connector, 3, "", 0, 0)
        };

        var expectedTotalChallenges = identities.Sum(i => i.NumberOfChallenges);
        var command = new CreateChallenges.Command(identities, "http://baseurl", new ClientCredentials("clientId", "clientSecret"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallToSet(() => _challengeFactory.TotalChallenges).To(expectedTotalChallenges).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_ShouldCallCreateForEachIdentityWithChallenges()
    {
        // Arrange
        var identities = new List<DomainIdentity>
        {
            new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 0, 0),
            new(null!, null, 0, 0, 0, IdentityPoolType.App, 3, "", 0, 0),
            new(null!, null, 0, 0, 0, IdentityPoolType.Connector, 1, "", 0, 0)
        };
        var command = new CreateChallenges.Command(identities, "http://baseurl", new ClientCredentials("clientId", "clientSecret"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _challengeFactory.Create(command, A<DomainIdentity>.That.Matches(i => i.NumberOfChallenges == 5))).MustHaveHappenedOnceExactly();
        A.CallTo(() => _challengeFactory.Create(command, A<DomainIdentity>.That.Matches(i => i.NumberOfChallenges == 3))).MustHaveHappenedOnceExactly();
        A.CallTo(() => _challengeFactory.Create(command, A<DomainIdentity>.That.Matches(i => i.NumberOfChallenges == 1))).MustHaveHappenedOnceExactly();
    }
}
