using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class TierQuotaDefinitionIdTests
{
    [Fact]
    public void Can_create_tier_quota_definition_id_with_valid_value()
    {
        var validTierQuotaDefinitionIdPrefix = "TQD";
        var validIdLengthWithoutPrefix = 17;
        var validIdValue = validTierQuotaDefinitionIdPrefix + TestDataGenerator.GenerateString(validIdLengthWithoutPrefix);

        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(validIdValue);
        var isTierQuotaDefinitionIdValid = tierQuotaDefinitionId.IsSuccess;

        isTierQuotaDefinitionIdValid.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_tier_quota_definition_id()
    {
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Generate();
        tierQuotaDefinitionId.Should().NotBeNull();
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_prefix()
    {
        var invalidTierQuotaDefinitionIdPrefix = "TQQ";
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(invalidTierQuotaDefinitionIdPrefix + TestDataGenerator.GenerateString(17));

        var isTierQuotaDefinitionIdValid = tierQuotaDefinitionId.IsFailure;
        var errorCode = tierQuotaDefinitionId.Error.Code;
        var errorMessage = tierQuotaDefinitionId.Error.Message;

        isTierQuotaDefinitionIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_length()
    {
        var validTierQuotaDefinitionIdPrefix = "TQD";
        var tierQuotaDefinitionIdValue = validTierQuotaDefinitionIdPrefix + TestDataGenerator.GenerateString(TierQuotaDefinitionId.DEFAULT_MAX_LENGTH);
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(tierQuotaDefinitionIdValue);

        var isTierQuotaDefinitionIdValid = tierQuotaDefinitionId.IsFailure;
        var errorCode = tierQuotaDefinitionId.Error.Code;
        var errorMessage = tierQuotaDefinitionId.Error.Message;

        isTierQuotaDefinitionIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id has a length of");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_characters()
    {
        char[] invalidCharacters = { '|', '-', '!' };
        var validTierQuotaDefinitionIdPrefix = "TQD";
        var tierQuotaDefinitionIdValue = TestDataGenerator.GenerateString(TierQuotaDefinitionId.DEFAULT_MAX_LENGTH_WITHOUT_PREFIX, invalidCharacters);
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(validTierQuotaDefinitionIdPrefix + tierQuotaDefinitionIdValue);

        var isTierQuotaDefinitionIdValid = tierQuotaDefinitionId.IsFailure;
        var errorCode = tierQuotaDefinitionId.Error.Code;
        var errorMessage = tierQuotaDefinitionId.Error.Message;

        isTierQuotaDefinitionIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Valid characters are");
    }
}
