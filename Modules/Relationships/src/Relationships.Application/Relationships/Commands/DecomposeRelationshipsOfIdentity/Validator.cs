﻿using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationshipsOfIdentity;

public class Validator : AbstractValidator<DecomposeRelationshipsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DecomposeRelationshipsOfIdentityCommand, IdentityAddress>();
    }
}
