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

        var deleteUnOnboardedDeviceCommand = new DeleteDeviceCommand()
        {
            DeviceId = unOnboardedDevice.Id
        };

        // Act
        await handler.Handle(deleteUnOnboardedDeviceCommand, CancellationToken.None);

        // Assert
        unOnboardedDevice.DeletedAt.Should().NotBeNull();
        unOnboardedDevice.DeletedAt.Should().BeAfter(startTime);
        unOnboardedDevice.DeletedByDevice.Should().Be(onboardedDevice.Id);

        A.CallTo(() => mockIdentitiesRepository.Update(
            unOnboardedDevice,
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
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

public class IdentityWithOnboardedAndUnonboardedDevicesSetup1
{
    public readonly Identity Identity;
    public readonly Device UnOnboardedDevice;
    public readonly Device OnboardedDevice;
    public readonly IIdentitiesRepository MockIdentitiesRepository;
    public readonly IUserContext FakeUserContext;
    public readonly Handler Handler;

    public IdentityWithOnboardedAndUnonboardedDevicesSetup1()
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
