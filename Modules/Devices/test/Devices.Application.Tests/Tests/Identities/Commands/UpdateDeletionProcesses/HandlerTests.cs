using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;

public class HandlerTests
{
    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var mockIdentitiesRepository = CreateFakeIdentitiesRepository(0, out _);

        var handler = CreateHandler(mockIdentitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DeletedIdentityAddresses.Should().BeEmpty();
    }

    [Fact]
    public async Task Response_contains_expected_identities()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcessAsOwner(anIdentity.Devices.First().Id);

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
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcessAsOwner(anIdentity.Devices.First().Id);

        var handler = CreateHandler(identitiesRepository);
        var command = new TriggerRipeDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DeletedIdentityAddresses.Should().HaveCount(1);
        identities.First().Status.Should().Be(IdentityStatus.Deleting);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
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
