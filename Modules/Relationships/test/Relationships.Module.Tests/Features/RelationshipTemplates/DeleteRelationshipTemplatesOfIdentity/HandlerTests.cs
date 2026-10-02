using DeleteRelationshipTemplatesOfIdentitySlice = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;
using System.Linq.Expressions;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Module.Tests.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteRelationshipTemplatesOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() };

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Delete(A<Expression<Func<RelationshipTemplate, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
