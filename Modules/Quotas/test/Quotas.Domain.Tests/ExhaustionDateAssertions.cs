using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Tooling.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;

namespace Backbone.Modules.Quotas.Domain.Tests;

public static class ExhaustionDateExtensions
{
    public static ExhaustionDateAssertions Should(this ExhaustionDate instance)
    {
        return new ExhaustionDateAssertions(instance);
    }
}

public class ExhaustionDateAssertions :
    ComparableTypeAssertions<ExhaustionDate, ExhaustionDateAssertions>
{
    public ExhaustionDateAssertions(ExhaustionDate subject) : base(subject) { }

    protected override string Identifier => "ExhaustionDate";

    public AndConstraint<ExhaustionDateAssertions> BeEndOfHour(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(exhaustionDate => ((ExhaustionDate)exhaustionDate).Value == ((ExhaustionDate)exhaustionDate).Value.EndOfHour())
            .FailWith("Expected {context:ExhaustionDate} to be {0}{reason}, but found {1}.",
                actual => new ExhaustionDate(((ExhaustionDate)actual).Value.EndOfHour()), actual => actual);

        return new AndConstraint<ExhaustionDateAssertions>(this);
    }

    public AndConstraint<ExhaustionDateAssertions> BeEndOfDay(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(exhaustionDate => ((ExhaustionDate)exhaustionDate).Value == ((ExhaustionDate)exhaustionDate).Value.EndOfDay())
            .FailWith("Expected {context:ExhaustionDate} to be {0}{reason}, but found {1}.",
                actual => new ExhaustionDate(((ExhaustionDate)actual).Value.EndOfDay()), actual => actual);

        return new AndConstraint<ExhaustionDateAssertions>(this);
    }
}
