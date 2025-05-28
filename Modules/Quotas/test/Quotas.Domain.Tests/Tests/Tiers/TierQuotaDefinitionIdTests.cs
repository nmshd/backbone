using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Tiers;

public class TierQuotaDefinitionIdTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_tier_quota_definition_id_with_valid_value()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQDsomeTierQuotaId11");

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Can_generate_tier_quota_definition_id()
    {
        // Act
        var tierQuotaDefinitionId = TierQuotaDefinitionId.Generate();

        // Assert
        tierQuotaDefinitionId.ShouldNotBeNull();
        tierQuotaDefinitionId.Value.ShouldHaveCount(20);
        tierQuotaDefinitionId.Value.ShouldStartWith("TQD");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_prefix()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQQsomeTierQuotaId11");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Id starts with");
    }

    [Fact]
    public void Cannot_create_tier_quota_definition_id_with_invalid_id_length()
    {
        // Act
        var result = TierQuotaDefinitionId.Create("TQDtooManyCharactersOnId");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Id has a length of");
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
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Valid characters are");
    }
}
