using FluentAssertions;
using FluentAssertions.Specialized;
using Relationships.Domain.Errors;

namespace Relationships.Domain.Tests.Extensions;

public static class ExceptionAssertionsExtensions
{
    public static void WithError<T>(this ExceptionAssertions<T> exceptionAssertions, DomainError error) where T : DomainException
    {
        exceptionAssertions.Which.Error.Code.Should().Be(error.Code);
    }
}
