using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Amqp.Framing;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsOwner;
public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var activeDevice = activeIdentity.Devices[0];
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(activeIdentity.Address, CancellationToken.None, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);
        var command = new CancelDeletionProcessAsOwnerCommand(activeIdentity.Address, deletionProcess.Id);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == activeIdentity.Address
                && i.Status == IdentityStatus.Active
                && i.DeletionProcesses[0].Status == DeletionProcessStatus.Cancelled
                && i.DeletionProcesses[0].Id == deletionProcess.Id), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        response.Status.Should().Be(DeletionProcessStatus.Cancelled);
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var randomAddress = CreateRandomIdentityAddress();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(randomAddress, CancellationToken.None, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository);

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionProcessAsOwnerCommand(randomAddress, deletionProcess.Id), CancellationToken.None);

        // Assert
        acting.Should().ThrowAsync<NotFoundException>();
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IUserContext? userContext = null)
    {
        userContext ??= A.Fake<IUserContext>();
        identitiesRepository ??= A.Fake<IIdentitiesRepository>();

        return new Handler(identitiesRepository, userContext);
    }
}
