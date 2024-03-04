using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Tiers;

public class TierQuotaDefinitionIdTests
{
    [Fact]
    public void Can_create_tier_quota_definition_id_with_valid_value()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQDsomeTierQuotaId11");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_tier_quota_definition_id()
    {
        // Act
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Generate();

        // Assert
        tierQuotaDefinitionId.Should().NotBeNull();
        tierQuotaDefinitionId.Value.Should().HaveLength(20);
        tierQuotaDefinitionId.Value.Should().StartWith("TQD");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_prefix()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQQsomeTierQuotaId11");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_length()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQDtooManyCharactersOnId");

        // Assert
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
        // Act
        var result = TierQuotaDefinitionId.Create("TQD1111111111111111" + invalidCharacter);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Valid characters are");
    }
}
