using Backbone.BuildingBlocks.Domain.Exceptions;
using Shouldly;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class ExceptionExtensions
{
    public static void ShouldHaveError(this DomainException exception, string code, string? messagePart = null, string? customMessage = null)
    {
        exception.Code.AssertAwesomely(v => string.Equals(v, code), exception.Code, code, customMessage);

        if (messagePart != null)
            exception.ShouldContainMessage(messagePart, customMessage);
    }

    public static T ShouldHaveErrorCode<T>(this T exception, string expectedCode, string? customMessage = null) where T : ApplicationException
    {
        exception.Code.AssertAwesomely(c => string.Equals(c, expectedCode), exception.Code, expectedCode, customMessage);

        return exception;
    }

    public static async Task<T> ShouldHaveErrorCode<T>(this Task<T> exception, string expectedCode, string? customMessage = null) where T : ApplicationException
    {
        return (await exception).ShouldHaveErrorCode(expectedCode, customMessage);
    }

    public static void ShouldContainMessage(this DomainException exception, string messagePart, string? customMessage = null)
    {
        exception.Message.AssertAwesomely(v => v.Contains(messagePart, StringComparison.CurrentCultureIgnoreCase), exception.Message, messagePart, customMessage);
    }

    public static T ShouldContainMessage<T>(this T exception, string messagePart, string? customMessage = null) where T : Exception
    {
        exception.Message.AssertAwesomely(m => m.Contains(messagePart, StringComparison.CurrentCultureIgnoreCase), exception.Message, messagePart, customMessage);

        return exception;
    }

    public static async Task<T> ShouldContainMessage<T>(this Task<T> exception, string messagePart, string? customMessage = null) where T : Exception
    {
        return (await exception).ShouldContainMessage(messagePart, customMessage);
    }
}
