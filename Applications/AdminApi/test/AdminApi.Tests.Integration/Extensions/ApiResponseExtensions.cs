using System.Text.Json;
using Backbone.AdminApi.Tests.Integration.Support;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Tests.Integration.Extensions;

[ShouldlyMethods]
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

    public static void ShouldBeASuccess<T>(this ApiResponse<T> response, string? customMessage = null)
    {
        if (response.IsError)
            throw new ShouldAssertException(new ActualShouldlyMessage($"Code: {response.Error.Code}, Message: {response.Error.Message}", customMessage).ToString());
    }
}
