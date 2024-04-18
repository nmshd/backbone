using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.RejectRelationshipReactivation;
public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreatePendingRelationship(to: activeIdentity);
        relationship.Test_SetStatusAsTerminated(); // todo: Nikola2 - remove when the other thing is implemented

        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = new Handler(fakeRelationshipsRepository, fakeUserContext, A.Fake<IEventBus>());

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.Reactivation,
            RelationshipStatus.Terminated,
            RelationshipStatus.Terminated,
            activeIdentity,
            activeDevice
        );
        relationship.AuditLog.Add(auditLogEntry);

        // Act
        var response = await handler.Handle(new RejectRelationshipReactivationCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNull();
        response.Status.Should().Be(RelationshipStatus.Terminated);
        response.AuditLog.Should().HaveCount(3);
    }

    [Fact]
    public async Task Publishes_RelationshipReactivationCompletedIntegrationEvent()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreatePendingRelationship(to: activeIdentity);
        relationship.Test_SetStatusAsTerminated(); // todo:Nikola2 change this
        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = new Handler(fakeRelationshipsRepository, fakeUserContext, mockEventBus);

        // Act
        await handler.Handle(new RejectRelationshipReactivationCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(
                () => mockEventBus.Publish(A<RelationshipReactivationCompletedIntegrationEvent>.That.Matches(e =>
                    e.RelationshipId.Id == relationship.Id && // todo:Nikola2 recheck this naming and things...
                    e.Peer == relationship.To)
                ))
            .MustHaveHappenedOnceExactly();
    }
}
