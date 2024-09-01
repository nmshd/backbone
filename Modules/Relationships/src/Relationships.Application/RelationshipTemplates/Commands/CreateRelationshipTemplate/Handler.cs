using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Handler : IRequestHandler<CreateRelationshipTemplateCommand, CreateRelationshipTemplateResponse>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
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

        return new CreateRelationshipTemplateResponse(template);
    }
}
