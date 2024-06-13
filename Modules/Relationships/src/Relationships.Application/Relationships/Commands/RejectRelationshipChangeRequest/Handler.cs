using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;

public class Handler : IRequestHandler<RejectRelationshipChangeRequestCommand, RejectRelationshipChangeRequestResponse>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository)
    {
        _userContext = userContext;
        _relationshipsRepository = relationshipsRepository;
        _mapper = mapper;
    }

    public async Task<RejectRelationshipChangeRequestResponse> Handle(RejectRelationshipChangeRequestCommand changeRequest, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipsRepository.FindRelationship(changeRequest.Id, _userContext.GetAddress(), cancellationToken, track: true);

        relationship.RejectChange(changeRequest.ChangeId, _userContext.GetAddress(), _userContext.GetDeviceId(), changeRequest.ResponseContent);

        await _relationshipsRepository.Update(relationship);

        var response = _mapper.Map<RejectRelationshipChangeRequestResponse>(relationship);

        return response;
    }
}
