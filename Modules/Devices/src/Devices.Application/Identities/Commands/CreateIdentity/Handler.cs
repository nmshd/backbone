using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class Handler : IRequestHandler<CreateIdentityCommand, CreateIdentityResponse>
{
    private readonly ApplicationOptions _applicationOptions;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(ChallengeValidator challengeValidator, ILogger<Handler> logger, IEventBus eventBus, IOptions<ApplicationOptions> applicationOptions, IIdentitiesRepository identitiesRepository, IOAuthClientsRepository oAuthClientsRepository)
    {
        _challengeValidator = challengeValidator;
        _logger = logger;
        _eventBus = eventBus;
        _applicationOptions = applicationOptions.Value;
        _identitiesRepository = identitiesRepository;
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<CreateIdentityResponse> Handle(CreateIdentityCommand command, CancellationToken cancellationToken)
    {
        var publicKey = PublicKey.FromBytes(command.IdentityPublicKey);
        await _challengeValidator.Validate(command.SignedChallenge, publicKey);

        _logger.LogTrace("Challenge successfully validated.");

        var address = IdentityAddress.Create(publicKey.Key, _applicationOptions.AddressPrefix);

        _logger.LogTrace("Address created. Result: '{address}'", address);

        var addressAlreadyExists = await _identitiesRepository.Exists(address, cancellationToken);

        if (addressAlreadyExists)
            throw new OperationFailedException(ApplicationErrors.Devices.AddressAlreadyExists());

        var client = await _oAuthClientsRepository.Find(command.ClientId!, cancellationToken);

        var clientIdentityCount = await _identitiesRepository.CountByClientId(command.ClientId, cancellationToken);

        if (clientIdentityCount >= client.MaxIdentities)
            throw new OperationFailedException(ApplicationErrors.Devices.ClientReachedIdentitiesLimit());

        var newIdentity = new Identity(command.ClientId, address, command.IdentityPublicKey, client.DefaultTier, command.IdentityVersion);

        var user = new ApplicationUser(newIdentity);

        await _identitiesRepository.AddUser(user, command.DevicePassword);

        _logger.CreatedIdentity(newIdentity.Address, user.DeviceId, user.UserName!);

        _eventBus.Publish(new IdentityCreatedIntegrationEvent(newIdentity));

        return new CreateIdentityResponse
        {
            Address = address,
            CreatedAt = newIdentity.CreatedAt,
            Device = new CreateIdentityResponseDevice
            {
                Id = user.DeviceId,
                Username = user.UserName!,
                CreatedAt = user.Device.CreatedAt
            }
        };
    }
}

internal static partial class CreatedIdentityLogs
{
    [LoggerMessage(
        EventId = 436321,
        EventName = "Devices.CreateIdentity.CreatedIdentity",
        Level = LogLevel.Information,
        Message = "Identity created. Address: '{address}', Device ID: '{deviceId}', Username: '{userName}'.")]
    public static partial void CreatedIdentity(this ILogger logger, IdentityAddress address, DeviceId deviceId, string userName);
}
