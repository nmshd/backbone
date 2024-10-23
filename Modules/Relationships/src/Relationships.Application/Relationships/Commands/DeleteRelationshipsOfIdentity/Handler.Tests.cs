using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipsRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteRelationshipsOfIdentityCommand(CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.DeleteRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
