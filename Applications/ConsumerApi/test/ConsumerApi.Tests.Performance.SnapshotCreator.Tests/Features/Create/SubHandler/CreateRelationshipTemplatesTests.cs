using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateRelationshipTemplatesTests
{
    private readonly CreateRelationshipTemplates.CommandHandler _sut;
    private readonly IRelationshipTemplateFactory _relationshipTemplateFactory;

    public CreateRelationshipTemplatesTests()
    {
        _relationshipTemplateFactory = A.Fake<IRelationshipTemplateFactory>();
        _sut = new CreateRelationshipTemplates.CommandHandler(_relationshipTemplateFactory);
    }

    [Fact]
    public async Task Handle_ShouldCreateRelationshipTemplates()
    {
        // Arrange
        var identities = new List<DomainIdentity>
        {
            new(null!, null, 0, 0, 2, IdentityPoolType.Never, 5, "", 2, 0),
            new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 5, 0),
            new(null!, null, 0, 0, 3, IdentityPoolType.Never, 5, "", 3, 0)
        };

        var countIdentitiesWithRelationshipTemplates = identities.Count(i => i.NumberOfRelationshipTemplates > 0);
        var expectedTotalRelationshipTemplates = identities.Sum(i => i.NumberOfRelationshipTemplates);

        var command = new CreateRelationshipTemplates.Command(
            identities,
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _relationshipTemplateFactory.Create(A<CreateRelationshipTemplates.Command>._, A<DomainIdentity>._))
            .MustHaveHappened(countIdentitiesWithRelationshipTemplates, Times.Exactly);

        _relationshipTemplateFactory.TotalConfiguredRelationshipTemplates.Should().Be(expectedTotalRelationshipTemplates);
    }
}
