﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse : IMapTo<RelationshipTemplate>
{
    public RelationshipTemplateId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
