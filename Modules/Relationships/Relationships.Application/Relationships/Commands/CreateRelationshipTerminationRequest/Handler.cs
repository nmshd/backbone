using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Relationships.Application.Extensions;
using Relationships.Application.IntegrationEvents;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Entities;

namespace Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;

public class Handler : IRequestHandler<CreateRelationshipTerminationRequestCommand, RelationshipChangeMetadataDTO>
{
    private readonly IDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    public async Task<RelationshipChangeMetadataDTO> Handle(CreateRelationshipTerminationRequestCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _dbContext
            .Set<Relationship>()
            .IncludeAll()
            .WithParticipant(_userContext.GetAddress())
            .FirstWithId(request.Id, cancellationToken);

        var change = relationship.RequestTermination(_userContext.GetAddress(), _userContext.GetDeviceId());

        _dbContext.Set<Relationship>().Update(relationship);
        await _dbContext.SaveChangesAsync(cancellationToken);

        PublishIntegrationEvent(change);

        return _mapper.Map<RelationshipChangeMetadataDTO>(change);
    }

    private void PublishIntegrationEvent(RelationshipChange change)
    {
        var evt = new RelationshipChangeCreatedIntegrationEvent(change);
        _eventBus.Publish(evt);
    }
}
