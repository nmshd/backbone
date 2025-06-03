using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;

public class TierNameTests : AbstractTestsBase
{
    [Theory]
    [InlineData("a-tier-name")]
    [InlineData("a tier name")]
    public void Can_create_tier_name_with_valid_value(string value)
    {
        var tierName = TierName.Create(value);
        var isTierNameValid = tierName.IsSuccess;

        isTierNameValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(31)]
    [InlineData(2)]
    public void Cannot_create_tier_name_with_invalid_name_length(int length)
    {
        var invalidTierName = CreateRandomString(length);
        var tierName = TierName.Create(invalidTierName);

        var isTierNameInvalid = tierName.IsFailure;
        var errorCode = tierName.Error.Code;
        var errorMessage = tierName.Error.Message;

        isTierNameInvalid.ShouldBeTrue();
        errorCode.ShouldBe("error.platform.validation.invalidTierName");
        errorMessage.ShouldBe($"Tier Name length must be between {TierName.MIN_LENGTH} and {TierName.MAX_LENGTH}");
    }
}
