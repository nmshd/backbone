using Backbone.Relationships.Application.Relationships.DTOs;
using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class GetRelationshipTemplateQuery : IRequest<RelationshipTemplateDTO>
{
    public RelationshipTemplateId Id { get; set; }
}
