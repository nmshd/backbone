﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Handler = Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship.Handler;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.DecomposeRelationship;

public class HandlerTests
{
    [Fact]
    public async Task Returns_the_updated_relationship()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreateTerminatedRelationship(activeIdentity);
        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(fakeRelationshipsRepository, fakeUserContext);

        // Act
        var response = await handler.Handle(new DecomposeRelationshipCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNull();
        response.Status.Should().Be(RelationshipStatus.DeletionProposed);
        response.AuditLog.Should().HaveCount(4);
    }

    [Fact]
    public async Task Saves_the_updated_relationship()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var mockRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreateTerminatedRelationship(from: activeIdentity);
        A.CallTo(() => mockRelationshipsRepository
            .FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var handler = CreateHandler(mockRelationshipsRepository, fakeUserContext);

        // Act
        await handler.Handle(new DecomposeRelationshipCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(
                () => mockRelationshipsRepository.Update(
                    A<Relationship>.That.Matches(r => r.Id == relationship.Id && r.Status == RelationshipStatus.DeletionProposed))
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var fakeRelationshipsRepository = A.Fake<IRelationshipsRepository>();
        var relationship = TestData.CreateTerminatedRelationship(to: activeIdentity);
        A.CallTo(() => fakeRelationshipsRepository.FindRelationship(relationship.Id, activeIdentity, A<CancellationToken>._, true)).Returns(relationship);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeRelationshipsRepository, fakeUserContext, mockEventBus);

        // Act
        await handler.Handle(new DecomposeRelationshipCommand
        {
            RelationshipId = relationship.Id
        }, CancellationToken.None);

        // Assert
        A.CallTo(
                () => mockEventBus.Publish(A<RelationshipStatusChangedDomainEvent>.That.Matches(e =>
                    e.RelationshipId == relationship.Id &&
                    e.Status == RelationshipStatus.DeletionProposed.ToDtoString() &&
                    e.Initiator == activeIdentity &&
                    e.Peer == relationship.From)
                ))
            .MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IRelationshipsRepository relationshipsRepository, IUserContext userContext, IEventBus? eventBus = null)
    {
        eventBus ??= A.Dummy<IEventBus>();
        return new Handler(relationshipsRepository, userContext, eventBus);
    }
}
