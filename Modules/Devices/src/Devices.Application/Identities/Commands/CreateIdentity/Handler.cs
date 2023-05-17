using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class Handler : IRequestHandler<CreateIdentityCommand, CreateIdentityResponse>
{
    private readonly ApplicationOptions _applicationOptions;
    private readonly ITiersRepository _tiersRepository;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(ChallengeValidator challengeValidator, ILogger<Handler> logger, IEventBus eventBus, IOptions<ApplicationOptions> applicationOptions, ITiersRepository tiersRepository, IIdentitiesRepository identitiesRepository)
    {
        _challengeValidator = challengeValidator;
        _logger = logger;
        _eventBus = eventBus;
        _applicationOptions = applicationOptions.Value;
        _tiersRepository = tiersRepository;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<CreateIdentityResponse> Handle(CreateIdentityCommand command, CancellationToken cancellationToken)
    {
        var publicKey = PublicKey.FromBytes(command.IdentityPublicKey);
        await _challengeValidator.Validate(command.SignedChallenge, publicKey);

        _logger.LogTrace("Challenge sucessfully validated.");

        var address = IdentityAddress.Create(publicKey.Key, _applicationOptions.AddressPrefix);

        _logger.LogTrace($"Address created. Result: {address}");

        var existingIdentity = await _identitiesRepository.FindByAddress(address, cancellationToken);

        if (existingIdentity != null)
            throw new OperationFailedException(ApplicationErrors.Devices.AddressAlreadyExists());

        var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);

        var newIdentity = new Identity(command.ClientId, address, command.IdentityPublicKey, basicTier.Id, command.IdentityVersion);

        var user = new ApplicationUser(newIdentity);

        await _identitiesRepository.AddUser(user, command.DevicePassword);
        
        _logger.LogTrace($"Identity created. Address: {newIdentity.Address}, Device ID: {user.DeviceId}, Username: {user.UserName}");

        _eventBus.Publish(new IdentityCreatedIntegrationEvent(newIdentity));

        _logger.LogTrace($"Successfully published IdentityCreatedIntegrationEvent. Identity Address: {newIdentity.Address}, Tier: {basicTier.Name}");

        return new CreateIdentityResponse
        {
            Address = address,
            CreatedAt = newIdentity.CreatedAt,
            Device = new CreateIdentityResponseDevice
            {
                Id = user.DeviceId,
                Username = user.UserName,
                CreatedAt = user.Device.CreatedAt
            }
        };
    }
}
