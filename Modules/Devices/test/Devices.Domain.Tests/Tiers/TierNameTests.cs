using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;
public class TierNameTests
{
    [Theory]
    [InlineData("a-tier-name")]
    [InlineData("a tier name")]
    public void Can_create_tier_name_with_valid_value(string value)
    {
        var tierName = TierName.Create(value);
        var isTierNameValid = tierName.IsSuccess;

        isTierNameValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(31)]
    [InlineData(2)]
    public void Cannot_create_tier_name_with_invalid_name_length(int length)
    {
        var invalidTierName = TestDataGenerator.GenerateString(length);
        var tierName = TierName.Create(invalidTierName);

        var isTierNameInvalid = tierName.IsFailure;
        var errorCode = tierName.Error.Code;
        var errorMessage = tierName.Error.Message;

        isTierNameInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidTierName");
        errorMessage.Should().Be($"Tier Name length must be between {TierName.MIN_LENGTH} and {TierName.MAX_LENGTH}");
    }
}
