using Backbone.BuildingBlocks.Domain;
using FluentAssertions;

namespace Backbone.UnitTestTools.FluentAssertions.Extensions;

public static class EntityExtensions
{
    public static EntityAssertions Should(this Entity instance) => new(instance);
}
