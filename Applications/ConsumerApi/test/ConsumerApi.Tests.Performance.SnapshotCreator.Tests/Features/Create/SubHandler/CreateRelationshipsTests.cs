using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateRelationshipsTests
{
    [Fact]
    public async Task Handle_ShouldCreateRelationships()
    {
        var relationshipFactory = A.Fake<IRelationshipFactory>();
        var handler = new CreateRelationships.CommandHandler(relationshipFactory);
        var command = new CreateRelationships.Command(
            [
                new DomainIdentity(null!, null, 0, 0, 2, IdentityPoolType.App, 5, "", 2, 0),
                new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 5, 0),
                new DomainIdentity(null!, null, 0, 0, 3, IdentityPoolType.Connector, 5, "", 3, 0)
                {
                    RelationshipTemplates =
                    {
                        new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                        {
                            Id = "null",
                            CreatedAt = default
                        }, used: false)
                    }
                }
            ],
            [],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        var expectedConnectorCount = command.Identities.Count(i => i.IdentityPoolType == IdentityPoolType.Connector);
        var expectedAppCount = command.Identities.Count(i => i.IdentityPoolType == IdentityPoolType.App);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        A.CallTo(() => relationshipFactory.Create(command,
                A<DomainIdentity>.That.Matches(d => d.IdentityPoolType == IdentityPoolType.App),
                A<DomainIdentity[]>.That.Matches(d => d.Length == expectedConnectorCount)))
            .MustHaveHappened(expectedAppCount, Times.Exactly);
    }

    [Fact]
    public async Task Handle_ConnectorIdenitityHasNoRelationshipTemplates_ShouldThrowException()
    {
        var relationshipFactory = A.Fake<IRelationshipFactory>();
        var handler = new CreateRelationships.CommandHandler(relationshipFactory);
        var command = new CreateRelationships.Command(
            [
                new DomainIdentity(null!, null, 0, 0, 2, IdentityPoolType.App, 5, "", 2, 0),
                new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 5, 0),
                new DomainIdentity(null!, null, 0, 0, 3, IdentityPoolType.Connector, 5, "", 3, 0)
            ],
            [],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        var act = () => handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
