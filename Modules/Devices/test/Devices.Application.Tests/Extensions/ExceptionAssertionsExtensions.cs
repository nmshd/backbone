using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Backbone.Modules.Devices.Application.Tests.Extensions;
public static class ExceptionAssertionsExtensions
{
    public static void WithErrorCode<T>(this ExceptionAssertions<T> assertions, string code) where T : OperationFailedException
    {
        assertions.Which.Code.Should().Be(code);
    }

    public static async Task WithErrorCode<T>(this Task<ExceptionAssertions<T>> assertions, string code) where T : OperationFailedException
    {
        (await assertions).WithErrorCode(code);
    }
}
