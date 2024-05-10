using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    public async Task<CreateRelationshipTemplateResponse> Handle(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new RelationshipTemplate(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.MaxNumberOfAllocations,
            request.ExpiresAt,
            request.Content);

        await _relationshipTemplatesRepository.Add(template, cancellationToken);

        PublishDomainEvent(template);

        return _mapper.Map<CreateRelationshipTemplateResponse>(template);
    }

    private void PublishDomainEvent(RelationshipTemplate template)
    {
        var evt = new RelationshipTemplateCreatedDomainEvent(template);
        _eventBus.Publish(evt);
    }
}
