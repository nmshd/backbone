using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Updates_the_identity_in_the_database()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(TierName.Create("Old tier").Value);
        var newTier = new Tier(TierName.Create("New Tier").Value);

        var identity = new Identity(CreateRandomDeviceId(), CreateRandomIdentityAddress(), [1, 1, 1, 1, 1], oldTier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        A.CallTo(() => identitiesRepository.Get(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);
        A.CallTo(() => tiersRepository.ListByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier> { oldTier, newTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository);
        var request = BuildRequest(newTier, identity);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => identitiesRepository.Update(
            A<Identity>.That.Matches(i => i.TierId == newTier.Id),
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Fails_when_identity_does_not_exist()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(CreateRandomDeviceId(), CreateRandomIdentityAddress(), [1, 1, 1, 1, 1], oldTier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        A.CallTo(() => tiersRepository.ListByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier> { oldTier, newTier });
        A.CallTo(() => identitiesRepository.Get(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(identitiesRepository, tiersRepository);
        var request = BuildRequest(newTier, identity);

        // Act
        var acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Fails_when_tier_does_not_exist()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldTier = new Tier(TierName.Create("Tier name").Value);
        var newTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(CreateRandomDeviceId(), CreateRandomIdentityAddress(), [1, 1, 1, 1, 1], oldTier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        A.CallTo(() => identitiesRepository.Get(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);
        A.CallTo(() => tiersRepository.ListByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier> { oldTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository);
        var request = BuildRequest(newTier, identity);

        // Act
        var acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Tier");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Does_nothing_when_tiers_are_the_same()
    {
        // Arrange
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var tiersRepository = A.Fake<ITiersRepository>();

        var oldAndNewTier = new Tier(TierName.Create("Tier name").Value);

        var identity = new Identity(CreateRandomDeviceId(), CreateRandomIdentityAddress(), [1, 1, 1, 1, 1], oldAndNewTier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        A.CallTo(() => identitiesRepository.Get(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);
        A.CallTo(() => tiersRepository.ListByIds(A<IEnumerable<TierId>>._, A<CancellationToken>._)).Returns(new List<Tier> { oldAndNewTier });

        var handler = CreateHandler(identitiesRepository, tiersRepository);
        var request = BuildRequest(oldAndNewTier, identity);

        // Act
        var acting = async () => await handler.Handle(request, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<DomainException>();
        A.CallTo(() => identitiesRepository.Update(A<Identity>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    private static UpdateIdentityCommand BuildRequest(Tier newTier, Identity identity)
    {
        return new UpdateIdentityCommand
        {
            Address = identity.Address,
            TierId = newTier.Id.Value
        };
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository)
    {
        return new Handler(identitiesRepository, tiersRepository);
    }
}
