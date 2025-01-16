using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Integration.Assertions;
using FluentAssertions.Execution;

namespace Backbone.ConsumerApi.Tests.Integration.Extensions;

public static class ApiResponseExtensions
{
    public static ApiResponseAssertions<T> Should<T>(this ApiResponse<T> actualValue)
    {
        return new ApiResponseAssertions<T>(actualValue, AssertionChain.GetOrCreate());
    }
}
