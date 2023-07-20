using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;

public class Handler : IRequestHandler<DeleteClientCommand>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, ILogger<Handler> logger)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace($"Deleting client with id: '{request.ClientId}'.");

        await _oAuthClientsRepository.Delete(request.ClientId, cancellationToken);

        _logger.LogTrace($"Successfully deleted client with id '{request.ClientId}'.");
    }
}
