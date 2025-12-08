using System.Reflection;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Tests.Integration.Extensions;

[ShouldlyMethods]
public static class IResponseExtensions
{
    extension(IResponse response)
    {
        public void ShouldBeASuccess(string? customMessage = null)
        {
            if (response.IsError)
                throw new ShouldAssertException(new ActualShouldlyMessage($"Code: {response.Error!.Code}, Message: {response.Error.Message}", customMessage).ToString());
        }

        public void ShouldBeAnError(string? customMessage = null)
        {
            if (response.IsSuccess)
                throw new ShouldAssertException(new ActualShouldlyMessage(response.Status, customMessage).ToString());
        }

        public async Task ShouldComplyWithSchema(string? customMessage = null)
        {
            var type = response.GetType();
            if (!type.IsGenericType)
                throw new ShouldAssertException(new ExpectedShouldlyMessage("should be generic", customMessage).ToString());

            var resultType = type.GetGenericArguments()[0];
            var method = typeof(IResponseExtensions)
                .GetMethod(nameof(CheckConvertedResponse), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(resultType);
            var task = (Task)method.Invoke(null, [response, customMessage])!;
            await task;
        }
    }

    private static async Task CheckConvertedResponse<T>(IResponse response, string? customMessage = null)
    {
        var generic = response as ApiResponse<T>;
        await generic!.ShouldComplyWithSchema(customMessage);
    }
}
