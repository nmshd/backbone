using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcessAsOwner;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();
        var activeDevice = activeIdentity.Devices[0];

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var command = new StartDeletionProcessAsOwnerCommand();
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ApprovedByDevice.Should().NotBeNull();

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(
                    i => i.Address == activeIdentity.Address &&
                         i.DeletionProcesses.Count == 1 &&
                         i.DeletionProcesses[0].Id == response.Id &&
                         i.DeletionProcesses[0].AuditLog.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity?>(null);
        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(address);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var command = new StartDeletionProcessAsOwnerCommand();
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, StartDeletionProcessAsOwnerResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return new Handler(identitiesRepository, userContext, A.Dummy<IEventBus>());
    }
}
