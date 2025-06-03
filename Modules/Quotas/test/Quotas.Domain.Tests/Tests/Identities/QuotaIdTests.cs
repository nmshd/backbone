using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;

public class QuotaIdTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_quota_id_with_valid_value()
    {
        // Act
        var result = QuotaId.Create("QUOsomeQuotaId111111");

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Can_generate_quota_id()
    {
        // Act
        var quotaId = QuotaId.Generate();

        // Assert
        quotaId.ShouldNotBeNull();
        quotaId.Value.ShouldHaveCount(20);
        quotaId.Value.ShouldStartWith("QUO");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_prefix()
    {
        // Act
        var result = QuotaId.Create("QQQsomeQuotaId111111");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Id starts with");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_length()
    {
        // Act
        var result = QuotaId.Create("QUOtooManyCharactersOnId");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Id has a length of");
    }

    [Theory]
    [InlineData("|")]
    [InlineData("-")]
    [InlineData("!")]
    public void Cannot_create_quota_id_with_invalid_id_characters(string invalidCharacter)
    {
        // Act
        var result = QuotaId.Create("QUO1111111111111111" + invalidCharacter);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.validation.invalidId");
        result.Error.Message.ShouldContain("Valid characters are");
    }
}
