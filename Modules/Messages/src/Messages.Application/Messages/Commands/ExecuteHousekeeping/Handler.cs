using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Messages.Application.Messages.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IMessagesRepository messagesRepository, ILogger<Handler> logger)
    {
        _messagesRepository = messagesRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteMessages(cancellationToken);
    }

    private async Task DeleteMessages(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numberOfDeletedItems = await _messagesRepository.Delete(Message.CanBeCleanedUp, cancellationToken);
        stopwatch.Stop();

        _logger.DataDeleted(numberOfDeletedItems, "messages", stopwatch.ElapsedMilliseconds);
    }
}
