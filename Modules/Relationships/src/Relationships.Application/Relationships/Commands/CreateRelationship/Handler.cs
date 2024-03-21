using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

public class Handler : IRequestHandler<CreateRelationshipCommand, CreateRelationshipResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;
    private CancellationToken _cancellationToken;
    private CreateRelationshipCommand _request;
    private RelationshipTemplate _template;
    private Relationship _relationship;

    public Handler(IUserContext userContext, IMapper mapper, IEventBus eventBus, IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
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

        return CreateResponse();
    }

    private async Task ReadTemplateFromDb()
    {
        _template = await _relationshipTemplatesRepository.Find(_request.RelationshipTemplateId, _userContext.GetAddress(), _cancellationToken, track: true) ??
                    throw new NotFoundException(nameof(RelationshipTemplate));
    }

    private async Task CreateAndSaveRelationship()
    {
        var activeIdentity = _userContext.GetAddress();
        var templateOwner = _template.CreatedBy;

        var existingRelationships = await _relationshipsRepository.FindRelationships(
            r =>
                (r.From == activeIdentity && r.To == templateOwner) ||
                (r.From == templateOwner && r.To == activeIdentity),
            _cancellationToken
        );

        _relationship = new Relationship(
            _template,
            activeIdentity,
            _userContext.GetDeviceId(),
            _request.Content,
            existingRelationships.ToList()
        );

        await _relationshipsRepository.Add(_relationship, _cancellationToken);
    }

    private void PublishIntegrationEvent()
    {
        _eventBus.Publish(new RelationshipCreatedIntegrationEvent(_relationship));
    }

    private CreateRelationshipResponse CreateResponse()
    {
        return _mapper.Map<CreateRelationshipResponse>(_relationship);
    }
}
