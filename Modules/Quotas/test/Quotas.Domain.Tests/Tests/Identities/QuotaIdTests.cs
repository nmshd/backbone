using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;
public class QuotaIdTests
{
    [Fact]
    public void Can_create_quota_id_with_valid_value()
    {
        // Act
        var result = QuotaId.Create("QUOsomeQuotaId111111");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_quota_id()
    {
        // Act
        var quotaId = QuotaId.Generate();

        // Assert
        quotaId.Should().NotBeNull();
        quotaId.Value.Should().HaveLength(20);
        quotaId.Value.Should().StartWith("QUO");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_prefix()
    {
        // Act
        var result = QuotaId.Create("QQQsomeQuotaId111111");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_length()
    {
        // Act
        var result = QuotaId.Create("QUOtooManyCharactersOnId");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id has a length of");
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
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Valid characters are");
    }
}
