using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var mockIdentitiesRepository = CreateFakeIdentitiesRepository(0, out _);

        var handler = CreateHandler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task Response_contains_expected_identities()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcessAsOwner(anIdentity.Devices.First().Id);

        var handler = CreateHandler(identitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.Should().HaveCount(1);
        response.Results.Single().Key.Should().Be(anIdentity.Address);
    }

    [Fact]
    public async Task Deletion_process_started_successfully()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcessAsOwner(anIdentity.Devices.First().Id);

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod + 1));

        var handler = CreateHandler(identitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.Should().HaveCount(1);
        identities.First().Status.Should().Be(IdentityStatus.Deleting);
    }

    [Fact]
    public async Task Publishes_IdentityDeleted_DomainEvent()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcessAsOwner(anIdentity.Devices.First().Id);

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod + 1));

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(identitiesRepository, mockEventBus);

        // Act
        await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<IdentityDeletedDomainEvent>.That.Matches(e =>
            e.IdentityAddress == anIdentity.Address))
        ).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IEventBus? eventBus = null)
    {
        return new Handler(identitiesRepository, eventBus ?? A.Dummy<IEventBus>());
    }

    private static IIdentitiesRepository CreateFakeIdentitiesRepository(ushort numberOfIdentities, out List<Identity> returnedIdentities)
    {
        returnedIdentities = [];

        for (var i = 0; i < numberOfIdentities; i++)
            returnedIdentities.Add(TestDataGenerator.CreateIdentityWithOneDevice());

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._, A<bool>._)).Returns(returnedIdentities);

        return identitiesRepository;
    }
}
