﻿using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;
using Backbone.Modules.Challenges.Application.Identities;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using MediatR;
using Xunit;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Identities;
public class IdentityDeleterTests
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deleter = new IdentityDeleter(mediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mediator.Send(A<DeleteChallengesOfIdentityCommand>.That.Matches(r => r.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }
}
