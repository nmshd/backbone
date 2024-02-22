using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessDuringGracePeriod;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Handler = Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessDuringGracePeriod.Handler;
using static Backbone.UnitTestTools.Data.TestDataGenerator;


namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessDuringGracePeriod;
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

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = new Handler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var command = new CancelDeletionProcessDuringGracePeriodCommand(activeDevice.Id);
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
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity>(null);

        var handler = new Handler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionProcessDuringGracePeriodCommand(address), CancellationToken.None);

        // Assert
        acting.Should().ThrowAsync<NotFoundException>();
    }
}
