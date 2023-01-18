using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Relationships.Application.Extensions;
using Relationships.Application.Infrastructure;
using Relationships.Application.IntegrationEvents;
using Relationships.Domain;
using Relationships.Domain.Entities;
using Relationships.Domain.Errors;

namespace Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;

public class Handler : IRequestHandler<AcceptRelationshipChangeRequestCommand, AcceptRelationshipChangeRequestResponse>
{
    private readonly IContentStore _contentStore;
    private readonly IDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IEventBus eventBus, IContentStore contentStore)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
        _contentStore = contentStore;
    }

    public async Task<AcceptRelationshipChangeRequestResponse> Handle(AcceptRelationshipChangeRequestCommand changeRequest, CancellationToken cancellationToken)
    {
        var relationship = await _dbContext
            .Set<Relationship>()
            .IncludeAll()
            .FirstWithId(changeRequest.Id, cancellationToken);

        var change = relationship.AcceptChange(changeRequest.ChangeId, _userContext.GetAddress(), _userContext.GetDeviceId(), changeRequest.ResponseContent);

        try
        {
            await _contentStore.SaveContentOfChangeResponse(change.Response);
        }
        catch (BlobAlreadyExistsException)
        {
            throw new DomainException(DomainErrors.ChangeRequestIsAlreadyCompleted(change.Status));
        }

        _dbContext.Set<Relationship>().Update(relationship);
        await _dbContext.SaveChangesAsync(cancellationToken);

        PublishIntegrationEvent(change);

        var response = _mapper.Map<AcceptRelationshipChangeRequestResponse>(relationship);

        return response;
    }

    private void PublishIntegrationEvent(RelationshipChange change)
    {
        var evt = new RelationshipChangeCompletedIntegrationEvent(change);
        _eventBus.Publish(evt);
    }
}
