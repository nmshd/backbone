using MediatR;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Ids;

namespace Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class GetRelationshipTemplateQuery : IRequest<RelationshipTemplateDTO>
{
    public RelationshipTemplateId Id { get; set; }
}
