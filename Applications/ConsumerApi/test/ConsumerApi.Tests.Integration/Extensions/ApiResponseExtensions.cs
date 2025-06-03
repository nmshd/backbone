using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Integration.Support;

namespace Backbone.ConsumerApi.Tests.Integration.Extensions;

public static class ApiResponseExtensions
{
    public static async Task ShouldComplyWithSchema<T>(this ApiResponse<T> response, string? customMessage = null)
    {
        if (response.Result == null)
            throw new ShouldAssertException(new ActualShouldlyMessage(null, customMessage).ToString());

        var resultJson = JsonSerializer.Serialize(response.Result);
        var (isValid, errors) = await JsonValidator.ValidateJsonSchema<T>(resultJson);

        if (!isValid)
            throw new ShouldAssertException(new ActualShouldlyMessage(string.Join(", ", errors), customMessage).ToString());
    }
}
