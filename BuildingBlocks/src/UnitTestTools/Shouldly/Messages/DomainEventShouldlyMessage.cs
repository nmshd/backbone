using System.Runtime.CompilerServices;
using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Messages;

public class DomainEventShouldlyMessage : ShouldlyMessage
{
    /**
     * Invoke this constructor (or its simplifications) to indicate a failed type check for the events
     */
    public DomainEventShouldlyMessage(Type expected, Type actual, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
        : this([expected], [actual], customMessage, shouldlyMethod)
    {
    }

    /**
     * Invoke this constructor (or its simplifications) to indicate a failed type check for the events
     */
    public DomainEventShouldlyMessage(Type expected, IEnumerable<Type> actual, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
        : this([expected], actual, customMessage, shouldlyMethod)
    {
    }

    /**
     * Invoke this constructor (or its simplifications) to indicate a failed type check for the events
     */
    public DomainEventShouldlyMessage(IEnumerable<Type> expected, IEnumerable<Type> actual, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
    {
        var joinedExpectedTypes = string.Join(", ", expected.Select(t => t.Name));
        var joinedActualTypes = string.Join(", ", actual.Select(t => t.Name));

        ShouldlyAssertionContext = new ShouldlyAssertionContext(shouldlyMethod, joinedExpectedTypes, joinedActualTypes)
        {
            HasRelevantActual = true
        };
        if (customMessage != null) ShouldlyAssertionContext.CustomMessage = customMessage;
    }

    /**
     * Invoke this constructor (or its simplifications) to indicate a wrong number of events
     */
    public DomainEventShouldlyMessage(Type expected, int foundCount, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
        : this([expected], foundCount, customMessage, shouldlyMethod)
    {
    }

    /**
     * Invoke this constructor (or its simplifications) to indicate a wrong number of events
     */
    public DomainEventShouldlyMessage(IEnumerable<Type> expected, int foundCount, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
    {
        var joinedExpectedTypes = string.Join(", ", expected.Select(t => t.Name));

        ShouldlyAssertionContext = new ShouldlyAssertionContext(shouldlyMethod, joinedExpectedTypes, $"{foundCount} event(s)")
        {
            HasRelevantActual = true
        };
        if (customMessage != null) ShouldlyAssertionContext.CustomMessage = customMessage;
    }

    /**
     * Invoke this constructor to indicate that a domain event was expected to not be found, but was found
     */
    public DomainEventShouldlyMessage(Type notExpected, string? customMessage = null, [CallerMemberName] string shouldlyMethod = null!)
    {
        ShouldlyAssertionContext = new ShouldlyAssertionContext(shouldlyMethod, notExpected.Name);
        if (customMessage != null) ShouldlyAssertionContext.CustomMessage = customMessage;
    }
}
