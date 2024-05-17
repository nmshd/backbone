﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsSupport;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(utcNow);
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => fakeIdentitiesRepository
                .FindByAddress(identity.Address, A<CancellationToken>._, true))
            .Returns(identity);

        var handler = CreateHandler(fakeIdentitiesRepository, mockPushNotificationSender);

        // Acting
        var result = await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address, deletionProcess.Id), CancellationToken.None);

        // Assert
        identity.Status.Should().Be(IdentityStatus.Active);

        result.Id.Should().Be(deletionProcess.Id);
        result.Status.Should().Be(DeletionProcessStatus.Cancelled);
        result.CancelledAt.Should().Be(utcNow);

        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address, A<DeletionProcessCancelledBySupportNotification>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_domain_events()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(DateTime.Parse("2000-01-01"));
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository
                .FindByAddress(identity.Address, A<CancellationToken>._, true))
            .Returns(identity);

        var mockEventBus = A.Fake<IEventBus>();

        var handler = CreateHandler(fakeIdentitiesRepository, mockEventBus);

        // Act
        var response = await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address, deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<TierOfIdentityChangedDomainEvent>.That.Matches(e =>
                e.IdentityAddress == identity.Address &&
                e.OldTierId == "TIR00000000000000001"))
        ).MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(
            A<IdentityDeletionProcessStatusChangedDomainEvent>.That.Matches(e =>
                e.Address == identity.Address &&
                e.DeletionProcessId == response.Id))
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var deletionProcessId = IdentityDeletionProcessId.Generate();
        var handler = CreateHandler();

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionAsSupportCommand(identity.Address, deletionProcessId), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, CancelDeletionAsSupportResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        return CreateHandler(identitiesRepository, null, pushNotificationSender);
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IEventBus? eventBus = null, IPushNotificationSender? pushNotificationSender = null)
    {
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        eventBus ??= A.Dummy<IEventBus>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();

        return new Handler(identitiesRepository, eventBus, pushNotificationSender);
    }
}
