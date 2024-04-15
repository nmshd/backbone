using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Commands.DeleteRelationshipTemplatesByIdentity;
public class HandlerTests
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipTemplatesRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteRelationshipTemplatesOfIdentityCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Delete(A<Expression<Func<RelationshipTemplate, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
