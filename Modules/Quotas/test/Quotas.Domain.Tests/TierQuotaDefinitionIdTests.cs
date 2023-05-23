using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierQuotaDefinitionIdTests
{
    [Fact]
    public void Can_create_tier_quota_definition_id_with_valid_value()
    {
        var result = TierQuotaDefinitionId.Create("TQDsomeTierQuotaId11");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_tier_quota_definition_id()
    {
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Generate();

        tierQuotaDefinitionId.Should().NotBeNull();
        tierQuotaDefinitionId.Value.Should().HaveLength(20);
        tierQuotaDefinitionId.Value.Should().StartWith("TQD");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_prefix()
    {
        var result = TierQuotaDefinitionId.Create("TQQsomeTierQuotaId11");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_length()
    {
        var result = TierQuotaDefinitionId.Create("TQDtooManyCharactersOnId");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id has a length of");
    }

    [Theory]
    [InlineData("|")]
    [InlineData("-")]
    [InlineData("!")]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_characters(string invalidCharacter)
    {
        var result = TierQuotaDefinitionId.Create("TQD1111111111111111" + invalidCharacter);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Valid characters are");
    }
}
