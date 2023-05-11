﻿using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.IntegrationEvents;
using Backbone.Modules.Relationships.Domain;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Errors;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;

public class Handler : IRequestHandler<RevokeRelationshipChangeRequestCommand, RevokeRelationshipChangeRequestResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IEventBus eventBus, IRelationshipsRepository relationshipsRepository)
    {
        _userContext = userContext;
        _relationshipsRepository = relationshipsRepository;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    public async Task<RevokeRelationshipChangeRequestResponse> Handle(RevokeRelationshipChangeRequestCommand changeRequest, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(changeRequest.Id, _userContext.GetAddress(), cancellationToken, track: true, fillContent: false);

        var change = relationship.RevokeChange(changeRequest.ChangeId, _userContext.GetAddress(), _userContext.GetDeviceId(), changeRequest.ResponseContent);

        try
        {
            await _relationshipsRepository.SaveContentOfChangeResponse(change.Response);
        }
        catch (BlobAlreadyExistsException)
        {
            throw new DomainException(DomainErrors.ChangeRequestIsAlreadyCompleted(change.Status));
        }

        await _relationshipsRepository.Update(relationship);

        PublishIntegrationEvent(change);

        var response = _mapper.Map<RevokeRelationshipChangeRequestResponse>(relationship);

        return response;
    }

    private void PublishIntegrationEvent(RelationshipChange change)
    {
        var evt = new RelationshipChangeCompletedIntegrationEvent(change);
        _eventBus.Publish(evt);
    }
}
