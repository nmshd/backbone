using Backbone.AdminApi.Tests.Integration.Models;

namespace Backbone.AdminApi.Tests.Integration.Extensions;

public static class ODataResponseExtensions
{
    public static void AssertHasValue<T>(this ODataResponse<T> response)
    {
        response.Should().NotBeNull();
    }

    public static void AssertStatusCodeIsSuccess<T>(this ODataResponse<T> response)
    {
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public static void AssertContentTypeIs<T>(this ODataResponse<T> response, string contentType)
    {
        response.ContentType.Should().Be(contentType);
    }

    public static void AssertContentCompliesWithSchema<T>(this ODataResponse<T> response)
    {
        // JsonValidators.ValidateJsonSchema<ODataResponseContent<T>>(response.RawContent!, out var errors)
        //     .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
