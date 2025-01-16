using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Tooling.Extensions;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;

namespace Backbone.Modules.Quotas.Domain.Tests;

public static class ExhaustionDateExtensions
{
    public static ExhaustionDateAssertions Should(this ExhaustionDate instance)
    {
        return new ExhaustionDateAssertions(instance, AssertionChain.GetOrCreate());
    }
}

public class ExhaustionDateAssertions :
    ComparableTypeAssertions<ExhaustionDate, ExhaustionDateAssertions>
{
    private readonly AssertionChain _assertionChain;

    public ExhaustionDateAssertions(ExhaustionDate subject, AssertionChain assertionChain) : base(subject, assertionChain)
    {
        _assertionChain = assertionChain;
    }

    protected override string Identifier => "ExhaustionDate";

    public AndConstraint<ExhaustionDateAssertions> BeEndOfHour(string because = "", params object[] becauseArgs)
    {
        _assertionChain
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(exhaustionDate => ((ExhaustionDate)exhaustionDate).Value == ((ExhaustionDate)exhaustionDate).Value.EndOfHour())
            .FailWith("Expected {context:ExhaustionDate} to be {0}{reason}, but found {1}.",
                actual => new ExhaustionDate(((ExhaustionDate)actual).Value.EndOfHour()), actual => actual);

        return new AndConstraint<ExhaustionDateAssertions>(this);
    }

    public AndConstraint<ExhaustionDateAssertions> BeEndOfDay(string because = "", params object[] becauseArgs)
    {
        _assertionChain
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject)
            .ForCondition(exhaustionDate => ((ExhaustionDate)exhaustionDate).Value == ((ExhaustionDate)exhaustionDate).Value.EndOfDay())
            .FailWith("Expected {context:ExhaustionDate} to be {0}{reason}, but found {1}.",
                actual => new ExhaustionDate(((ExhaustionDate)actual).Value.EndOfDay()), actual => actual);

        return new AndConstraint<ExhaustionDateAssertions>(this);
    }
}
