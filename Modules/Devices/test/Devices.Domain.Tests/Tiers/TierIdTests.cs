using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Devices.Domain.Tests;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;
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
        const string TIER_ID_WITH_21_CHARACTERS = "TIRxxxxxxxxxxxxxxxxxx";
        var tierId = TierId.Create(TIER_ID_WITH_21_CHARACTERS);

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
        const string TIER_ID_WITH_21_CHARACTERS = "TIRxxxxxxxxxxxxxxxxxx";
        var tierId = TierId.Create(TIER_ID_WITH_21_CHARACTERS);

        var isTierIdInvalid = tierId.IsFailure;
        var errorCode = tierId.Error.Code;
        var errorMessage = tierId.Error.Message;

        isTierIdInvalid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Valid characters are");
    }
}
