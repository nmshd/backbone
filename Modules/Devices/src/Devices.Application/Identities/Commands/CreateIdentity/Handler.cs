using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class Handler : IRequestHandler<CreateIdentityCommand, CreateIdentityResponse>
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;

    public Handler(ChallengeValidator challengeValidator, ILogger<Handler> logger, IOptions<ApplicationConfiguration> applicationOptions, IIdentitiesRepository identitiesRepository,
        IOAuthClientsRepository oAuthClientsRepository)
    {
        _challengeValidator = challengeValidator;
        _logger = logger;
        _applicationConfiguration = applicationOptions.Value;
        _identitiesRepository = identitiesRepository;
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<CreateIdentityResponse> Handle(CreateIdentityCommand command, CancellationToken cancellationToken)
    {
        var publicKey = await ValidateChallenge(command);
        _logger.LogTrace("Challenge successfully validated.");

        var address = await CreateIdentityAddress(publicKey, cancellationToken);
        _logger.LogTrace("Address created.");

        var newIdentity = await CreateNewIdentity(command, cancellationToken, address);
        await _identitiesRepository.Add(newIdentity, command.DevicePassword);
        _logger.CreatedIdentity();

        return new CreateIdentityResponse(newIdentity);
    }

    private async Task<Identity> CreateNewIdentity(CreateIdentityCommand command, CancellationToken cancellationToken, IdentityAddress address)
    {
        var client = await _oAuthClientsRepository.Get(command.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));

        var clientIdentityCount = await _identitiesRepository.CountByClientId(command.ClientId, cancellationToken);
        if (clientIdentityCount >= client.MaxIdentities)
            throw new OperationFailedException(ApplicationErrors.Devices.ClientReachedIdentitiesLimit());

        var communicationLanguageResult = CommunicationLanguage.Create(command.CommunicationLanguage);

        return new Identity(client.ClientId, address, command.IdentityPublicKey, client.DefaultTier, command.IdentityVersion, communicationLanguageResult.Value);
    }

    private async Task<IdentityAddress> CreateIdentityAddress(PublicKey publicKey, CancellationToken cancellationToken)
    {
        var address = IdentityAddress.Create(publicKey.Key, _applicationConfiguration.DidDomainName);

        var addressAlreadyExists = await _identitiesRepository.Exists(address, cancellationToken);
        if (addressAlreadyExists)
            throw new OperationFailedException(ApplicationErrors.Devices.AddressAlreadyExists());

        return address;
    }

    private async Task<PublicKey> ValidateChallenge(CreateIdentityCommand command)
    {
        var publicKey = PublicKey.FromBytes(command.IdentityPublicKey);
        await _challengeValidator.Validate(command.SignedChallenge, publicKey);
        return publicKey;
    }
}

internal static partial class CreatedIdentityLogs
{
    [LoggerMessage(
        EventId = 436321,
        EventName = "Devices.CreateIdentity.CreatedIdentity",
        Level = LogLevel.Information,
        Message = "Identity created.")]
    public static partial void CreatedIdentity(this ILogger logger);
}
