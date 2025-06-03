using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokenAllocationsOfIdentity;

public class Handler : IRequestHandler<AnonymizeTokenAllocationsOfIdentityCommand>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(ITokensRepository tokensRepository, IOptions<ApplicationConfiguration> applicationOptions)
    {
        _tokensRepository = tokensRepository;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeTokenAllocationsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var tokens = (await _tokensRepository.List(Token.HasAllocationFor(IdentityAddress.Parse(request.IdentityAddress)), cancellationToken, track: true)).ToList();

        foreach (var token in tokens)
            token.AnonymizeTokenAllocation(request.IdentityAddress, _applicationConfiguration.DidDomainName);

        await _tokensRepository.Update(tokens, cancellationToken);
    }
}
