using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Shouldly.Extensions;

[ShouldlyMethods]
public static class FeatureFlagSetExtensions
{
    public static void ShouldContain(this FeatureFlagSet instance, FeatureFlagName name, string? customMessage = null)
    {
        instance.AssertAwesomely(s => s.Contains(name), instance, name, customMessage);
    }
}
