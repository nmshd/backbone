using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;
public class HandlerTests
{
    private static List<Identity> _identities;

    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(0);

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcessAsOwner(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(30); // Future

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().BeEmpty();
        A.CallTo(() => identitiesRepository.FindAllToBeDeletedWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Response_contains_expected_identities()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1);

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcessAsOwner(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Single().Should().Be(anIdentity.Address);
    }

    [Fact]
    public async Task Handler_calls_DeletionStarted()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1);

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcessAsOwner(new Device(anIdentity).Id); // not called
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().HaveCount(1);
        _identities.First().Status.Should().Be(IdentityStatus.Deleting);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(
            identitiesRepository,
            A.Dummy<ILogger<Handler>>()
            );
    }

    private static IIdentitiesRepository CreateFakeIdentitiesRepository(ushort numberOfIdentities)
    {
        var tierId = TestDataGenerator.CreateRandomTierId();
        _identities = new List<Identity>();

        for (var i = 0; i < numberOfIdentities; i++)
        {
            var identity = TestDataGenerator.CreateIdentityWithTier(tierId);
            _identities.Add(identity);
        }

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllToBeDeletedWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(_identities);

        return identitiesRepository;
    }
}
