using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Devices.Domain.Tests;
public class TierNameTests
{
    [Fact]
    public void Can_create_tier_name_with_valid_value()
    {
        var validLength = 30;
        var validTierName = TestDataGenerator.GenerateString(validLength);
        var tierName = TierName.Create(validTierName);

        var isTierNameValid = tierName.IsSuccess;

        isTierNameValid.Should().BeTrue();
    }

    [Fact]
    public void Cannot_create_tier_name_with_invalid_name_length()
    {
        var invalidLength = 31;
        var invalidTierName = TestDataGenerator.GenerateString(invalidLength);
        var tierName = TierName.Create(invalidTierName);

        var isTierNameInvalid = tierName.IsFailure;
        var errorCode = tierName.Error.Code;
        var errorMessage = tierName.Error.Message;

        isTierNameInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidTierName");
        errorMessage.Should().Be("Tier Name is longer than the 30 characters");
    }
}
