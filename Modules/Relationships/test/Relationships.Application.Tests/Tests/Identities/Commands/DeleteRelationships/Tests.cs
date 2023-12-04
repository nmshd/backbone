using Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationships;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Identities.Commands.DeleteRelationships;
public class Tests
{
    [Fact]
    public async Task Command_calls_delete_on_repository()
    {
        // Arrange
        var relationshipsRepository = A.Fake<IRelationshipsRepository>();
        var templateRelationshipsRepository = A.Fake<IRelationshipTemplatesRepository>();

        var handler = new Handler(relationshipsRepository, templateRelationshipsRepository);
        var request = new DeleteRelationshipsCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => relationshipsRepository.Delete(A<IEnumerable<RelationshipId>>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => templateRelationshipsRepository.Delete(A<IEnumerable<RelationshipTemplateId>>._, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Command_calls_delete_for_right_relationships_in_from()
    {
        // Arrange
        var relationshipsRepository = A.Fake<IRelationshipsRepository>();

        var identityAddress = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = UnitTestTools.Data.TestDataGenerator.CreateRandomDeviceId();
        var deviceId2 = UnitTestTools.Data.TestDataGenerator.CreateRandomDeviceId();
        var relationshipTemplate = new RelationshipTemplate(identityAddress, deviceId, null, null, []);
        var relationships = new List<Relationship>() {
            new(relationshipTemplate, identityAddress, deviceId, []),
            new(relationshipTemplate, identityAddress, deviceId2, [])
        };

        A.CallTo(() => relationshipsRepository.FindRelationshipsWithIdentityAddress(identityAddress, A<CancellationToken>._)).Returns(relationships);

        var templateRelationshipsRepository = A.Dummy<IRelationshipTemplatesRepository>();

        var handler = new Handler(relationshipsRepository, templateRelationshipsRepository);
        var request = new DeleteRelationshipsCommand(identityAddress);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        A.CallTo(() => relationshipsRepository.Delete(A<IEnumerable<RelationshipId>>.That.Matches(it => it.All(r => relationships.Select(or => or.Id).Contains(r))), A<CancellationToken>._)).MustHaveHappened();
    }
}
