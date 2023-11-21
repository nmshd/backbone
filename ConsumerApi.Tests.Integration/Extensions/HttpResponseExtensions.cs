using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.ConsumerApi.Tests.Integration.Support;

namespace Backbone.ConsumerApi.Tests.Integration.Extensions;

public static class HttpResponseExtensions
{
    public static void AssertContentCompliesWithSchema<T>(this HttpResponse<T> response)
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T>>(response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
