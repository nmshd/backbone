using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;
public class TierIdTests
{
    [Fact]
    public void Can_create_tier_id_with_valid_value()
    {
        const string validTierIdPrefix = "TIR";
        const int validIdLengthWithoutPrefix = 17;
        var validIdValue = validTierIdPrefix + TestDataGenerator.GenerateString(validIdLengthWithoutPrefix);

        var tierId = TierId.Create(validIdValue);
        var isTierIdValid = tierId.IsSuccess;

        isTierIdValid.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_tier_id()
    {
        var tierId = TierId.Generate();
        tierId.Should().NotBeNull();
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_prefix()
    {
        const string invalidTierIdPrefix = "TIE";
        var tierId = TierId.Create(invalidTierIdPrefix + TestDataGenerator.GenerateString(17));

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_length()
    {
        const string tierIdWith21Characters = "TIRxxxxxxxxxxxxxxxxxx";
        var tierId = TierId.Create(tierIdWith21Characters);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id has a length of");
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_characters()
    {
        const string tierIdWithInvalidCharacter = "TIRxxxxxxxxxxxxxxxx!";
        var tierId = TierId.Create(tierIdWithInvalidCharacter);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Valid characters are");
    }
}
