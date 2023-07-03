using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;

public class Handler : IRequestHandler<CreateClientCommand, CreateClientResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<CreateClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.ClientId))
        {
            var clientExists = await _oAuthClientsRepository.Exists(request.ClientId, cancellationToken);
            if (clientExists)
                throw new OperationFailedException(ApplicationErrors.Devices.ClientAlreadyExists());
        }

        var clientSecret = string.IsNullOrEmpty(request.ClientSecret) ? PasswordGenerator.Generate(30) : request.ClientSecret;
        var clientId = string.IsNullOrEmpty(request.ClientId) ? ClientIdGenerator.Generate() : request.ClientId;
        var displayName = string.IsNullOrEmpty(request.DisplayName) ? request.ClientId : request.DisplayName;

        await _oAuthClientsRepository.Add(clientId, displayName, clientSecret, cancellationToken);

        return new CreateClientResponse(clientId, displayName, clientSecret);
    }
}
