using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using Xunit;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentAssertions;
using Backbone.UnitTestTools.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcess;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path_as_owner()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockEventBus = A.Fake<IEventBus>();
        var identity = TestDataGenerator.CreateIdentity();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns(identity);

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(identity.Address);

        var handler = CreateHandler(mockIdentitiesRepository, mockEventBus, fakeUserContext);

        // Act
        await handler.Handle(new StartDeletionProcessCommand(identity.Address), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.DeletionProcesses.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(
                A<IdentityDeletionProcessStartedIntegrationEvent>.That.Matches(e => e.IdentityAddress == identity.Address)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Happy_path_as_support()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockEventBus = A.Fake<IEventBus>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns(identity);

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(null);

        var handler = CreateHandler(mockIdentitiesRepository, mockEventBus, fakeUserContext);

        // Act
        await handler.Handle(new StartDeletionProcessCommand(identity.Address), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.DeletionProcesses.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(
                A<IdentityDeletionProcessStartedIntegrationEvent>.That.Matches(e => e.IdentityAddress == identity.Address)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Identity_can_only_start_deletion_process_for_itself()
    {
        // Arrange
        var fakeUserContext = A.Fake<IUserContext>();
        var handler = CreateHandler(fakeUserContext);

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(IdentityAddress.Create(new byte[] { 2 }, "id1"));

        // Act
        var acting = async () => await handler.Handle(new StartDeletionProcessCommand(IdentityAddress.Create(new byte[] { 1}, "id1")), CancellationToken.None);


        // Assert
        acting.Should().AwaitThrowAsync<ApplicationException>().Which.Code.Should().Be("error.platform.validation.identity.canOnlyStartDeletionProcessForOwnIdentity");
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var address = TestDataGenerator.CreateRandomIdentityAddress();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns((Identity)null);

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(address);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new StartDeletionProcessCommand(address), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IUserContext identitiesRepository)
    {
        return CreateHandler(A.Dummy<IIdentitiesRepository>(), A.Dummy<IEventBus>(), identitiesRepository);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return CreateHandler(identitiesRepository, A.Dummy<IEventBus>(), userContext);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IEventBus eventBus, IUserContext userContext)
    {
        return new Handler(identitiesRepository, eventBus, userContext);
    }
}
