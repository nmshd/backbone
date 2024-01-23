using System.Linq.Expressions;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
using Backbone.Modules.Tokens.Domain.Entities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Tokens.DeleteTokenOfIdentity;
public class HandlerTests
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<ITokensRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteTokensOfIdentityCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.DeleteTokens(A<Expression<Func<Token, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
