using DecomposeAndAnonymizeRelationshipsOfIdentitySlice = Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FakeItEasy;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Module.Tests.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Command_calls_update_on_repository()
    {
        // Arrange
        var mockRelationshipTemplatesRepository = A.Fake<IRelationshipsRepository>();
        var mockOptions = A.Dummy<IOptions<ApplicationConfiguration>>();

        var handler = new Handler(mockRelationshipTemplatesRepository, mockOptions);
        var request = new DecomposeAndAnonymizeRelationshipsOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() };

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRelationshipTemplatesRepository.Update(A<IEnumerable<Relationship>>._)).MustHaveHappened();
    }
}
