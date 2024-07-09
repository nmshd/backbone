using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext, IMapper mapper)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
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

        await _relationshipTemplatesRepository.Add(template, cancellationToken);

        return _mapper.Map<CreateRelationshipTemplateResponse>(template);
    }
}
