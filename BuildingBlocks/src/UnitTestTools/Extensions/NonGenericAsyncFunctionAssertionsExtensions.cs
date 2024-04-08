using FluentAssertions.Specialized;

namespace Backbone.UnitTestTools.Extensions;

public static class NonGenericAsyncFunctionAssertionsExtensions
{
    public static ExceptionAssertions<TException> AwaitThrowAsync<TException>(this NonGenericAsyncFunctionAssertions assertions, string because = "",
        params object[] becauseArgs)
        where TException : Exception
    {
        return assertions.ThrowAsync<TException>(because, becauseArgs).WaitAsync(CancellationToken.None).Result;
    }

    public static ExceptionAssertions<TException> AwaitThrowAsync<TException, TResult>(this GenericAsyncFunctionAssertions<TResult> assertions, string because = "",
        params object[] becauseArgs)
        where TException : Exception
    {
        return assertions.ThrowAsync<TException>(because, becauseArgs).WaitAsync(CancellationToken.None).Result;
    }
}
