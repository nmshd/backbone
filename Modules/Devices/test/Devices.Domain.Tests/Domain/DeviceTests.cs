using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        device.CommunicationLanguage.ShouldBe(expectedCommunicationLanguage);
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
        result.ShouldBeTrue();
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
        result.ShouldBeFalse();
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
        isOnboarded.ShouldBeFalse();
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
        isOnboarded.ShouldBeTrue();
    }

    [Fact]
    public void An_unonboarded_device_can_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithoutDevice();

        CreateOnboardedDevice(identity);
        var unOnboardedDevice = CreateUnonboardedDevice(identity);

        // Act
        var acting = () => unOnboardedDevice.EnsureCanBeDeleted(identity.Address);

        // Assert
        acting.ShouldNotThrow();
    }

    [Fact]
    public void An_onboarded_device_cannot_be_deleted()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        CreateOnboardedDevice(identity);
        var onboardedDevice = CreateOnboardedDevice(identity);

        // Act
        var acting = () => onboardedDevice.EnsureCanBeDeleted(identity.Address);

        // Assert
        var domainException = acting.ShouldThrow<DomainException>();
        domainException.Code.ShouldBe("error.platform.validation.device.deviceCannotBeDeleted");
    }

    [Fact]
    public void A_device_not_owned_by_active_identity_cannot_be_deleted()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var otherIdentity = TestDataGenerator.CreateIdentityWithoutDevice();

        CreateOnboardedDevice(activeIdentity);
        var unonboardedDeviceOfOtherIdentity = CreateUnonboardedDevice(otherIdentity);

        // Act
        var action = () => unonboardedDeviceOfOtherIdentity.EnsureCanBeDeleted(activeIdentity.Address);

        // Assert
        var domainException = action.ShouldThrow<DomainException>();
        domainException.Code.ShouldBe("error.platform.validation.device.deviceCannotBeDeleted");
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
        device.IsBackupDevice.ShouldBeFalse();
    }

    [Fact]
    public void When_a_device_logs_in_the_last_login_date_is_set()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        var utcNow = DateTime.UtcNow;
        SystemTime.Set(utcNow);

        // Act
        device.LoginOccurred();

        // Assert
        device.User.LastLoginAt.ShouldBe(utcNow);
    }

    [Fact]
    public void When_a_backup_device_logs_in_a_BackupDeviceUsedDomainEvent_is_raised()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = identity.AddDevice(CommunicationLanguage.DEFAULT_LANGUAGE, identity.Devices.First().Id, true);

        // Act
        device.LoginOccurred();

        // Assert
        var domainEvent = device.ShouldHaveASingleDomainEvent<BackupDeviceUsedDomainEvent>();
        domainEvent.IdentityAddress.ShouldBe(identity.Address);
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
