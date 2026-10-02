using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.PublicRelationshipTemplateReferences.ListPublicRelationshipTemplateReferences;

public class Query : IRequest<IEnumerable<PublicRelationshipTemplateReferenceDefinition>>;
