using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcess;
public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(SystemTime.UtcNow.Date);
        var activeDevice = activeIdentity.Devices[0];

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(activeIdentity.Address, CancellationToken.None, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = new Handler(mockIdentitiesRepository, fakeUserContext);
        var command = new CancelDeletionProcessCommand(activeDevice.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var result = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.Cancelled)!;
        result.Status.Should().Be(DeletionProcessStatus.Cancelled);
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeUserContext = A.Fake<IUserContext>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                address,
                CancellationToken.None,
                A<bool>._))
            .Returns<Identity>(null);

        var handler = new Handler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionProcessCommand(address), CancellationToken.None);

        // Assert
        acting.Should().ThrowAsync<NotFoundException>();
    }
}
