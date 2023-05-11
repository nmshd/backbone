using ConsumerApi.Tests.Integration.Models;
using ConsumerApi.Tests.Integration.Support;

namespace ConsumerApi.Tests.Integration.Extensions;

public static class HttpResponseExtensions
{
    public static void AssertResponseHasValue<T>(this HttpResponse<T> response)
    {
        response.Should().NotBeNull();
    }

    public static void AssertStatusCodeIsSuccess<T>(this HttpResponse<T> response)
    {
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public static void AssertResponseContentTypeIs<T>(this HttpResponse<T> response, string contentType)
    {
        response.ContentType.Should().Be(contentType);
    }

    public static void AssertResponseContentCompliesWithSchema<T>(this HttpResponse<T> response)
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T>>(response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
