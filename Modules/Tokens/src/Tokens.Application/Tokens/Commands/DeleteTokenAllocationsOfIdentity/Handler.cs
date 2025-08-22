using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokenAllocationsOfIdentity;

public class Handler : IRequestHandler<DeleteTokenAllocationsOfIdentityCommand>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(DeleteTokenAllocationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var tokens = (await _tokensRepository.ListWithoutContent(Token.HasAllocationFor(IdentityAddress.Parse(request.IdentityAddress)), cancellationToken, track: true)).ToList();

        foreach (var token in tokens)
            token.DeleteTokenAllocation(request.IdentityAddress);

        await _tokensRepository.Update(tokens, cancellationToken);
    }
}
