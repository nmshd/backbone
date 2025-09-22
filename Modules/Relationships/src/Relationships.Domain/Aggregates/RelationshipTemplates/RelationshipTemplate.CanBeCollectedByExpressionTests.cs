using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.TestHelpers;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateCanBeCollectedBy : AbstractTestsBase
{
    private const string I1 = "did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f";
    private const string I2 = "did:e:prod.enmeshed.eu:dids:cdef0d1a1f2545703f40ca";
    private const string I3 = "did:e:prod.enmeshed.eu:dids:998351a946063fa1c28e04";

    [Theory]
    [InlineData(I1, null, I1, true)] // creator can collect if no forIdentity 
    [InlineData(I1, null, I3, true)] // third party can collect if no forIdentity
    [InlineData(I1, I2, I1, true)] // creator can collect if forIdentity
    [InlineData(I1, I2, I2, true)] // forIdentity can collect if forIdentity
    [InlineData(I1, I2, I3, false)] // third party can't collect if forIdentity
    [InlineData(I1, I1, I1, true)] // creator can collect if it is also forIdentity
    [InlineData(I1, I1, I3, false)] // third party can't collect if creator is also forIdentity
    public void Expression_CanBeCollectedBy_Returns_Correct_Result(string creator, string? forIdentity, string collector, bool expectedResult)
    {
        var creatorAddress = IdentityAddress.ParseUnsafe(creator);
        var forIdentityAddress = forIdentity == null ? null : IdentityAddress.ParseUnsafe(forIdentity);
        var collectorAddress = IdentityAddress.ParseUnsafe(collector);

        // Arrange
        var token = TestData.CreateRelationshipTemplate(creatorAddress, forIdentityAddress);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, collectorAddress);

        // Assert
        result.ShouldBe(expectedResult);
    }

    private static bool EvaluateCanBeCollectedByExpression(RelationshipTemplate relationshipTemplate, IdentityAddress identityAddress)
    {
        var expression = RelationshipTemplate.CanBeCollectedBy(identityAddress);
        var result = expression.Compile()(relationshipTemplate);
        return result;
    }
}
