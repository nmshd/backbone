using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class QuotaIdTests
{
    [Fact]
    public void Can_create_quota_id_with_valid_value()
    {
        var validQuotaIdPrefix = "QUO";
        var validIdLengthWithoutPrefix = 17;
        var validIdValue = validQuotaIdPrefix + TestDataGenerator.GenerateString(validIdLengthWithoutPrefix);

        var quotaId = QuotaId.Create(validIdValue);
        var isQuotaIdValid = quotaId.IsSuccess;

        isQuotaIdValid.Should().BeTrue();
    }

    [Fact]
    public void Can_generate_quota_id()
    {
        var quotaId = QuotaId.Generate();
        quotaId.Should().NotBeNull();
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_prefix()
    {
        var invalidQuotaIdPrefix = "QQQ";
        var quotaId = QuotaId.Create(invalidQuotaIdPrefix + TestDataGenerator.GenerateString(17));

        var isQuotaIdValid = quotaId.IsFailure;
        var errorCode = quotaId.Error.Code;
        var errorMessage = quotaId.Error.Message;

        isQuotaIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id starts with");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_length()
    {
        var validQuotaIdPrefix = "QUO";
        var quotaIdValue = validQuotaIdPrefix + TestDataGenerator.GenerateString(QuotaId.DEFAULT_MAX_LENGTH);
        var quotaId = QuotaId.Create(quotaIdValue);

        var isQuotaIdValid = quotaId.IsFailure;
        var errorCode = quotaId.Error.Code;
        var errorMessage = quotaId.Error.Message;

        isQuotaIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Id has a length of");
    }

    [Fact]
    public void Cannot_create_quota_id_with_invalid_id_characters()
    {
        char[] invalidCharacters = { '|', '-', '!' };
        var validQuotaIdPrefix = "QUO";
        var quotaIdValue = TestDataGenerator.GenerateString(QuotaId.DEFAULT_MAX_LENGTH_WITHOUT_PREFIX, invalidCharacters);
        var quotaId = QuotaId.Create(validQuotaIdPrefix + quotaIdValue);

        var isQuotaIdValid = quotaId.IsFailure;
        var errorCode = quotaId.Error.Code;
        var errorMessage = quotaId.Error.Message;

        isQuotaIdValid.Should().BeTrue();
        errorCode.Should().Be("error.platform.validation.invalidId");
        errorMessage.Should().Contain("Valid characters are");
    }
}