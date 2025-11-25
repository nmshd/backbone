using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Quotas.Domain.Tests;

[ShouldlyMethods]
public static class ExhaustionDateExtensions
{
    extension(ExhaustionDate instance)
    {
        public void ShouldBeEndOfHour(string? customMessage = null)
        {
            instance.AssertAwesomely(v => v.Value == v.Value.EndOfHour(), instance.Value, instance.Value.EndOfHour(), customMessage);
        }

        public void ShouldBeEndOfDay(string? customMessage = null)
        {
            instance.AssertAwesomely(v => v.Value == v.Value.EndOfDay(), instance.Value, instance.Value.EndOfDay(), customMessage);
        }
    }
}
