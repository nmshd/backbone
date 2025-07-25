﻿using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplatesForIdentity;

public class Handler : IRequestHandler<AnonymizeRelationshipTemplatesForIdentityCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IOptions<ApplicationConfiguration> options)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _applicationConfiguration = options.Value;
    }

    public async Task Handle(AnonymizeRelationshipTemplatesForIdentityCommand request, CancellationToken cancellationToken)
    {
        var relationshipTemplates = (await _relationshipTemplatesRepository.ListWithoutContent(RelationshipTemplate.IsFor(IdentityAddress.Parse(request.IdentityAddress)), cancellationToken)).ToList();

        foreach (var relationshipTemplate in relationshipTemplates)
            relationshipTemplate.AnonymizeForIdentity(_applicationConfiguration.DidDomainName);

        await _relationshipTemplatesRepository.Update(relationshipTemplates, cancellationToken);
    }
}
