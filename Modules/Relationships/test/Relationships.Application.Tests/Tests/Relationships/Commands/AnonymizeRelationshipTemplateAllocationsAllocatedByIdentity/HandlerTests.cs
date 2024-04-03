using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class HandlerTests
{
    [Fact]
    public async Task Command_calls_update_for_each_RelationshipTemplateAllocation()
    {
        // Arrange
        var mockRepository = A.Fake<IRelationshipsRepository>();

        var oldIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTemplateAllocations = new List<RelationshipTemplateAllocation> { new(RelationshipTemplateId.New(), oldIdentityAddress, DeviceId.New()) };

        var request = new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(oldIdentityAddress);
        var handler = new Handler(mockRepository);

        A.CallTo(() => mockRepository.FindRelationshipTemplateAllocations(A<Expression<Func<RelationshipTemplateAllocation, bool>>>._, A<CancellationToken>._))
            .Returns(relationshipTemplateAllocations);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRepository.UpdateRelationshipTemplateAllocation(A<RelationshipTemplateAllocation>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Command_does_not_call_update_for_RelationshipTemplateAllocation_with_different_IdentityAddress()
    {
        // Arrange
        var mockRepository = A.Fake<IRelationshipsRepository>();

        var oldIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anotherIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTemplateAllocations = new List<RelationshipTemplateAllocation> { new(RelationshipTemplateId.New(), oldIdentityAddress, DeviceId.New()) };

        var request = new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(anotherIdentityAddress);
        var handler = new Handler(mockRepository);

        A.CallTo(() => mockRepository.FindRelationshipTemplateAllocations(A<Expression<Func<RelationshipTemplateAllocation, bool>>>._, A<CancellationToken>._))
            .Returns(relationshipTemplateAllocations);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRepository.UpdateRelationshipTemplateAllocation(A<RelationshipTemplateAllocation>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}
