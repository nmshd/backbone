using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.DeleteDevice;

public class HandlerTests
{
    [Fact]
    public async Task Deletes_unonboarded_device_owned_by_identity()
    {
        // Arrange
        var startTime = SystemTime.UtcNow;

        var identity = TestDataGenerator.CreateIdentity();
        var unOnboardedDevice = CreateUnOnboardedDevice(identity);
        var onboardedDevice = CreateOnboardedDevice(identity);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(unOnboardedDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(unOnboardedDevice);
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(onboardedDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(onboardedDevice);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(onboardedDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        var deleteDeviceCommand = new DeleteDeviceCommand()
        {
            DeviceId = unOnboardedDevice.Id
        };

        // Act
        await handler.Handle(deleteDeviceCommand, CancellationToken.None);

        // Assert
        unOnboardedDevice.DeletedAt.Should().NotBeNull();
        unOnboardedDevice.DeletedAt.Should().BeAfter(startTime);
        unOnboardedDevice.DeletedByDevice.Should().Be(onboardedDevice.Id);

        A.CallTo(() => mockIdentitiesRepository.Update(
            unOnboardedDevice,
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }



    [Fact]
    public void Deleting_a_device_not_owned_by_the_current_identity_is_not_possible()
    {
        // Arrange
        var identity1 = TestDataGenerator.CreateIdentity();
        var onboardedDevice = CreateOnboardedDevice(identity1);

        var identity2 = TestDataGenerator.CreateIdentity();
        var unOnboardedDevice = CreateUnOnboardedDevice(identity2);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(unOnboardedDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(unOnboardedDevice);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(onboardedDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        var deleteDeviceCommand = new DeleteDeviceCommand()
        {
            DeviceId = unOnboardedDevice.Id
        };

        // Act
        var action = async () => await handler.Handle(deleteDeviceCommand, CancellationToken.None);

        // Assert
        var exception = action.Should().ThrowAsync<NotFoundException>();
        exception.WithMessage(nameof(unOnboardedDevice));
    }

    #region helpers

    private static Device CreateUnOnboardedDevice(Identity identity)
    {
        var unOnboardedDevice = new Device(identity);
        var unOnboardedDeviceUser = new ApplicationUser(identity, unOnboardedDevice.Id);
        unOnboardedDevice.User = unOnboardedDeviceUser;

        return unOnboardedDevice;
    }

    private static Device CreateOnboardedDevice(Identity identity)
    {
        var onboardedDevice = new Device(identity);
        var onboardedDeviceUser = new ApplicationUser(identity, onboardedDevice.Id);
        onboardedDevice.User = onboardedDeviceUser;
        onboardedDevice.User.LoginOccurred();

        return onboardedDevice;
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext fakeUserContext)
    {
        return new Handler(mockIdentitiesRepository, fakeUserContext, A.Dummy<ILogger<Handler>>());
    }

    #endregion
}
