using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationshipsOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_find_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipsRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DecomposeRelationshipsOfIdentityCommand(CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockRelationshipTemplatesRepository.Update(A<IEnumerable<Relationship>>._)).MustHaveHappened();
    }
}
