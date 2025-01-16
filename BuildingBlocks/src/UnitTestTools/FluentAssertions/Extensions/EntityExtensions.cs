using Backbone.BuildingBlocks.Domain;
using Backbone.UnitTestTools.FluentAssertions.Assertions;
using FluentAssertions.Execution;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace FluentAssertions;
#pragma warning restore IDE0130

public static class EntityExtensions
{
    public static EntityAssertions Should(this Entity? instance) => new(instance, AssertionChain.GetOrCreate());
}
