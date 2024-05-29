using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Domain;

public class CommunicationLanguageTests : AbstractTestsBase
{
    [Theory]
    [InlineData("de")]
    [InlineData("pt")]
    public void Can_create_communication_language_with_valid_value(string value)
    {
        // Arrange & Act
        var communicationLanguage = CommunicationLanguage.Create(value);

        // Assert
        communicationLanguage.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(15)]
    [InlineData(3)]
    public void Cannot_create_communication_language_with_invalid_length(int length)
    {
        // Arrange
        var invalidCommunicationLanguage = TestDataGenerator.GenerateString(length);

        // Act
        var communicationLanguage = CommunicationLanguage.Create(invalidCommunicationLanguage);

        // Assert
        communicationLanguage.IsFailure.Should().BeTrue();
        communicationLanguage.Error.Code.Should().Be("error.platform.validation.invalidDeviceCommunicationLanguage");
        communicationLanguage.Error.Message.Should().Be($"Device Communication Language length must be {CommunicationLanguage.LENGTH}");
    }

    [Theory]
    [InlineData("xz")]
    [InlineData("15")]
    public void Cannot_create_communication_language_with_invalid_iso_code(string value)
    {
        // Arrange & Act
        var communicationLanguage = CommunicationLanguage.Create(value);

        // Assert
        communicationLanguage.IsFailure.Should().BeTrue();
        communicationLanguage.Error.Code.Should().Be("error.platform.validation.invalidDeviceCommunicationLanguage");
        communicationLanguage.Error.Message.Should().Be("Device Communication Language must be a valid two letter ISO code");
    }
}
