using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.DeleteDevice;

public class HandlerTests
{
    [Fact]
    public async Task Deletes_unOnboarded_device_owned_by_identity()
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
    public void Throws_if_given_device_id_does_not_exist()
    {
        // Arrange
        var activeDevice = CreateOnboardedDevice();
        const string nonExistentDeviceId = "DVCnonExistingIdxxxx";

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(activeDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(activeDevice);

        var handler = CreateHandler(mockIdentitiesRepository);

        var deleteDeviceCommand = new DeleteDeviceCommand()
        {
            DeviceId = nonExistentDeviceId
        };

        // Act
        var action = async () => await handler.Handle(deleteDeviceCommand, CancellationToken.None);

        // Assert
        var exception = action.Should().ThrowAsync<NotFoundException>();
        exception.WithMessage(nameof(Device));
    }

    #region helpers

    private static Device CreateUnOnboardedDevice(Identity identity)
    {
        var unOnboardedDevice = new Device(identity);
        var unOnboardedDeviceUser = new ApplicationUser(identity, unOnboardedDevice.Id);
        unOnboardedDevice.User = unOnboardedDeviceUser;

        return unOnboardedDevice;
    }

    private static Device CreateOnboardedDevice(Identity identity = null)
    {
        identity ??= TestDataGenerator.CreateIdentity();
        var onboardedDevice = new Device(identity);
        var onboardedDeviceUser = new ApplicationUser(identity, onboardedDevice.Id);
        onboardedDevice.User = onboardedDeviceUser;
        onboardedDevice.User.LoginOccurred();

        return onboardedDevice;
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext? fakeUserContext = null)
    {
        fakeUserContext ??= A.Dummy<IUserContext>();
        return new Handler(mockIdentitiesRepository, fakeUserContext, A.Dummy<ILogger<Handler>>());
    }

    #endregion
}
