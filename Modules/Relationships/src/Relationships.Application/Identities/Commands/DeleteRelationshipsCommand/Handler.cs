using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationshipCommand;

public class Handler(IRelationshipsRepository relationshipsRepository) : IRequestHandler<DeleteRelationshipsCommand>
{
    private readonly IRelationshipsRepository _relationshipsRepository = relationshipsRepository;

    public async Task Handle(DeleteRelationshipsCommand request, CancellationToken cancellationToken)
    {
        var relationships = await _relationshipsRepository.FindRelationshipsWithIdentityAddress(request.IdentityAddress, cancellationToken);
        await _relationshipsRepository.Delete(relationships.Select(r => r.Id), cancellationToken);
    }
}
