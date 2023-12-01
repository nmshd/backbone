using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
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
    public async Task Deletes_un_onboarded_device_owned_by_identity()
    {
        // Arrange
        var startTime = SystemTime.UtcNow;
        var deleteDeviceTestSetup = new IdentityWithOnboardedAndUnonboardedDevicesSetup();

        var deleteUnOnboardedDeviceCommand = new DeleteDeviceCommand()
        {
            DeviceId = deleteDeviceTestSetup.UnOnboardedDevice.Id
        };

        // Act
        await deleteDeviceTestSetup.Handler.Handle(deleteUnOnboardedDeviceCommand, CancellationToken.None);

        // Assert
        deleteDeviceTestSetup.UnOnboardedDevice.DeletedAt.Should().NotBeNull();
        deleteDeviceTestSetup.UnOnboardedDevice.DeletedAt.Should().BeAfter(startTime);
        deleteDeviceTestSetup.UnOnboardedDevice.DeletedByDevice.Should().Be(deleteDeviceTestSetup.OnboardedDevice.Id);

        A.CallTo(() => deleteDeviceTestSetup.MockIdentitiesRepository.Update(
            deleteDeviceTestSetup.UnOnboardedDevice,
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }
}

public class IdentityWithOnboardedAndUnonboardedDevicesSetup
{
    public readonly Identity Identity;
    public readonly Device UnOnboardedDevice;
    public readonly Device OnboardedDevice;
    public readonly IIdentitiesRepository MockIdentitiesRepository;
    public readonly IUserContext FakeUserContext;
    public readonly Handler Handler;

    public IdentityWithOnboardedAndUnonboardedDevicesSetup()
    {
        Identity = TestDataGenerator.CreateIdentity();
        UnOnboardedDevice = CreateUnOnboardedDevice(Identity);
        OnboardedDevice = CreateOnboardedDevice(Identity);

        MockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => MockIdentitiesRepository.GetDeviceById(UnOnboardedDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(UnOnboardedDevice);
        A.CallTo(() => MockIdentitiesRepository.GetDeviceById(OnboardedDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(OnboardedDevice);

        FakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => FakeUserContext.GetAddress()).Returns(Identity.Address);
        A.CallTo(() => FakeUserContext.GetDeviceId()).Returns(OnboardedDevice.Id);

        Handler = CreateHandler(MockIdentitiesRepository, FakeUserContext);
    }

    private static Device CreateUnOnboardedDevice(Identity identity)
    {
        var unOnboardedDevice = new Device(identity);
        var unOnboardedDeviceUser = new ApplicationUser(identity, unOnboardedDevice.Id)
        {
            LastLoginAt = null
        };
        unOnboardedDevice.User = unOnboardedDeviceUser;

        return unOnboardedDevice;
    }

    private static Device CreateOnboardedDevice(Identity identity)
    {
        var onboardedDevice = new Device(identity);
        var onboardedDeviceUser = new ApplicationUser(identity, onboardedDevice.Id)
        {
            LastLoginAt = SystemTime.UtcNow
        };
        onboardedDevice.User = onboardedDeviceUser;

        return onboardedDevice;
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext fakeUserContext)
    {
        return new Handler(mockIdentitiesRepository, fakeUserContext, A.Dummy<ILogger<Handler>>());
    }
}
