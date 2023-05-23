using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class QuotaIdTests
{
    [Fact]
    public void Can_create_quota_id_with_valid_value()
    {
        var result = QuotaId.Create("QUOsomeQuotaId111111");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_quota_id()
    {
        var quotaId = QuotaId.Generate();
        quotaId.Should().NotBeNull();
        quotaId.Value.Should().HaveLength(20);
        quotaId.Value.Should().StartWith("QUO");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_prefix()
    {
        var result = QuotaId.Create("QQQsomeQuotaId111111");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_length()
    {
        var result = QuotaId.Create("QUOtooManyCharactersOnId");

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
        var result = QuotaId.Create("QUO1111111111111111" + invalidCharacter);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.validation.invalidId");
        result.Error.Message.Should().Contain("Valid characters are");
    }
}