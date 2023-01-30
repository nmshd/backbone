using Backbone.Modules.Relationships.Domain;
using Backbone.Modules.Relationships.Domain.Errors;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Relationships.Domain.Tests.Extensions;

public static class ExceptionAssertionsExtensions
{
    public static void WithError<T>(this ExceptionAssertions<T> exceptionAssertions, DomainError error) where T : DomainException
    {
        exceptionAssertions.Which.Error.Code.Should().Be(error.Code);
    }
}
