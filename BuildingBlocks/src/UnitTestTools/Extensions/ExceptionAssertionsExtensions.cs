using Backbone.BuildingBlocks.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Backbone.UnitTestTools.Extensions;

public static class ExceptionAssertionsExtensions
{
    public static void WithError<T>(this ExceptionAssertions<T> exceptionAssertions, string code, string? messagePart = null) where T : DomainException
    {
        var exception = exceptionAssertions.Which;

        exception.Code.Should().Be(code);

        if (messagePart != null)
            exception.Message.Should().Contain(messagePart);
    }
}
