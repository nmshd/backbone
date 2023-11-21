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
    private static List<Identity> _identities;

    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcess(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(30); // Future

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
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcess(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var handler = new Handler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().HaveCount(1);
        result.IdentityAddresses.First().Should().Be(anIdentity.Address);
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    private static IIdentitiesRepository CreateFakeIdentitiesRepository()
    {
        var tierId = TestDataGenerator.CreateRandomTierId();
        _identities = new List<Identity>();

        for (var i = 0; i < 5; i++)
        {
            var identity = TestDataGenerator.CreateIdentityWithTier(tierId);
            _identities.Add(identity);
        }

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(_identities.Where(i => i.DeletionGracePeriodEndsAt < SystemTime.UtcNow));

        return identitiesRepository;
    }
}
