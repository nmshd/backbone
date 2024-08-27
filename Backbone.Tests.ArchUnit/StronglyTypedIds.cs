using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.xUnit;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class StronglyTypedIds
{
    [Fact]
    public void StronglyTypedIdsShouldHaveIsValidMethod()
    {
        Classes().That().AreAssignableTo(typeof(StronglyTypedId))
            .And().AreNotAbstract()
            .Should().FollowCustomCondition(type =>
            {
                var methods = type.GetMethodMembers().ToArray();

                var isValidMethodExists = methods.Any(m => m.NameStartsWith("IsValid(") &&
                                                           m.IsStatic == true &&
                                                           m.Visibility == Visibility.Public &&
                                                           m.Parameters.Count() == 1 &&
                                                           m.Parameters.Single().FullName == typeof(string).FullName &&
                                                           m.ReturnType.FullName == typeof(bool).FullName);

                const string errorMessage = "should have a method with the following signature: 'public static boolean IsValid(string stringValue)'.";

                return new ConditionResult(type, isValidMethodExists, isValidMethodExists ? string.Empty : errorMessage);
            }, "should have a static IsValid method with a single parameter of type 'string'.")
            .Check(Backbone.ARCHITECTURE);
    }
}
