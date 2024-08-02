using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteRelationshipTemplatesOfIdentityCommand(TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Delete(A<Expression<Func<RelationshipTemplate, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
