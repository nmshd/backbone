using Backbone.BuildingBlocks.Domain;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Backbone.Modules.Quotas.Application.Tests.Extensions;
public static class ExceptionAssertionsExtensions
{
    public static void WithErrorCode<T>(this ExceptionAssertions<T> assertions, string code) where T : DomainException
    {
        assertions.Which.Code.Should().Be(code);
    }

    public static async Task WithErrorCode<T>(this Task<ExceptionAssertions<T>> assertions, string code) where T : DomainException
    {
        (await assertions).WithErrorCode(code);
    }
}
