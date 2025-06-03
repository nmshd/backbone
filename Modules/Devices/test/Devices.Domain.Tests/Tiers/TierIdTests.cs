using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;

public class TierIdTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_tier_id_with_valid_value()
    {
        const string validTierIdPrefix = "TIR";
        const int validIdLengthWithoutPrefix = 17;
        var validIdValue = validTierIdPrefix + CreateRandomString(validIdLengthWithoutPrefix);

        var tierId = TierId.Create(validIdValue);
        var isTierIdValid = tierId.IsSuccess;

        isTierIdValid.ShouldBeTrue();
    }

    [Fact]
    public void Can_generate_tier_id()
    {
        var tierId = TierId.Generate();
        tierId.ShouldNotBeNull();
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_prefix()
    {
        const string invalidTierIdPrefix = "TIE";
        var tierId = TierId.Create(invalidTierIdPrefix + CreateRandomString(17));

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.ShouldBeTrue();
        errorCode.ShouldBe("error.platform.validation.invalidId");
        errorMessage.ShouldContain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_length()
    {
        const string tierIdWith21Characters = "TIRxxxxxxxxxxxxxxxxxx";
        var tierId = TierId.Create(tierIdWith21Characters);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.ShouldBeTrue();
        errorCode.ShouldBe("error.platform.validation.invalidId");
        errorMessage.ShouldContain("Id has a length of");
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_characters()
    {
        const string tierIdWithInvalidCharacter = "TIRxxxxxxxxxxxxxxxx!";
        var tierId = TierId.Create(tierIdWithInvalidCharacter);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.ShouldBeTrue();
        errorCode.ShouldBe("error.platform.validation.invalidId");
        errorMessage.ShouldContain("Valid characters are");
    }
}
