using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Handler = Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice.Handler;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.UpdateActiveDevice;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Updates_device_with_values_sent()
    {
        // Arrange
        var activeDevice = CreateDevice();

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(activeDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(activeDevice);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        var expectedCommunicationLanguage = CommunicationLanguage.Create("de").Value;
        var updateDeviceCommand = new UpdateActiveDeviceCommand
        {
            CommunicationLanguage = expectedCommunicationLanguage
        };

        // Act
        await handler.Handle(updateDeviceCommand, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(
            A<Device>.That.Matches(d => d.CommunicationLanguage == updateDeviceCommand.CommunicationLanguage),
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Throws_if_given_device_id_does_not_exist()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(A<DeviceId>._, A<CancellationToken>._, A<bool>._)).Returns((Device?)null);

        var handler = CreateHandler(mockIdentitiesRepository);
        var updateDeviceCommand = new UpdateActiveDeviceCommand
        {
            CommunicationLanguage = "de"
        };

        // Act
        var acting = async () => await handler.Handle(updateDeviceCommand, CancellationToken.None);

        // Assert
        acting.Should().ThrowAsync<NotFoundException>().WithMessage(nameof(Device));
    }

    private static Device CreateDevice()
    {
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        return identity.Devices.First();
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext? fakeUserContext = null)
    {
        fakeUserContext ??= A.Dummy<IUserContext>();
        return new Handler(fakeUserContext, A.Dummy<ILogger<Handler>>(), mockIdentitiesRepository);
    }
}
