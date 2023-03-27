using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Devices.Domain.Tests;
public class TierNameTests
{
    [Fact]
    public void Can_create_tier_name_with_valid_value()
    {
        var tierNameOne = TierName.Create("a-tier-name");
        var isTierNameOneValid = tierNameOne.IsSuccess;

        var tierNameTwo = TierName.Create("a tier name");
        var isTierNameTwoValid = tierNameTwo.IsSuccess;

        isTierNameOneValid.Should().BeTrue();
        isTierNameTwoValid.Should().BeTrue();
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
