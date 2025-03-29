using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Backbone.Modules.Devices.Domain.Tests.FluentAssertions.Assertions;

public class FeatureFlagSetAssertions : ObjectAssertions<FeatureFlagSet, FeatureFlagSetAssertions>
{
    public FeatureFlagSetAssertions(FeatureFlagSet instance) : base(instance)
    {
    }

    protected override string Identifier => "FeatureFlagSet";

    public void Contain(FeatureFlagName name, string because = "", params object[] becauseArgs)
    {
        if (Subject == null)
            throw new Exception("Subject cannot be null.");

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(e => Subject.Contains(name))
            .FailWith("Expected {context:FeatureFlagSet} to contain the feature with the name {0}.", name.Value);
    }
}
