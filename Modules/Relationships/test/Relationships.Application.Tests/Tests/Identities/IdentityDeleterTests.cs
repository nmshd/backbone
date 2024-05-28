﻿using Backbone.Modules.Relationships.Application.Identities;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Identities;
public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var mockMediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(mockMediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(
            A<DeleteRelationshipTemplatesOfIdentityCommand>.That.Matches(i => i.IdentityAddress == identityAddress),
            A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
