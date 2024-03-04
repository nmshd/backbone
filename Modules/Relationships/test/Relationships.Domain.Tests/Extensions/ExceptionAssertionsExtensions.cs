using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Backbone.Modules.Relationships.Domain.Tests.Extensions;

public static class ExceptionAssertionsExtensions
{
    public static void WithError<T>(this ExceptionAssertions<T> exceptionAssertions, DomainError error) where T : DomainException
    {
        exceptionAssertions.Which.Code.Should().Be(error.Code);
    }
}
