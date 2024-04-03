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
    public async Task Persists_updated_allocations()
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
        A.CallTo(() => mockRepository.UpdateRelationshipTemplateAllocations(
            A<List<RelationshipTemplateAllocation>>.That.Matches(l => l.Contains(relationshipTemplateAllocations.First())),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_persist_allocations_that_were_not_updated()
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
        A.CallTo(() => mockRepository.UpdateRelationshipTemplateAllocations(
            A<List<RelationshipTemplateAllocation>>.That.Matches(l => l.Contains(relationshipTemplateAllocations.First())),
            A<CancellationToken>._)
        ).MustNotHaveHappened();
    }
}
