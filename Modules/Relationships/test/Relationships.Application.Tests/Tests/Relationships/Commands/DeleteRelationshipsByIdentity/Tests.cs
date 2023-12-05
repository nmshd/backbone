using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsByIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.DeleteRelationshipsByIdentity;
public class Tests
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var templatesRepository = A.Fake<IRelationshipsRepository>();

        var handler = new Handler(templatesRepository);
        var request = new DeleteRelationshipsByIdentityCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => templatesRepository.DeleteRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
