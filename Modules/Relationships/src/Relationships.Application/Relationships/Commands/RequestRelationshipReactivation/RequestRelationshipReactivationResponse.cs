﻿using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RequestRelationshipReactivation;

public class RequestRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RequestRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
