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

        tierQuotaDefinitionId.IsSuccess.Should().BeTrue();
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

        tierQuotaDefinitionId.IsFailure.Should().BeTrue();
        tierQuotaDefinitionId.Error.Code.Should().Be("error.platform.validation.invalidId");
        tierQuotaDefinitionId.Error.Message.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_length()
    {
        var validTierQuotaDefinitionIdPrefix = "TQD";
        var tierQuotaDefinitionIdValue = validTierQuotaDefinitionIdPrefix + TestDataGenerator.GenerateString(TierQuotaDefinitionId.DEFAULT_MAX_LENGTH);
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(tierQuotaDefinitionIdValue);

        tierQuotaDefinitionId.IsFailure.Should().BeTrue();
        tierQuotaDefinitionId.Error.Code.Should().Be("error.platform.validation.invalidId");
        tierQuotaDefinitionId.Error.Message.Should().Contain("Id has a length of");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_characters()
    {
        char[] invalidCharacters = { '|', '-', '!' };
        var validTierQuotaDefinitionIdPrefix = "TQD";
        var tierQuotaDefinitionIdValue = TestDataGenerator.GenerateString(TierQuotaDefinitionId.DEFAULT_MAX_LENGTH_WITHOUT_PREFIX, invalidCharacters);
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Create(validTierQuotaDefinitionIdPrefix + tierQuotaDefinitionIdValue);

        tierQuotaDefinitionId.IsFailure.Should().BeTrue();
        tierQuotaDefinitionId.Error.Code.Should().Be("error.platform.validation.invalidId");
        tierQuotaDefinitionId.Error.Message.Should().Contain("Valid characters are");
    }
}
