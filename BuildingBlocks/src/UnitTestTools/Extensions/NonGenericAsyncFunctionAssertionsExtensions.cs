using FluentAssertions.Specialized;

namespace Enmeshed.UnitTestTools.Extensions;

public static class NonGenericAsyncFunctionAssertionsExtensions
{
    public static ExceptionAssertions<TException> AwaitThrowAsync<TException>(this NonGenericAsyncFunctionAssertions assertions, string because = "",
        params object[] becauseArgs)
        where TException : Exception
    {
        return assertions.ThrowAsync<TException>(because, becauseArgs).WaitAsync(CancellationToken.None).Result;
    }
}
