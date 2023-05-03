using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipsRepository relationshipsRepository, IUserContext userContext, IMapper mapper)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<CreateRelationshipTemplateResponse> Handle(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new RelationshipTemplate(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.MaxNumberOfAllocations,
            request.ExpiresAt,
            request.Content);

        await _relationshipsRepository.AddRelationshipTemplate(template, cancellationToken);

        return _mapper.Map<CreateRelationshipTemplateResponse>(template);
    }
}
