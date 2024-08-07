using System.Linq.Expressions;
using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        // Arrange
        const string identityAddress = "identity-address";
        var mockChallengesRepository = A.Fake<IChallengesRepository>();
        var handler = CreateHandler(mockChallengesRepository);

        // Act
        await handler.Handle(new DeleteChallengesOfIdentityCommand(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => mockChallengesRepository.Delete(A<Expression<Func<Challenge, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IChallengesRepository challengesRepository)
    {
        return new Handler(challengesRepository);
    }
}
