using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Devices.Commands.UpdateDevice;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
using Handler = Backbone.Modules.Devices.Application.Devices.Commands.UpdateDevice.Handler;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.UpdateDevice;

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
        var updateDeviceCommand = new UpdateDeviceCommand
        {
            CommunicationLanguage = "en"
        };

        // Act
        await handler.Handle(updateDeviceCommand, CancellationToken.None);

        // Assert
        activeDevice.CommunicationLanguage.Should().Be(updateDeviceCommand.CommunicationLanguage);

        A.CallTo(() => mockIdentitiesRepository.Update(
            A<Device>.That.Matches(d => d.CommunicationLanguage == updateDeviceCommand.CommunicationLanguage),
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Throws_when_provided_an_invalid_communication_language()
    {
        // Arrange
        var activeDevice = CreateDevice();

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(activeDevice.Id, A<CancellationToken>._, A<bool>._)).Returns(activeDevice);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);
        var updateDeviceCommand = new UpdateDeviceCommand
        {
            CommunicationLanguage = "some-non-existent-language-code"
        };

        // Act
        var action = async () => await handler.Handle(updateDeviceCommand, CancellationToken.None);

        // Assert
        var exception = action.Should().ThrowAsync<ApplicationException>();
        await exception.WithErrorCode("error.platform.validation.invalidPropertyValue");
    }

    [Fact]
    public void Throws_if_given_device_id_does_not_exist()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.GetDeviceById(A<DeviceId>._, A<CancellationToken>._, A<bool>._)).Returns((Device)null!);

        var handler = CreateHandler(mockIdentitiesRepository);
        var updateDeviceCommand = new UpdateDeviceCommand
        {
            CommunicationLanguage = "en"
        };

        // Act
        var action = async () => await handler.Handle(updateDeviceCommand, CancellationToken.None);

        // Assert
        var exception = action.Should().ThrowAsync<NotFoundException>();
        exception.WithMessage(nameof(Device));
    }

    private static Device CreateDevice(Identity? identity = null)
    {
        identity ??= TestDataGenerator.CreateIdentity();
        var onboardedDevice = new Device(identity);
        return onboardedDevice;
    }

    private static Handler CreateHandler(IIdentitiesRepository mockIdentitiesRepository, IUserContext? fakeUserContext = null)
    {
        fakeUserContext ??= A.Dummy<IUserContext>();
        return new Handler(fakeUserContext, A.Dummy<ILogger<Handler>>(), mockIdentitiesRepository);
    }
}
