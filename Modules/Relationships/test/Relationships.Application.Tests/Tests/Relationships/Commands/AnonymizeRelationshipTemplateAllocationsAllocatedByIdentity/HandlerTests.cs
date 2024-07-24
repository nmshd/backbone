﻿using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Persists_updated_allocations()
    {
        // Arrange
        var mockRepository = A.Fake<IRelationshipTemplatesRepository>();

        var oldIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTemplateAllocations = new List<RelationshipTemplateAllocation> { new(RelationshipTemplateId.New(), oldIdentityAddress, DeviceId.New()) };

        var request = new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(oldIdentityAddress);
        var handler = CreateHandler(mockRepository);

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
        var mockRepository = A.Fake<IRelationshipTemplatesRepository>();

        var oldIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anotherIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTemplateAllocations = new List<RelationshipTemplateAllocation> { new(RelationshipTemplateId.New(), oldIdentityAddress, DeviceId.New()) };

        var request = new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(anotherIdentityAddress);
        var handler = CreateHandler(mockRepository);

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

    private static Handler CreateHandler(IRelationshipTemplatesRepository mockRepository)
    {
        return new Handler(mockRepository, Options.Create(new ApplicationOptions { DidDomainName = "localhost" }));
    }
}
