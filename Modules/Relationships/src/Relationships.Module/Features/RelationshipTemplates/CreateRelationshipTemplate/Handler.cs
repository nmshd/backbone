using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;

public class Handler : IRequestHandler<Command, Response>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var forIdentity = request.ForIdentity == null ? null : IdentityAddress.Parse(request.ForIdentity);
        var template = new RelationshipTemplate(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.MaxNumberOfAllocations,
            request.ExpiresAt,
            request.Content,
            forIdentity,
            request.Password);

        await _relationshipTemplatesRepository.Add(template, cancellationToken);

        return new Response(template);
    }
}
