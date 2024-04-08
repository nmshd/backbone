using System.Text;
using FluentAssertions.Primitives;
using Xunit.Sdk;

namespace Backbone.UnitTestTools.FluentAssertions.Extensions;

public static class ObjectAssertionsExtensions
{
    public static void NotHaveNullProperties(this ObjectAssertions objectAssertions)
    {
        var nullPropertyNames = new StringBuilder();

        foreach (var property in objectAssertions.Subject.GetType().GetProperties()
                     .Where(p => p.GetValue(objectAssertions.Subject) == null))
            nullPropertyNames.AppendLine($"- {property.Name}");

        if (nullPropertyNames.Length > 0)
            throw new XunitException(
                "Expected all properties not to be null, but the following properties do not meet this requirement: \r\n" +
                nullPropertyNames);
    }
}
