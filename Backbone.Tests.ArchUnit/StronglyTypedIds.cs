using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.xUnitV3;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class StronglyTypedIds
{
    private static readonly IObjectProvider<IType> NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST =
        Classes()
            .That().AreAssignableTo(typeof(IRequest<>)).Or().AreAssignableTo(typeof(IRequest)).As("Classes that implement 'IRequest'")
            .And().AreNotAbstract();

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

    [Fact]
    public void NoStronglyTypedIdsInMediatrCommandsAndQueries()
    {
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .Should().FollowCustomCondition(type =>
            {
                var valueObjectDoesNotExist = !type.GetPropertyMembers().Any(f => f.Type.IsAssignableTo(typeof(StronglyTypedId).FullName));
                const string errorMessage = $"{nameof(type)} should use string instead of value objects ";

                return new ConditionResult(type, valueObjectDoesNotExist, valueObjectDoesNotExist ? string.Empty : errorMessage);
            }, "Should use string instead of value objects in Mediatr Commands and Queries.")
            .Check(Backbone.ARCHITECTURE);
    }
}
