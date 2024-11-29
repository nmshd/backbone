using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class DeviceTests : AbstractTestsBase
{
    [Fact]
    public void Updates_communication_language()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        var expectedCommunicationLanguage = CommunicationLanguage.Create("de").Value;

        // Act
        device.Update(expectedCommunicationLanguage);

        // Assert
        device.CommunicationLanguage.Should().Be(expectedCommunicationLanguage);
    }

    [Fact]
    public void Update_returns_true_if_communication_language_is_different()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        // Act
        var result = device.Update(CommunicationLanguage.Create("de").Value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Update_returns_false_if_communication_language_is_the_same()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        // Act
        var result = device.Update(CommunicationLanguage.Create("en").Value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOnboarded_returns_false_if_user_has_never_logged_in_before()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        // Act
        var isOnboarded = device.IsOnboarded;

        // Assert
        isOnboarded.Should().BeFalse();
    }

    [Fact]
    public void IsOnboarded_returns_true_if_user_has_been_used_to_login()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        device.LoginOccurred();

        // Act
        var isOnboarded = device.IsOnboarded;

        // Assert
        isOnboarded.Should().BeTrue();
    }

    [Fact]
    public void An_unOnboarded_device_can_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithoutDevice();

        var activeDevice = CreateOnboardedDevice(identity);
        var unOnboardedDevice = CreateUnonboardedDevice(identity);

        // Act
        unOnboardedDevice.MarkAsDeleted(activeDevice.Id, identity.Address);

        // Assert
        unOnboardedDevice.DeletedAt.Should().NotBeNull();
        unOnboardedDevice.DeletedByDevice.Should().Be(activeDevice.Id);
    }

    [Fact]
    public void An_onboarded_device_cannot_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        var activeDevice = CreateOnboardedDevice(identity);
        var onboardedDevice = CreateOnboardedDevice(identity);

        // Act
        var action = () => onboardedDevice.MarkAsDeleted(activeDevice.Id, identity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void A_device_not_owned_by_active_identity_cannot_be_deleted()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var otherIdentity = TestDataGenerator.CreateIdentityWithoutDevice();

        var activeDevice = CreateOnboardedDevice(activeIdentity);
        var unOnboardedDeviceOfOtherIdentity = CreateUnonboardedDevice(otherIdentity);

        // Act
        var action = () => unOnboardedDeviceOfOtherIdentity.MarkAsDeleted(activeDevice.Id, activeIdentity.Address);

        // Assert
        var domainException = action.Should().Throw<DomainException>().Which;
        domainException.Code.Should().Be("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void A_backup_device_can_be_marked_as_used()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var device = CreateBackupDevice(activeIdentity);

        // Act
        device.LoginOccurred();

        // Assert
        device.IsBackupDevice.Should().BeFalse();
    }

    [Fact]
    public void Login_occurred()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        // Act
        device.LoginOccurred();

        // Assert
        device.User.HasLoggedIn.Should().BeTrue();
    }

    [Fact]
    public void Login_occurred_creates_a_BackupDeviceUsedDomainEvent_if_logged_in_device_is_backup_device()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE, isBackupDevice: true);

        // Act
        device.LoginOccurred();

        // Assert
        device.Should().HaveASingleDomainEvent<BackupDeviceUsedDomainEvent>().Should().NotBeNull();
    }

    private static Device CreateUnonboardedDevice(Identity identity)
    {
        return identity.AddDevice(CommunicationLanguage.DEFAULT_LANGUAGE, identity.Devices.First().Id, false);
    }

    private static Device CreateOnboardedDevice(Identity identity)
    {
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        device.LoginOccurred();
        return device;
    }

    private static Device CreateBackupDevice(Identity identity)
    {
        return new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE, null, true);
    }
}
