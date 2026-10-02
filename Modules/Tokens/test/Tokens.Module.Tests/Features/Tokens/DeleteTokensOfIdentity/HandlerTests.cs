using System.Linq.Expressions;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;
using FakeItEasy;
using DeleteTokensOfIdentitySlice = Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

namespace Backbone.Modules.Tokens.Module.Tests.Features.Tokens.DeleteTokensOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<ITokensRepository>();

        var handler = new Handler(mockRelationshipTemplatesRepository);
        var request = new DeleteTokensOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() };

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Delete(A<Expression<Func<Token, bool>>>._, A<CancellationToken>._)).MustHaveHappened();
    }
}
