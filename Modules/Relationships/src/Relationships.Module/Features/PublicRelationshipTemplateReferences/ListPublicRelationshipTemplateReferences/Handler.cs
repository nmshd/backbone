using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Backbone.Modules.Relationships.Module.Features.PublicRelationshipTemplateReferences.ListPublicRelationshipTemplateReferences;

public class Handler(IUserContext userContext, IConfiguration configuration)
    : IRequestHandler<ListPublicRelationshipTemplateReferencesQuery, IEnumerable<PublicRelationshipTemplateReferenceDefinition>>
{
    public Task<IEnumerable<PublicRelationshipTemplateReferenceDefinition>> Handle(ListPublicRelationshipTemplateReferencesQuery request, CancellationToken cancellationToken)
    {
        var definitions = new Dictionary<string, IEnumerable<PublicRelationshipTemplateReferenceDefinition>>();
        configuration.GetSection("Modules:Relationships:PublicRelationshipTemplateReferences").Bind(definitions);
        return Task.FromResult(definitions.GetValueOrDefault(userContext.GetClientId()) ?? []);
    }
}
