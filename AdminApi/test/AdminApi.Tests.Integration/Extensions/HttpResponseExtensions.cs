using Backbone.AdminApi.Tests.Integration.Models;
using Backbone.AdminApi.Tests.Integration.Support;

namespace Backbone.AdminApi.Tests.Integration.Extensions;

public static class HttpResponseExtensions
{
    public static void AssertHasValue<T>(this HttpResponse<T> response)
    {
        response.Should().NotBeNull();
    }

    public static void AssertStatusCodeIsSuccess<T>(this HttpResponse<T> response)
    {
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public static void AssertContentTypeIs<T>(this HttpResponse<T> response, string contentType)
    {
        response.ContentType.Should().Be(contentType);
    }

    public static void AssertContentCompliesWithSchema<T>(this HttpResponse<T> response)
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T>>(response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
