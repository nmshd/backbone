using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Validators;
using FluentAssertions;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Extensions;

public static class ApiResponseExtensions
{
    public static void AssertContentCompliesWithSchema<T>(this ApiResponse<T> response)
    {
        JsonValidators.ValidateJsonSchema<ResponseContent<T>>(response.RawContent!, out var errors)
            .Should().BeTrue($"Response content does not comply with the {typeof(T).FullName} schema: {string.Join(", ", errors)}");
    }
}
