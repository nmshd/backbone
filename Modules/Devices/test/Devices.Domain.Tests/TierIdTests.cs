using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Devices.Domain.Tests;
public class TierIdTests
{
    [Fact]
    public void Can_create_tier_id_with_valid_value()
    {
        var validTierIdPrefix = "TIR";
        var validIdLengthWithoutPrefix = 17;
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
        var invalidTierIdPrefix = "TIE";
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
        var validTierIdPrefix = "TIR";
        var tierIdValue = validTierIdPrefix + TestDataGenerator.GenerateString(TierId.DEFAULT_MAX_LENGTH);
        var tierId = TierId.Create(tierIdValue);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Be("Id has a length of");
    }

    [Fact]
    public void Cannot_create_tier_id_with_invalid_id_characters()
    {
        char[] invalidCharacters = { '|', '-', '!' };
        var validTierIdPrefix = "TIR";
        var tierIdValue = TestDataGenerator.GenerateString(TierId.DEFAULT_MAX_LENGTH_WITHOUT_PREFIX, invalidCharacters);
        var tierId = TierId.Create(validTierIdPrefix + tierIdValue);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Valid characters are");
    }
}
