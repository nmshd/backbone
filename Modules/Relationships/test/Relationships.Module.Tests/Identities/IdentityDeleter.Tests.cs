using AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;
using AnonymizeRelationshipTemplatesForIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;
using DecomposeAndAnonymizeRelationshipsOfIdentity = Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;
using DeleteRelationshipTemplatesOfIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Relationships.Module.Features.Identities.DeleteIdentity;
using FakeItEasy;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Tests.Identities;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(mockMediator, mockIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(
            A<DecomposeAndAnonymizeRelationshipsOfIdentity.Command>.That.Matches(i => i.IdentityAddress == identityAddress),
            A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockMediator.Send(
            A<DeleteRelationshipTemplatesOfIdentity.Command>.That.Matches(i => i.IdentityAddress == identityAddress),
            A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockMediator.Send(
            A<AnonymizeRelationshipTemplatesForIdentity.Command>.That.Matches(i => i.IdentityAddress == identityAddress),
            A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockMediator.Send(
            A<AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity.Command>.That.Matches(i => i.IdentityAddress == identityAddress),
            A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }


    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Dummy<IMediator>();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(dummyMediator, mockIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "Relationships")).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplates")).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplateAllocations")).MustHaveHappenedOnceExactly();
    }
}
