using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

public class Handler : IRequestHandler<CreateRelationshipCommand, CreateRelationshipResponse>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;
    private CancellationToken _cancellationToken;
    private CreateRelationshipCommand _request;
    private RelationshipTemplate _template;
    private Relationship _relationship;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _relationshipsRepository = relationshipsRepository;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;

        _request = null!;
        _template = null!;
        _relationship = null!;
    }

    public async Task<CreateRelationshipResponse> Handle(CreateRelationshipCommand request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _request = request;

        await ReadTemplateFromDb();
        await EnsureRelationshipCanBeEstablished();
        await CreateAndSaveRelationship();

        return CreateResponse();
    }

    private async Task ReadTemplateFromDb()
    {
        _template = await _relationshipTemplatesRepository.Find(_request.RelationshipTemplateId, _userContext.GetAddress(), _cancellationToken, track: true, fillContent: false);
    }

    private async Task EnsureRelationshipCanBeEstablished()
    {
        EnsureActiveIdentityIsNotTemplateOwner();
        await EnsureThereIsNoExistingRelationshipBetweenActiveIdentityAndTemplateOwner();
    }

    private void EnsureActiveIdentityIsNotTemplateOwner()
    {
        if (_template.CreatedBy == _userContext.GetAddress())
            throw new OperationFailedException(ApplicationErrors.Relationship.CannotSendRelationshipRequestToYourself());
    }

    private async Task EnsureThereIsNoExistingRelationshipBetweenActiveIdentityAndTemplateOwner()
    {
        var relationshipExists = await _relationshipsRepository.RelationshipBetweenTwoIdentitiesExists(_userContext.GetAddress(), _template.CreatedBy, _cancellationToken);

        if (relationshipExists)
            throw new OperationFailedException(ApplicationErrors.Relationship.RelationshipToTargetAlreadyExists(_template.CreatedBy));
    }

    private async Task CreateAndSaveRelationship()
    {
        _relationship = new Relationship(
            _template,
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            _request.Content);

        await _relationshipsRepository.Add(_relationship, _cancellationToken);
    }

    private CreateRelationshipResponse CreateResponse()
    {
        return _mapper.Map<CreateRelationshipResponse>(_relationship);
    }
}
