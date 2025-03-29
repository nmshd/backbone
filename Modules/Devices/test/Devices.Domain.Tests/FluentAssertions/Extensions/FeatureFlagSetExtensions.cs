using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.FluentAssertions.Assertions;

namespace Backbone.Modules.Devices.Domain.Tests.FluentAssertions.Extensions;

public static class FeatureFlagSetExtensions
{
    public static FeatureFlagSetAssertions Should(this FeatureFlagSet instance) => new(instance);
}
