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
    public async Task Deletes_device_owned_by_identity()
    {
        // Arrange
        var startTime = SystemTime.UtcNow;

        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(device.Id);

        var deleteDeviceCommand = new DeleteDeviceCommand
        {
            DeviceId = device.Id
        };

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        await handler.Handle(deleteDeviceCommand, CancellationToken.None);

        // Assert
        device.DeletedAt.Should().NotBeNull();
        device.DeletedAt.Should().BeAfter(startTime);
        device.DeletedByDevice.Should().Be(deleteDeviceCommand.DeviceId);

        A.CallTo(() => mockIdentitiesRepository.Update(
            device,
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext fakeUserContext)
    {
        return new Handler(mockIdentitiesRepository, fakeUserContext, A.Dummy<ILogger<Handler>>());
    }
}
