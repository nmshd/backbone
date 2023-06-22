using AutoMapper;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;
public class Handler : IRequestHandler<CreateClientCommand, CreateClientResponse>
{
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly IMapper _mapper;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager, IMapper mapper)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _mapper = mapper;
        _applicationManager = applicationManager;
    }

    public async Task<CreateClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var clientSecret = string.IsNullOrEmpty(request.ClientSecret) ? Password.Generate(30) : request.ClientSecret;
        var clientId = string.IsNullOrEmpty(request.ClientId) ? ClientIdGenerator.Generate() : request.ClientId;
        var displayName = string.IsNullOrEmpty(request.DisplayName) ? request.ClientId : request.DisplayName;

        if (await _applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken) != null)
        {
            throw new OperationFailedException(ApplicationErrors.Devices.ClientAlreadyExists());
        }
        
        await _oAuthClientsRepository.AddClients(clientId, displayName, clientSecret, cancellationToken);

        return new CreateClientResponse(clientId, displayName);
    }
}
