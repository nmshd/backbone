using Backbone.BuildingBlocks.API.MinimalApi;

namespace Backbone.BuildingBlocks.API.Tests.MinimalApi;

public class Base64QueryValueTests : AbstractTestsBase
{
    [Theory]
    [InlineData("+/8=")]
    [InlineData("-_8=")]
    [InlineData("-_8")]
    public void Standard_and_url_safe_base64_are_decoded(string value)
    {
        Base64QueryValue.TryParse(value, out var result).ShouldBeTrue();
        result.Value.ShouldBe(new byte[] { 251, 255 });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Missing_and_empty_values_remain_null(string? value)
    {
        Base64QueryValue.TryParse(value, out var result).ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData("invalid!")]
    [InlineData("a")]
    public void Invalid_base64_is_a_binding_failure(string value)
    {
        Base64QueryValue.TryParse(value, out _).ShouldBeFalse();
    }
}
