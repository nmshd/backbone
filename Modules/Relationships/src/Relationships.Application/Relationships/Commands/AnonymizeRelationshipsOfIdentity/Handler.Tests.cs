﻿using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AnonymizeRelationshipsOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_update_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipsRepository>();
        var mockOptions = A.Dummy<IOptions<ApplicationOptions>>();

        var handler = new Handler(mockRelationshipTemplatesRepository, mockOptions);
        var request = new AnonymizeRelationshipsOfIdentityCommand(CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Update(A<IEnumerable<Relationship>>._)).MustHaveHappened();
    }
}
