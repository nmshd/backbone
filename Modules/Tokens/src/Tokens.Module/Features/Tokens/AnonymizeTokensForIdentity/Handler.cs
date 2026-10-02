using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(ITokensRepository tokensRepository, IOptions<ApplicationConfiguration> applicationOptions)
    {
        _tokensRepository = tokensRepository;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var tokens = (await _tokensRepository.ListWithoutContent(Token.IsFor(IdentityAddress.Parse(request.IdentityAddress)), cancellationToken)).ToList();

        foreach (var token in tokens)
            token.AnonymizeForIdentity(_applicationConfiguration.DidDomainName);

        await _tokensRepository.Update(tokens, cancellationToken);
    }
}
