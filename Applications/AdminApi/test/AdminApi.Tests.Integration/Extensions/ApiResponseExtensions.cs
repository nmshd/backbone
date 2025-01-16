using Backbone.AdminApi.Tests.Integration.Assertions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using FluentAssertions.Execution;

namespace Backbone.AdminApi.Tests.Integration.Extensions;

public static class ApiResponseExtensions
{
    public static ApiResponseAssertions<T> Should<T>(this ApiResponse<T> instance)
    {
        return new ApiResponseAssertions<T>(instance, AssertionChain.GetOrCreate());
    }
}
