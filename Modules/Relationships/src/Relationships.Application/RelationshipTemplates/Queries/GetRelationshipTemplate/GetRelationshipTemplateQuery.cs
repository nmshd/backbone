using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class GetRelationshipTemplateQuery : IRequest<RelationshipTemplateDTO>
{
    public required RelationshipTemplateId Id { get; set; }
}
