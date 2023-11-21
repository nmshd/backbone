using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;
public class HandlerTests
{
    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var tierId = TestDataGenerator.CreateRandomTierId();

        var firstIdentity = TestDataGenerator.CreateIdentityWithTier(tierId);
        var secondIdentity = TestDataGenerator.CreateIdentityWithTier(tierId);

        firstIdentity.StartDeletionProcess(new Device(firstIdentity).Id);
        firstIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(30); // Future
        secondIdentity.DeletionGracePeriodEndsAt = null;

        var identities = new List<Identity> { firstIdentity, secondIdentity };

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(identities.Where(i => i.DeletionGracePeriodEndsAt < SystemTime.UtcNow));

        var handler = new Handler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().BeEmpty();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Response_contains_identities_which_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var tierId = TestDataGenerator.CreateRandomTierId();

        var firstIdentity = TestDataGenerator.CreateIdentityWithTier(tierId);
        var secondIdentity = TestDataGenerator.CreateIdentityWithTier(tierId);

        firstIdentity.StartDeletionProcess(new Device(firstIdentity).Id);
        firstIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Future
        secondIdentity.DeletionGracePeriodEndsAt = null;

        var identities = new List<Identity> { firstIdentity, secondIdentity };

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(identities.Where(i => i.DeletionGracePeriodEndsAt < SystemTime.UtcNow));

        var handler = new Handler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().HaveCount(1);
        result.IdentityAddresses.First().Should().Be(firstIdentity.Address);
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }
}
