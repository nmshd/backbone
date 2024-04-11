using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

public class Handler : IRequestHandler<CreateRelationshipCommand, CreateRelationshipResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IdentityAddress _activeIdentity;
    private readonly DeviceId _activeDevice;

    private CancellationToken _cancellationToken;
    private CreateRelationshipCommand _request;
    private RelationshipTemplate _template;
    private Relationship _relationship;

    public Handler(IUserContext userContext, IEventBus eventBus, IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
        _relationshipsRepository = relationshipsRepository;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _eventBus = eventBus;

        _request = null!;
        _template = null!;
        _relationship = null!;
    }

    public async Task<CreateRelationshipResponse> Handle(CreateRelationshipCommand request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _request = request;

        await ReadTemplateFromDb();
        await CreateAndSaveRelationship();
        PublishIntegrationEvent();

        return new CreateRelationshipResponse(_relationship);
    }

    private async Task ReadTemplateFromDb()
    {
        var templateId = RelationshipTemplateId.Parse(_request.RelationshipTemplateId);

        _template = await _relationshipTemplatesRepository.Find(templateId, _activeIdentity, _cancellationToken, track: true) ??
                    throw new NotFoundException(nameof(RelationshipTemplate));
    }

    private async Task CreateAndSaveRelationship()
    {
        var existingRelationships = await _relationshipsRepository.FindRelationships(
            r =>
                (r.From == _activeIdentity && r.To == _template.CreatedBy) ||
                (r.From == _template.CreatedBy && r.To == _activeIdentity),
            _cancellationToken
        );

        var relationships = existingRelationships.ToList();

        EnsureNoTerminatedRelationshipExists(relationships);

        _relationship = new Relationship(
            _template,
            _activeIdentity,
            _activeDevice,
            _request.CreationContent,
            relationships
        );

        await _relationshipsRepository.Add(_relationship, _cancellationToken);
    }

    private void EnsureNoTerminatedRelationshipExists(List<Relationship> relationships)
    {
        foreach (var relationship in relationships)
            if (relationship.Status == RelationshipStatus.Terminated)
                throw new OperationFailedException(ApplicationErrors.Relationship
                    .CannotCreateRelationshipWhileTerminatedRelationshipExists(relationship.Id.ToString()));
    }

    private void PublishIntegrationEvent()
    {
        _eventBus.Publish(new RelationshipCreatedIntegrationEvent(_relationship));
    }
}
