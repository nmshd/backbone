using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
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
        var mockIdentitiesRepository = CreateFakeIdentitiesRepository(0);

        var handler = CreateHandler(mockIdentitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DeletedIdentityAddresses.Should().BeEmpty();
        A.CallTo(() => mockIdentitiesRepository.FindAllToBeDeletedWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Response_contains_expected_identities()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1);

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcessAsOwner(new Device(anIdentity).Id);

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DeletedIdentityAddresses.Should().HaveCount(1);
        result.DeletedIdentityAddresses.Single().Should().Be(anIdentity.Address);
    }

    [Fact]
    public async Task Deletion_process_started_successfully()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1);

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcessAsOwner(DeviceId.New()); // not called

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DeletedIdentityAddresses.Should().HaveCount(1);
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
        _identities = new List<Identity>();

        for (var i = 0; i < numberOfIdentities; i++)
            _identities.Add(TestDataGenerator.CreateIdentity());

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllToBeDeletedWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(_identities);

        return identitiesRepository;
    }
}
