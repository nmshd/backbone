using FluentAssertions.Specialized;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Application.Tests;

public static class ExceptionAssertionsExtensions
{
    public static void WithErrorCode<T>(this ExceptionAssertions<T> assertions, string code) where T : ApplicationException
    {
        assertions.Which.Code.Should().Be(code);
    }

    public static async Task WithErrorCode<T>(this Task<ExceptionAssertions<T>> assertions, string code) where T : ApplicationException
    {
        (await assertions).WithErrorCode(code);
    }
}
