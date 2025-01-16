using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Tests.Integration.Assertions;
using FluentAssertions.Execution;

namespace Backbone.ConsumerApi.Tests.Integration.Extensions;

public static class IResponseExtensions
{
    public static IResponseAssertions Should(this IResponse actualValue)
    {
        return new IResponseAssertions(actualValue, AssertionChain.GetOrCreate());
    }
}
