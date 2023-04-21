using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class Handler : IRequestHandler<CreateIdentityCommand, CreateIdentityResponse>
{
    private readonly ApplicationOptions _applicationOptions;
    private readonly ITiersRepository _tiersRepository;
    private readonly ChallengeValidator _challengeValidator;
    private readonly IDevicesDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;
    private readonly UserManager<ApplicationUser> _userManager;

    public Handler(IDevicesDbContext dbContext, UserManager<ApplicationUser> userManager, ChallengeValidator challengeValidator, ILogger<Handler> logger, IEventBus eventBus, IOptions<ApplicationOptions> applicationOptions, ITiersRepository tiersRepository)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _challengeValidator = challengeValidator;
        _logger = logger;
        _eventBus = eventBus;
        _applicationOptions = applicationOptions.Value;
        _tiersRepository = tiersRepository;
    }

    public async Task<CreateIdentityResponse> Handle(CreateIdentityCommand command, CancellationToken cancellationToken)
    {
        var publicKey = PublicKey.FromBytes(command.IdentityPublicKey);
        await _challengeValidator.Validate(command.SignedChallenge, publicKey);

        _logger.LogTrace("Challenge sucessfully validated.");

        var address = IdentityAddress.Create(publicKey.Key, _applicationOptions.AddressPrefix);

        _logger.LogTrace($"Address created. Result: {address}");

        var existingIdentity = await _dbContext.Set<Identity>().FirstWithAddressOrDefault(address, cancellationToken);

        if (existingIdentity != null)
            throw new OperationFailedException(ApplicationErrors.Devices.AddressAlreadyExists());

        var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);

        var newIdentity = new Identity(command.ClientId, address, command.IdentityPublicKey, basicTier.Id, command.IdentityVersion);

        var user = new ApplicationUser(newIdentity);

        var createUserResult = await _userManager.CreateAsync(user, command.DevicePassword);

        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));

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
